using System;

namespace EW.Navigator.SCM.RestfulGit.Entities
{
    public class TreeEntry : IEquatable<TreeEntry>
    {
        protected TreeEntry() { }

        public virtual GitEntry GitEntry { get; set; }

        public bool Equals(TreeEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(GitEntry, other.GitEntry);
        }

        public override int GetHashCode()
        {
            return (GitEntry != null ? GitEntry.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TreeEntry) obj);
        }

        public static bool operator ==(TreeEntry left, TreeEntry right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(TreeEntry left, TreeEntry right) => !(left == right);
    }
}
