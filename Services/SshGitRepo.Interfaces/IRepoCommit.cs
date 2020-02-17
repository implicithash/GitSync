using System;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    /// <summary>
    /// Data for local commits to be retrieved
    /// </summary>
    public interface IRepoCommit
    {
        /// <summary>
        /// Identifier (sha1)
        /// </summary>
        string Sha { get; set; }

        /// <summary>
        /// Committer
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// Git commit message 
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Date of the commit
        /// </summary>
        DateTimeOffset When { get; set; }
    }
}