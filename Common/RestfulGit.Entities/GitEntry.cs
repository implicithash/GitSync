using System;
using Newtonsoft.Json;

namespace EW.Navigator.SCM.RestfulGit.Entities
{
    public abstract class GitEntry : IEquatable<GitEntry>
    {
        protected GitEntry() { }

        [JsonProperty("url")]
        public virtual string Url { get; set; }

        /// <summary>
        /// sh1
        /// </summary>
        [JsonProperty("sha")]
        public virtual string Sha { get; set; }

        public bool Equals(GitEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Url, other.Url) && string.Equals(Sha, other.Sha);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Url != null ? Url.GetHashCode() : 0) * 397) ^ (Sha != null ? Sha.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((GitEntry) obj);
        }

        /*public static bool operator == (GitEntry left, GitEntry right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left.Equals(right);
        }

        public static bool operator != (GitEntry left, GitEntry right) => !(left == right);*/

    }
}

