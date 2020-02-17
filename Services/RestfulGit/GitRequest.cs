using EW.Navigator.SCM.RestfulGit.Interfaces;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    public class CommitsRequest : ICommitsRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// usage: GET /repos/:repo_key/git/commits/
        /// </summary>
        public string Url { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// optional: ?start_sha=:sha
        /// </summary>
        public string StartSha { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// optional: ?ref_name=:ref_name
        /// </summary>
        public string RefName { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// optional: ?limit=:limit(default=50, or as specified by the config)
        /// </summary>
        public int Limit { get; set; }
    }

    public class ShaCommitRequest : IShaCommitRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// usage: GET /repos/:repo_key/git/commits/
        /// </summary>
        public string Url { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// given SHA =&gt; :sha
        /// usage: GET /repos/:repo_key/git/commits/:sha/
        /// </summary>
        public string Sha { get; set; }
    }
}

