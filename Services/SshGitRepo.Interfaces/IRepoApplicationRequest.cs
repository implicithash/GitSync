using EW.Navigator.Entities;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.SshGit.Entities;
using System.Collections.Generic;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Request for updating service
    /// </summary>
    public interface IRepoApplicationRequest : IRepoRequest
    {
        /// <summary>
        /// Ssh credentials for remote connection
        /// </summary>
        GitSshCredentials SshCredentials { get; set; }

        /// <summary>
        /// Collection of application entities which is due to be updated/deleted
        /// </summary>
        List<IAppInfo> ApplicationInfos { get; set; }

        /// <summary>
        /// Hash of the commit
        /// </summary>
        string Sha { get; set; }
    }
}
