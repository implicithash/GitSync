using EW.Navigator.SCM.SshGit.Entities;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    /// <summary>
    /// Git transaction
    /// </summary>
    public interface IGitTransaction
    {
        /// <summary>
        /// Transaction state
        /// </summary>
        TransactionState State { get; }

        string RemotePath { get; }

        /// <summary>
        /// Path of the local git repository
        /// </summary>
        string LocalPath { get; }


        /// <summary>
        /// Begins a new transaction by cloning the repository and creating local branches for all remote branches.
        /// The repository will be cloned as a bare repository
        /// </summary>
        void Begin();

        /// <summary>
        /// Completes the transaction by pushing all commits created in the local repository to the remote repository
        /// </summary>
        void Commit(string defaultCommit = null);
    }
}

