using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.Contracts
{
    /// <summary>
    /// Manager for working with remote git repositories (connection through SSH)
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Synchronization with the updating service (ansible)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IRepoResult> ExecuteApplicationConfiguration(IRepoRequest request);
    }

    /// <summary>
    /// Common interface for repo request (connection through SSH)
    /// </summary>
    public interface IRepoRequest
    {
        /// <summary>
        /// Local path of git repository (the location of git clone in a new directory)
        /// </summary>
        string LocalPath { get; set; }

        /// <summary>
        /// Remote path for origin source repository
        /// </summary>
        string RemotePath { get; set; }
    }

    /// <summary>
    /// Common interface for the result of sync operation (connection through SSH)
    /// </summary>
    public interface IRepoResult
    {
        /// <summary>
        /// Status of the sync repo operation
        /// </summary>
        ResultCode Status { get; set; }

        /// <summary>
        /// Message describes the error
        /// </summary>
        string Message { get; set; }
    }


    /// <summary>
    /// Result code of the git repository synchronization
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// Success of the operation
        /// </summary>
        Success = 0,

        /// <summary>
        /// Failure of the operation
        /// </summary>
        Error = 1
    }
}