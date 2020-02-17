using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EW.Navigator.Entities;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.GitRepo.Sync.Transactions;
using EW.Navigator.SCM.GitRepo.Sync.Utilities;
using EW.Navigator.SCM.SshGit.Entities;
using EW.Navigator.SCM.SshGitRepo.Interfaces;
using EW.Navigator.Utilities;
using LibGit2Sharp;
using Newtonsoft.Json;

namespace EW.Navigator.SCM.GitRepo.Sync
{
    public class GitRepository : IDisposable
    {
        private readonly GitSshCredentials _gitSshCredentials;
        private IGitTransaction _transaction;

        private static readonly TraceSource Ts = new TraceSource("SCMModule");

        private List<RegisteredApp> _registeredApps = new List<RegisteredApp>();
        private readonly Dictionary<Guid, AppDbProperties> _appDb = new Dictionary<Guid, AppDbProperties>();
        private readonly StringBuilder _commitDetails = new StringBuilder();
        private readonly IList<string> _hostVarPaths = new List<string>();

        public GitRepository(GitSshCredentials gitSshCredentials)
        {
            _gitSshCredentials = gitSshCredentials;
            if (_gitSshCredentials == null)
                throw new NullReferenceException(Properties.Resources.SshCredentials_Empty);
        }

        private SshUserKeyCredentials SetSshAuthentication()
        {
            if (string.IsNullOrEmpty(_gitSshCredentials.UserName) || string.IsNullOrEmpty(_gitSshCredentials.PrivateKey) || string.IsNullOrEmpty(_gitSshCredentials.PublicKey))
            {
                throw new NotImplementedException(Properties.Resources.SshCredentials_Empty);
            }

            var credentials = new SshUserKeyCredentials
            {
                Username = _gitSshCredentials.UserName,
                PublicKey = _gitSshCredentials.PublicKey,
                PrivateKey = _gitSshCredentials.PrivateKey,
                Passphrase = _gitSshCredentials.Passphrase
            };

            return credentials;
        }

        private void UpdateApplicationConfigs(IList<IAppInfo> appInfos)
        {
            Clear();
            RegisterApplications(appInfos);

            UpdateHosts();
            UpdateHostVars(appInfos);

            // delete files of app configs with old names
            DeleteUnnecessaryFiles();
        }

        private void Clear()
        {
            _registeredApps.Clear();
            _appDb.Clear();
            _commitDetails.Clear();
            _hostVarPaths.Clear();
        }

        internal struct AppDbProperties
        {
            public string ServerType { get; set; }
            public string Group { get; set; }
            public string ServerApplicationName { get; set; }
            public string ServerIp { get; set; }
            public string ServerTcpPort { get; set; }
        }

        private void RegisterApplications(IEnumerable<IAppInfo> appInfos)
        {
            var dbPath = Path.Combine(_transaction.LocalPath, RepoSettings.Default.RegisteredAppsDb);
            if (!File.Exists(dbPath)) throw new FileNotFoundException();

            var content = File.ReadAllText(dbPath);

            _registeredApps = JsonConvert.DeserializeObject<List<RegisteredApp>>(content);
            _registeredApps?.ForEach(app => _appDb.Add(app.Id, new AppDbProperties()
            {
                Group = app.Group,
                ServerType = app.ServerType,
                ServerApplicationName = app.ServerApplicationName,
                ServerIp = app.ServerIp,
                ServerTcpPort = app.ServerTcpPort
            }));

            foreach (var appInfo in appInfos)
            {
                if (_appDb.TryGetValue(appInfo.Id, out var dbProperties))
                {
                    if (!appInfo.IsRegistered)
                    {
                        _commitDetails.Append($"removed: '{dbProperties.ServerApplicationName}'; ");
                        _appDb.Remove(appInfo.Id);
                    }
                    else
                    {
                        _commitDetails.Append($"updated the group: '{appInfo.Group}' of '{dbProperties.ServerApplicationName}' application; ");
                        dbProperties.Group = appInfo.Group;
                        _appDb[appInfo.Id] = dbProperties;
                    }
                }
                else
                {
                    if (!appInfo.IsRegistered) continue;

                    if (!SetProperties(appInfo.AppSettings, out dbProperties))
                        continue;
                    _commitDetails.Append($"added a new application: '{dbProperties.ServerApplicationName}'; ");
                    _appDb.Add(appInfo.Id, new AppDbProperties()
                    {
                        Group = appInfo.Group,
                        ServerType = dbProperties.ServerType,
                        ServerApplicationName = dbProperties.ServerApplicationName,
                        ServerIp = dbProperties.ServerIp,
                        ServerTcpPort = dbProperties.ServerTcpPort
                    });
                }
            }

            _registeredApps?.Clear();
            if (_registeredApps == null) _registeredApps = new List<RegisteredApp>();

            foreach (var app in _appDb)
            {
                _registeredApps.Add(new RegisteredApp()
                {
                    Id = app.Key,
                    ServerType = app.Value.ServerType,
                    Group = app.Value.Group,
                    ServerApplicationName = app.Value.ServerApplicationName,
                    ServerIp = app.Value.ServerIp,
                    ServerTcpPort = app.Value.ServerTcpPort
                });
                SetHostVarPaths(app.Value.ServerApplicationName, app.Value.ServerIp, app.Value.ServerTcpPort);
            }
            content = JsonConvert.SerializeObject(_registeredApps);
            using (var fs = File.Create(dbPath))
            {
                var info = new UTF8Encoding(true).GetBytes(content);
                fs.Write(info, 0, info.Length);
            }
        }

