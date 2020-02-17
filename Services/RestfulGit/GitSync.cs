using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EW.Navigator.SCM.RestfulGit.Interfaces;
using Newtonsoft.Json;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    public class GitSync
    {
        private HttpClient _client;

        /// <summary>
        /// Query for retrieving of the commits
        /// </summary>
        /// <typeparam name="TReq">Template parameter for request</typeparam>
        /// <typeparam name="TResult">Template parameter for result</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResult> GitCommitsAsync<TReq, TResult>(TReq request) where TReq : IRequest
                                                                                where TResult : IResponse
        {
            var commits = new List<Commit>();

            var url = string.Empty;
            if (typeof(TReq).IsAssignableFrom(typeof(ICommitsRequest)))
            {
                var commitRequest = (ICommitsRequest)request;
                url = SetCommitUrl(commitRequest);
            }
            if (typeof(TReq).IsAssignableFrom(typeof(IShaCommitRequest)))
            {
                var commitRequest = (IShaCommitRequest)request;
                url = $"{commitRequest.Url}/{commitRequest.Sha}";
            }


            using (_client = new HttpClient())
            {
                //_client.DefaultRequestHeaders.Add(RequestConstants.UserAgent, RequestConstants.UserAgentValue);
                ICommitResponse result = null;
                if (typeof(TReq).IsAssignableFrom(typeof(ICommitsRequest)))
                    result = await GetCommitsAsync(url);

                if (typeof(TReq).IsAssignableFrom(typeof(IShaCommitRequest)))
                    result = await GetCommitsAsync(url, true);

                return (TResult)result;
            }
        }

        /// <summary>
        /// Get url for commit query
        /// </summary>
        /// <param name="request">Parameters for query</param>
        /// <returns>url</returns>
        private static string SetCommitUrl(ICommitsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Url))
            {
                throw new ArgumentNullException(Properties.Resources.RequestError);
            }
            return $"{request.Url}?start_sha={request.StartSha}&ref_name={request.RefName}&limit={request.Limit}";
        }

        /// <summary>
        /// Send GET query for commit retrieving
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isOnly">the only record or not</param>
        /// <returns></returns>
        private async Task<ICommitResponse> GetCommitsAsync(string url, bool isOnly = false)
        {
            var response = await _client.GetAsync(new Uri(url));
            var commitResponse = new CommitResponse();

            if (!response.IsSuccessStatusCode)
            {
                var errResponse = JsonConvert.DeserializeObject<CommitResponse>(response.Content.ReadAsStringAsync().Result, SetJsonSerializerSettings());
                commitResponse.StatusCode = response.StatusCode;
                commitResponse.Error = errResponse.Error;
                return commitResponse;
            }

            var commits = new List<Commit>();
            Commit commit = null;

            var content = response.Content.ReadAsStringAsync().Result;

            if (!isOnly)
            {
                commits = JsonConvert.DeserializeObject<List<Commit>>(content, SetJsonSerializerSettings());
            }
            else
            {
                commit = JsonConvert.DeserializeObject<Commit>(content, SetJsonSerializerSettings());
                if (commit != null)
                    commits.Add(commit);
            }

            commitResponse.StatusCode = response.StatusCode;
            commitResponse.Commits = commits;

            return commitResponse;
        }

        /// <summary>
        /// Set the properties to be serialized
        /// </summary>
        /// <returns></returns>
        JsonSerializerSettings SetJsonSerializerSettings()
        {
            var resolver = new PropertyFilterResolver()
                .SetIncludedProperties<Commit>(
                    u => u.Committer,
                    u => u.Author,
                    u => u.Sha,
                    u => u.Url,
                    u => u.Message
                )
                .SetIncludedProperties<CommitResponse>(
                    e => e.Error);
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "YYYY-MM-DD",
                ContractResolver = resolver
            };
            return settings;
        }
    }

    public static class RequestConstants
    {
        /// <summary>
        ///Retrieves a list of commit objects (in plumbing format):
        ///GET /repos/:repo_key/git/commits/
        ///optional: ?start_sha=:sha
        ///    optional: ?ref_name=:ref_name
        ///    optional: ?limit=:limit(default=50, or as specified by the config)
        /// 
        /// Retrieves a specific commit object (plumbing format) given its SHA:
        /// GET /repos/:repo_key/git/commits/:sha/
        /// </summary>
        public static string CommitsUrl { get; set; } = "http://192.168.3.93:8000/repos/eastwind/git/commits";

        public static string Sha { get; set; } = "3ffd02ae7ac4c99febcd37a304030d2eca1e83b9";

        public static string RefName { get; set; } = "";

        public static int Limit { get; set; } = 2;

        public static string UserAgent { get; set; } = "";
        public static string UserAgentValue { get; set; } = "";
    }
}

