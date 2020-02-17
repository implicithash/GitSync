using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EW.Navigator.SCM.RestfulGit.Entities;
using EW.Navigator.SCM.RestfulGit.Interfaces;
using Newtonsoft.Json;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    public class Commit : GitEntry, ICommit
    {
        public Commit() { }

        [JsonProperty("committer")]
        public Signature Committer { get; set; }

        [JsonProperty("author")]
        public Signature Author { get; set; }

        public TreeEntry TreeEntry { get; set; }
        public IEnumerable<GitEntry> Parents { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

