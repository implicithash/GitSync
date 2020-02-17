using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using EW.Navigator.SCM.RestfulGit.Interfaces;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    internal class CommitResponse : ICommitResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<ICommit> Commits { get; set; }
    }
}

