using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EW.Navigator.SCM.RestfulGit.Interfaces;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    internal class NullQuery : IQuery
    {
        private static readonly Lazy<IQuery> NullObject = new Lazy<IQuery>(() => new NullQuery());

        public static IQuery Empty => NullObject.Value;

        private NullQuery()
        {

        }

        public IRequest Request { get; set; }

        public Task<IResponse> GitCommitsAsync()
        {
            throw new NotImplementedException();
        }

        public JsonSerializerSettings SetJsonSerializerSettings()
        {
            throw new NotImplementedException();
        }
    }

    public class CommitsQuery : ICommitsQuery
    {
        private HttpClient _client;

        public IRequest Request { get; set; }

        public CommitsQuery() { }

        public CommitsQuery(IRequest request)
        {
            Request = request;
        }

        public async Task<IResponse> GitCommitsAsync()
        {
            var commits = new List<Commit>();

            if (!(Request is ICommitsRequest commitRequest))
                throw new NullReferenceException(Properties.Resources.CommitRequest_Empty);

            var url = SetCommitUrl(commitRequest);

            using (_client = new HttpClient())
            {
                var result = await GetCommitsAsync(url);
                return result;
            }
        }

        /// <summary>
        /// Send GET query for retrieving remote commits
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        private async Task<ICommitResponse> GetCommitsAsync(string url)
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
            var content = response.Content.ReadAsStringAsync().Result;

            var commits = JsonConvert.DeserializeObject<List<Commit>>(content, SetJsonSerializerSettings());

            commitResponse.StatusCode = response.StatusCode;
            commitResponse.Commits = commits;

            return commitResponse;
        }

        /// <summary>
        /// Set url query for retrieving remote commits
        /// </summary>
        /// <param name="request">Query parameters</param>
        /// <returns>url</returns>
        public string SetCommitUrl(ICommitsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Url))
            {
                throw new ArgumentNullException(Properties.Resources.RequestError);
            }
            return $"{request.Url}?start_sha={request.StartSha}&ref_name={request.RefName}&limit={request.Limit}";
        }

        public JsonSerializerSettings SetJsonSerializerSettings()
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

    public class ShaCommitQuery : IShaCommitQuery
    {
        public IRequest Request { get; set; }
        private HttpClient _client;

        public ShaCommitQuery(IRequest request)
        {
            Request = request;
        }

        public async Task<IResponse> GitCommitsAsync()
        {
            var commits = new List<Commit>();

            if (!(Request is IShaCommitRequest commitRequest))
                throw new NullReferenceException(Properties.Resources.CommitRequest_Empty);

            var url = $"{commitRequest.Url}/{commitRequest.Sha}";

            using (_client = new HttpClient())
            {
                var result = await GetCommitsAsync(url);
                return result;
            }
        }

        /// <summary>
        /// Send GET query
        /// </summary>
        /// <param name="url">query url</param>
        /// <returns></returns>
        private async Task<ICommitResponse> GetCommitsAsync(string url)
        {
            var response = await _client.GetAsync(new Uri(url));
            var commitResponse = new CommitResponse();
            var commits = new List<ICommit>();

            if (!response.IsSuccessStatusCode)
            {
                var errResponse = JsonConvert.DeserializeObject<CommitResponse>(response.Content.ReadAsStringAsync().Result, SetJsonSerializerSettings());
                commitResponse.StatusCode = response.StatusCode;
                commitResponse.Error = errResponse.Error;
                return commitResponse;
            }
            var content = response.Content.ReadAsStringAsync().Result;

            var commit = JsonConvert.DeserializeObject<Commit>(content, SetJsonSerializerSettings());
            if (commit != null)
                commits.Add(commit);

            commitResponse.StatusCode = response.StatusCode;
            commitResponse.Commits = commits;

            return commitResponse;
        }

        public JsonSerializerSettings SetJsonSerializerSettings()
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
}
