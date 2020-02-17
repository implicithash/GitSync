using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EW.Navigator.SCM.GitRepo.Sync;
using EW.Navigator.SCM.SshGit.Entities;
using EW.Navigator.Entities;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.Services;
using EW.Navigator.SCM.SshGitRepo.Interfaces;
using Nito.AsyncEx;

namespace Git.Tests
{
    [TestClass]
    public class UnitTestRemoteSync
    {
        /// <summary>
        /// Git remote synchronization through SSH 
        /// </summary>
        [TestMethod]
        public async Task TestMethodRemoteGitRepoSync()
        {
            if (File.Exists(@"keys\id_rsa.pub") is bool publicExists)
            {
                Assert.AreNotEqual(false, publicExists);
            }
            if (File.Exists(@"keys\id_rsa") is bool privateExists)
            {
                Assert.AreNotEqual(false, privateExists);
            }

            var repoRequest = new RepoApplicationRequest()
            {
                LocalPath = @"repos",
                RemotePath = @"ssh://git@192.168.14.73:22/home/git/repos/ansibletest.git",
                SshCredentials = new GitSshCredentials()
                {
                    UserName = "git",
                    PublicKey = @"keys\id_rsa.pub",
                    PrivateKey = @"keys\id_rsa",
                    Passphrase = ""
                },
                ApplicationInfos = new List<IAppInfo>()
                {
                    new AppInfo()
                    {
                        Id = Guid.Parse("01341CFF-20FD-456F-AD58-9E7714E2F89A"),
                        IsRegistered = true,
                        Group = "Local services AAA",
                        AppSettings = SetAppSettings()
                    }
                }
            };

            IRepoResult result = new RepoResult();

            var asyncLock = new AsyncLock();
            var ct = new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token;

            //using (await asyncLock.LockAsync(ct))
            //{
                try
                {
                    result = await new SshGitAnsibleRepoProvider().GitRepoSync(repoRequest);
                }
                catch (OperationCanceledException e)
                {
                    result.Message = $"{e.Message}. The permissible time (1 minute) is exceeded";
                    result.Status = ResultCode.Error;
                }
                catch (Exception e)
                {
                    result.Message = e.Message;
                    result.Status = ResultCode.Error;
                }
            //}

            Assert.AreNotEqual(ResultCode.Error, result.Status);
        }

        private List<ApplicationSetting> SetAppSettings()
        {
            var lst = new List<ApplicationSetting>
            {
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "ServerTcpPort",
                    Value = "3335"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "LocalPath",
                    Value = @"C:\Projects\Navigator\USSD"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "ServerTcpBaseAddress",
                    Value = @"net.tcp://localhost:3335"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "ServerApplicationName",
                    Value = @"CPR Server"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "ServerIp",
                    Value = @"fe80::1810:3a19:db07:5558%24"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "MachineName",
                    Value = @"VALEKSANDROVA"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "ProcessName",
                    Value = @"CPR.Launcher"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "ServerType",
                    Value = @"MainCprApplication"
                },
                new ApplicationSetting()
                {
                    Group = "CommonSettings",
                    Name = "DomainConfigFilePath",
                    Value = @"Config\Domain.config"
                }
            };
            return lst;
        }
    }
}