        private string SetHostVarPath(string serverAppName, string serverIp, string port)
        {
            var applicationName = serverAppName.Replace(" ", "_");
            var ansibleHostVar = $"{applicationName}_{serverIp}_{port}";
            var hostVarPath = Path.Combine(_transaction.LocalPath, RepoSettings.Default.HostVars, $"{ansibleHostVar}.yml");

            return hostVarPath;
        }

        private void SetHostVarPaths(string serverAppName, string serverIp, string port)
        {
            var hostVarPath = SetHostVarPath(serverAppName, serverIp, port);
            _hostVarPaths.Add(hostVarPath);
        }

        private void DeleteUnnecessaryFiles()
        {
            var allFiles = new List<string>(Directory.GetFiles(Path.Combine(_transaction.LocalPath, RepoSettings.Default.HostVars)));

            var localHostPath = Path.Combine(_transaction.LocalPath, RepoSettings.Default.HostVars, RepoSettings.Default.LocalHostVarPath);
            _hostVarPaths.Add(localHostPath);
            var unnecessaryFiles = allFiles.Except(_hostVarPaths).ToList();

            unnecessaryFiles.ForEach(x => { try { File.Delete(x); }
                catch (Exception)
                {
                    // ignored
                }
            });
        }

        private static string SetHostsConstants(string sha ="")
        {
            return $"[local]\r\nlocalhost ansible_connection=local checkout='{sha}'\r\n";
        }

        private void UpdateHosts()
        {
            var content = new StringBuilder();
            var iniDictionary = new Dictionary<string, List<string>>();
            foreach (var app in _registeredApps)
            {
                var group = app.Group.Replace(" ", "_");
                var groupSection = $"{app.ServerType}_{group}";
                if (iniDictionary.ContainsKey(groupSection))
                {
                    iniDictionary[groupSection].Add(app.ToString());
                }
                else
                {
                    iniDictionary.Add(groupSection, new List<string>() { app.ToString() });
                }
            }

            content.Append(SetHostsConstants());

            foreach (var item in iniDictionary)
            {
                content.Append($"[{item.Key}]\r\n");
                iniDictionary[item.Key].ForEach(app => { content.Append($"{app}\r\n"); });
            }

            var hostsPath = Path.Combine(_transaction.LocalPath, RepoSettings.Default.Hosts);
            if (!File.Exists(hostsPath)) throw new FileNotFoundException();

            using (var fs = File.Create(hostsPath))
            {
                var info = new UTF8Encoding(true).GetBytes(content.ToString());
                fs.Write(info, 0, info.Length);
            }
        }

        private void UpdateGroupVars()
        {
            var groupVarsPath = Path.Combine(_transaction.LocalPath, RepoSettings.Default.GroupVars);
        }

        private static EndpointAddress SetEndpointAddress(ApplicationInfo applicationInfo)
        {
            var address = RegParseHelper.Parse(applicationInfo.EndpointAddress, @"(?<ip>\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(?<port>\d{4})\b");
            if (!address.Any()) return null;

            if (!address.TryGetValue("ip", out var ip)) return null;
            return !address.TryGetValue("port", out var port) ? null :
                new EndpointAddress() { Ip = ip, Port = port };
        }

        private static bool SetProperties(IReadOnlyCollection<ApplicationSetting> settings, out AppDbProperties dbProperties)
        {
            dbProperties = new AppDbProperties();

            if (settings == null)
            {
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.ApplicationSettings_Empty);
                return false;
            }

