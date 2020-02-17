using EW.Navigator.SCM.GitRepo.Sync.Utilities;
using LibGit2Sharp;

namespace EW.Navigator.SCM.GitRepo.Sync.Transactions
{
    public class GitTransaction : BaseGitTransaction
    {
        public GitTransaction(string remotePath, string localPath, SshUserKeyCredentials credentials) : base(remotePath, localPath, credentials)
        {
        }
        protected override void OnTransactionCompleted() => DirectoryHelper.ClearRecursively(LocalPath);
    }
}
