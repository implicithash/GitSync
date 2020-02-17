using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EW.Navigator.Entities;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.GitRepo.Sync;
using EW.Navigator.SCM.SshGit.Entities;
using EW.Navigator.SCM.SshGitRepo.Interfaces;
using EW.Navigator.Servers.DEAM.Contracts;
using EW.Navigator.Servers.DEAM.Interfaces;
using EW.Navigator.Utilities;
using Nito.AsyncEx;

namespace EW.Navigator.SCM.Services
{
    public class AppConfigurationManager : IConfigurationManager, IDisposable
    {
        private readonly GitSshCredentials _sshCredentials;
        private readonly IApplicationManager _applicationService;
        private readonly object _lockObject = new object();
        protected readonly AsyncLock AsyncLock = new AsyncLock();

        private static readonly TraceSource Ts = new TraceSource("SCMModule");
        private readonly CancellationToken _cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(Settings.Default.MinutesDelay)).Token;

        public AppConfigurationManager(IApplicationManager applicationService)
        {
            _sshCredentials = new GitSshCredentials()
            {
                UserName = Settings.Default.UserName,
                PublicKey = Settings.Default.PublicKeyPath,
                PrivateKey = Settings.Default.PrivateKeyPath,
                Passphrase = Settings.Default.Passphrase
            };
            _applicationService = applicationService;
            _applicationService.UpdatedSettings += OnApplicationServiceOnUpdatedSettings;
        }

        private async void OnApplicationServiceOnUpdatedSettings(IEnumerable<ApplicationInfo> applicationInfos)
        {
            using (await AsyncLock.LockAsync(_cancellationToken))
            {
                try
                {
                    var appInfos = new List<IAppInfo>();
                    foreach (var appObj in applicationInfos)
                    {
                        var applicationSettings = _applicationService.GetSettings(appObj.Id, null);
                        if (!applicationSettings.Any())
                        {
                            Ts.TraceEvent(TraceEventType.Warning, () => $"Application settings are empty. Application name - {appObj.Name}, endpoint address - {appObj.EndpointAddress}");
                            return;
                        }
                        applicationSettings = applicationSettings.Where(_ => _.Group.Equals(Settings.Default.CommonSettings)).ToList();
                        appInfos.Add(new AppInfo() {
                            Id = appObj.Id,
                            IsRegistered = appObj.IsRegistered,
                            Group = appObj.Group,
                            AppSettings = applicationSettings });
                    }
                    var appRequest = SetApplicationRequest(_sshCredentials, appInfos);
                    var result = await ExecuteApplicationConfiguration(appRequest);
                }
                catch (OperationCanceledException exception)
                {
                    Ts.TraceEvent(TraceEventType.Error, () => $"{exception}. The permissible time (1 minute) is exceeded");
                }
            }
        }

        public async Task<IRepoResult> ExecuteApplicationConfiguration(IRepoRequest request)
        {
            IRepoResult result = new RepoResult();
            try
            {
                if (!(request is IRepoApplicationRequest applicationRequest))
                    throw new NullReferenceException(Properties.Resources.ApplicationRequest_Empty);
                if (applicationRequest?.SshCredentials == null) return null;

                result = await new SshGitAnsibleRepoProvider().GitRepoSync(applicationRequest);
            }
            catch (Exception exception)
            {
                Ts.TraceEvent(TraceEventType.Error, () => $"{exception}");
                result.Message = exception.Message;
                result.Status = ResultCode.Error;
            }
            return result;
        }

        private static IRepoApplicationRequest SetApplicationRequest(GitSshCredentials credentials, List<IAppInfo> applicationInfos)
        {
            return new RepoApplicationRequest()
            {
                LocalPath = Settings.Default.LocalPath,
                RemotePath = Settings.Default.RemotePath,
                SshCredentials = credentials,
                ApplicationInfos = applicationInfos
            };
        }

        #region IDisposable Support
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                lock (_lockObject)
                {
                    _applicationService.UpdatedSettings -= OnApplicationServiceOnUpdatedSettings;
                }
            }
            _disposed = true;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AppConfigurationManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