            var serverType = settings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerType));
            if (serverType == null)
            {
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.ServerType_Empty);
                return false;
            }

            var serverApplicationName = settings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerApplicationName));
            if (serverApplicationName == null)
            {
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.ServerApplicationName_Empty);
                return false;
            }

            var serverIp = settings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.MachineName));
            if (serverIp == null)
            {
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.MachineName_Empty);
                return false;
            }

            var serverTcpPort = settings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerTcpPort));
            if (serverTcpPort == null)
            {
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.TcpPort_Empty);
                return false;
            }

            dbProperties.ServerType = serverType?.Value.ToString();
            dbProperties.ServerApplicationName = serverApplicationName?.Value.ToString();
            dbProperties.ServerIp = serverIp?.Value.ToString();
            dbProperties.ServerTcpPort = serverTcpPort?.Value.ToString();

            return true;
        }

        private bool SetEndpointAddress(IReadOnlyCollection<ApplicationSetting> settings, out EndpointAddress endpointAddress)
        {
            if (settings == null)
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.ApplicationSettings_Empty);

            var machineName = settings?.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.MachineName));
            if (machineName == null)
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.MachineName_Empty);

            var serverTcpPort = settings?.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerTcpPort));
            if (serverTcpPort == null)
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.TcpPort_Empty);

            endpointAddress = new EndpointAddress() { Ip = machineName?.Value.ToString(), Port = serverTcpPort?.Value.ToString() };

            return true;
        }

        private void UpdateHostVars(IEnumerable<IAppInfo> appInfos)
        {
            foreach (var appInfo in appInfos)
            {
                if (!_appDb.ContainsKey(appInfo.Id))
                    continue;
                var properties = _appDb[appInfo.Id];

                var hostVarPath = SetHostVarPath(properties.ServerApplicationName, properties.ServerIp, properties.ServerTcpPort);

                // add host var setting
                var content = new StringBuilder();
                content.Append("---\n");
                appInfo.AppSettings.ForEach(_ => { content = GenerateHostVars(content, _); });

                using (var fs = File.Create(hostVarPath))
                {
                    var info = new UTF8Encoding(true).GetBytes(content.ToString());
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        private static StringBuilder GenerateHostVars(StringBuilder content, ApplicationSetting setting)
        {
            return content.Append($"{setting.Name}: '{setting.Value}'\n");
        }

        /// <summary>
        /// Updating/removing the configuration of the app hosts (according groups)
        /// </summary>
        /// <param name="appInfos"></param>
        private void UpdateHosts(IEnumerable<IAppInfo> appInfos)
        {
            foreach (var appInfo in appInfos)
            {
                var group = appInfo.Group.Replace(" ", "_");

                var serverType = appInfo.AppSettings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerType));
                var serverAppName = appInfo.AppSettings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerApplicationName));
                if (serverType == null)
                {
                    Ts.TraceEvent(TraceEventType.Warning, () => $"{Properties.Resources.ServerType_Empty}, Server application name: {serverAppName}");
                    continue;
                }

                var complexGroup = $"{serverType.Value.ToString()}_{group}";

                if(!SetEndpointAddress(appInfo.AppSettings, out var address))
                    continue;

                if(!SetAnsibleHostVar(appInfo, address, out var ansibleHostVar))
                    continue;

                try
                {
                    var hostsPath = Path.Combine(_transaction.LocalPath, RepoSettings.Default.Hosts);
                    File.Move(hostsPath, Path.ChangeExtension(hostsPath, RepoSettings.Default.IniExtension));

                    var iniPath = $"{hostsPath}{RepoSettings.Default.IniExtension}";
                    if (!File.Exists(iniPath)) throw new FileNotFoundException();

                    var ini = new IniFile(iniPath);

                    var parameter = $"{ansibleHostVar} {RepoSettings.Default.AnsibleHostVariable}";

                    // update or delete the record in the config files
                    if (appInfo.IsRegistered)
                    {
                        ini.Write(complexGroup, parameter, address.Ip);
                    }
                    else
                    {
                        if(ini.KeyExists(parameter, complexGroup))
                            ini.DeleteKey(parameter, complexGroup);
                    }

                    File.Copy(iniPath, hostsPath);
                    File.Delete(iniPath);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        private bool SetAnsibleHostVar(IAppInfo appInfo, EndpointAddress address, out string ansibleHostVar)
        {
            ansibleHostVar = null;
            var serverAppName = appInfo.AppSettings.FirstOrDefault(_ => _.Name.Equals(RepoSettings.Default.ServerApplicationName));
            if (serverAppName == null)
            {
                Ts.TraceEvent(TraceEventType.Warning, () => Properties.Resources.ServerApplicationName_Empty);
                return false;
            }
            var appName = serverAppName.Value.ToString().Replace(" ", "_");
            
            ansibleHostVar = $"{appName}_{address.Ip}_{address.Port}";
            return true;
        }

        /*public CloneOptions CloningAuthentication()
        {
            var options = new CloneOptions();
            options.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials { Username = "Username", Password = "Password" };
            return options;
        }*/

        /// <summary>
        /// Synchronization of the application settings with remote git repo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IRepoResult> RemoteRepoSync(IRepoRequest request)
        {
            if (!(request is IRepoApplicationRequest applicationRequest))
                throw new NullReferenceException(Properties.Resources.ApplicationRequest_Empty);

            var credentials = SetSshAuthentication();

            if (_transaction == null)
                _transaction = CreateTransaction(request.RemotePath, request.LocalPath, credentials);

            _transaction.Begin();
            UpdateApplicationConfigs(applicationRequest.ApplicationInfos);

            var defaultCommit = string.Format(RepoSettings.Default.DefaultCommit, _commitDetails);
            _transaction.Commit(defaultCommit);

            return new RepoResult() { Status = ResultCode.Success };
        }

        protected IGitTransaction CreateTransaction(string remotePath, string localPath, SshUserKeyCredentials credentials)
        {
            return new GitTransaction(remotePath, localPath, credentials);
        }

        public void Dispose()
        {
            _transaction.Commit();
        }
    }
}
