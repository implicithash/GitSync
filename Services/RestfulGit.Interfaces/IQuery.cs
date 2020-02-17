using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EW.Navigator.SCM.RestfulGit.Interfaces
{
    /// <summary>
    /// Common query entity for retrieving commits
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Common request
        /// </summary>
        IRequest Request { get; set; }

        /// <summary>
        /// Sending GET http request for retrieving commits
        /// </summary>
        /// <returns></returns>
        Task<IResponse> GitCommitsAsync();

        /// <summary>
        /// Settings for json serialization
        /// </summary>
        /// <returns></returns>
        JsonSerializerSettings SetJsonSerializerSettings();
    }

    /// <inheritdoc />
    /// <summary>
    /// Query entity for retrieving collection of the commits
    /// </summary>
    public interface ICommitsQuery : IQuery
    {
        string SetCommitUrl(ICommitsRequest request);
    }

    /// <inheritdoc />
    /// <summary>
    /// Query entity for retrieving the only commit
    /// </summary>
    public interface IShaCommitQuery : IQuery
    {

    }
}

