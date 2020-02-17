using EW.Navigator.SCM.RestfulGit.Entities;
using System.Collections.Generic;

namespace EW.Navigator.SCM.RestfulGit.Interfaces
{
    /// <summary>
    /// Data for commits to be retrieved through http request
    /// </summary>
    public interface ICommit
    {
        /// <summary>
        /// The person who originally wrote the code
        /// </summary>
        Signature Author { get; set; }

        /// <summary>
        /// The person who committed the code on behalf of the original author
        /// </summary>
        Signature Committer { get; set; }

        /// <summary>
        /// Indicates the location of a resource
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Identifier
        /// </summary>
        string Sha { get; set; }

        /// <summary>
        /// The entity which consists of url and sha
        /// </summary>
        TreeEntry TreeEntry { get; set; }

        /// <summary>
        /// Collection of git entries
        /// </summary>
        IEnumerable<GitEntry> Parents { get; set; }

        /// <summary>
        /// Git commit message
        /// </summary>
        string Message { get; set; }
    }
}

