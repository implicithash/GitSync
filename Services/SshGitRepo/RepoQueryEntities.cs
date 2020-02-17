using System;
using System.Collections.Generic;
using EW.Navigator.Entities;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.SshGit.Entities;
using EW.Navigator.SCM.SshGitRepo.Interfaces;

namespace EW.Navigator.SCM.GitRepo.Sync
{
    public class RepoCommit : IRepoCommit
    {
        public string Sha { get; set; }

        public string Author { get; set; }

        public string Message { get; set; }

        public DateTimeOffset When { get; set; }
    }

    public class RepoRequest : IRepoRequest
    {
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
    }

    public class RepoCommitRequest : IRepoCommitRequest
    {
        public string LocalPath { get; set; }

        public string RemotePath { get; set; }
        public string CommitFilter { get; set; }
    }

    public class RepoApplicationRequest : IRepoApplicationRequest
    {
        public GitSshCredentials SshCredentials { get; set; }
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
        public List<IAppInfo> ApplicationInfos { get; set; }
        public string Sha { get; set; }
    }

    public class AppInfo : IAppInfo
    {
        public Guid Id { get; set; }
        public bool IsRegistered { get; set; }
        public string Group { get; set; }
        public List<ApplicationSetting> AppSettings { get; set; }
    }

    public class RepoResult : IRepoResult
    {
        public ResultCode Status { get; set; }
        public string Message { get; set; }
    }

    public class RepoCommitResult : IRepoCommitResult
    {
        public ResultCode Status { get; set; }
        public string Message { get; set; }
        public IEnumerable<IRepoCommit> Commits { get; set; }
    }

    public class RegisteredApp : IRegisteredApp
    {
        public Guid Id { get; set; }
        public string ServerType { get; set; }
        public string Group { get; set; }
        public string ServerApplicationName { get; set; }
        public string ServerIp { get; set; }
        public string ServerTcpPort { get; set; }

        public override string ToString()
        {
            var serverAppName = ServerApplicationName.Replace(" ", "_");
            return $"{serverAppName}_{ServerIp}_{ServerTcpPort} {RepoSettings.Default.AnsibleHostVariable}={ServerIp}";
        }
    }
}

