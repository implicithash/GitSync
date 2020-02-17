using System;
using EW.Navigator.SCM.Contracts;
using System.Collections.Generic;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Result of retrieving local commits (collection of commits)
    /// </summary>
    public interface IRepoCommitResult : IRepoResult
    {
        /// <summary>
        /// Collection of commits
        /// </summary>
        IEnumerable<IRepoCommit> Commits { get; set; }
    }
}

