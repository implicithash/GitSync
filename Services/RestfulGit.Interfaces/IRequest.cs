namespace EW.Navigator.SCM.RestfulGit.Interfaces
{
    /// <summary>
    /// Common entity for http request
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Http query
        /// GET /repos/:repo_key/git/commits/
        /// </summary>
        string Url { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Request for retrieving commits via restful api
    /// </summary>
    public interface ICommitsRequest : IRequest
    {
        /// <summary>
        /// Commit from which the search starts
        /// optional: ?start_sha=:sha
        /// </summary>
        string StartSha { get; set; }

        /// <summary>
        /// optional: ?ref_name=:ref_name
        /// </summary>
        string RefName { get; set; }

        /// <summary>
        /// Quantity of the commits to be retrieved
        /// optional: ?limit=:limit(default=50, or as specified by the config)
        /// </summary>
        int Limit { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Request for retrieving the only commit via restful api
    /// </summary>
    public interface IShaCommitRequest : IRequest
    {
        /// <summary>
        /// given SHA => :sha
        /// usage: GET /repos/:repo_key/git/commits/:sha/
        /// </summary>
        string Sha { get; set; }
    }
}
