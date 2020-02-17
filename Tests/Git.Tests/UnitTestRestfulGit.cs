using System;
using System.Linq;
using EW.Navigator.SCM.RestfulGit.Interfaces;
using EW.Navigator.SCM.RestfulGit.Sync;
using EW.Navigator.SCM.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Git.Tests
{
    [TestClass]
    public class UnitTestRestfulGit
    {
        [TestMethod]
        public void TestMethodRestfulGit()
        {
            var config = new HttpRequestConfiguration()
            {
                Url = RequestConstants.Url,
                StartSha = RequestConstants.StartSha,
                RefName = RequestConstants.RefName,
                Limit = RequestConstants.Limit,
                Sha = RequestConstants.Sha
            };
            var container = RegisterFactory.Configure(config);
            Assert.AreNotEqual(null, container, "The container of the factory is null");

            var factory = container.Resolve<IQueryFactory>();
            Assert.AreNotEqual(null, factory, "The factory of the queries is null");

            //commits
            var commitsQuery = factory.Create(QueryType.Commits);
            Assert.AreNotEqual(null, commitsQuery, "The query object is null");

            var result = (ICommitResponse)commitsQuery.GitCommitsAsync().GetAwaiter().GetResult();
            Assert.AreNotEqual(null, result, "The result of the 'commits query' is null");
            Assert.IsTrue(result.Commits.Any(), "Commits are empty");

            //one commit
            var commitQuery = factory.Create(QueryType.OneCommit);
            Assert.AreNotEqual(null, commitQuery, "The query object is null");

            var resultCommit = (ICommitResponse)commitQuery.GitCommitsAsync().GetAwaiter().GetResult();
            Assert.AreNotEqual(null, resultCommit, "The result of the 'commit query' is null");
            Assert.IsTrue(resultCommit.Commits.Any(), "Commits are empty");
            Assert.IsTrue(resultCommit.Commits.Count() == 1, "Commits contain more than one element");
        }

        /// <summary>
        /// Constants for unit testing
        /// </summary>
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
            public static string Url { get; set; } = "http://192.168.3.93:8000/repos/eastwind/git/commits";

            public static string StartSha { get; set; } = "";

            public static string RefName { get; set; } = "";

            public static int Limit { get; set; } = 2;

            public static string Sha { get; set; } = "3ffd02ae7ac4c99febcd37a304030d2eca1e83b9";
        }
    }
}
