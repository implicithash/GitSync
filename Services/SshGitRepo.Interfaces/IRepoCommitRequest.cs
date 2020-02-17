using System.Collections.Generic;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.SshGit.Entities;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Request for retrieving local commit
    /// </summary>
    public interface IRepoCommitRequest : IRepoRequest
    {
        /// <summary>
        /// Filter for retrieving commit
        /// </summary>
        string CommitFilter { get; set; }
    }
}

