namespace EW.Navigator.SCM.Services.Interfaces
{
    /// <summary>
    ///Parameters for retrieving a list of commit objects (in plumbing format):
    ///GET /repos/:repo_key/git/commits/
    ///optional: ?start_sha=:sha
    ///    optional: ?ref_name=:ref_name
    ///    optional: ?limit=:limit(default=50, or as specified by the config)
    /// 
    /// Retrieves a specific commit object (plumbing format) given its SHA:
    /// GET /repos/:repo_key/git/commits/:sha/
    /// </summary>
    public interface IHttpRequestConfiguration
    {
        /// <summary>
        /// url of the query
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// sha from which the search begins
        /// </summary>
        string StartSha { get; set; }

        /// <summary>
        /// reference name
        /// </summary>
        string RefName { get; set; }

        /// <summary>
        /// the limit of the selected elements (default=50, or as specified by the config)
        /// </summary>
        int Limit { get; set; }

        /// <summary>
        /// sha
        /// </summary>
        string Sha { get; set; }
    }
}
