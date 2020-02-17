using System.Collections.Generic;
using System.Net;

namespace EW.Navigator.SCM.RestfulGit.Interfaces
{
    /// <summary>
    /// Common entity for http response 
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Error message
        /// </summary>
        string Error { get; set; }

        /// <summary>
        /// Http status code of the request
        /// </summary>
        HttpStatusCode StatusCode { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Response of retrieving commits via restful api
    /// </summary>
    public interface ICommitResponse : IResponse
    {
        /// <summary>
        /// Collection of the commits
        /// </summary>
        IEnumerable<ICommit> Commits { get; set; }
    }

    /// <summary>
    /// Query status
    /// </summary>
    public enum StatusCode
    {
        Success = 0,
        NotFound = 1
    }
}

