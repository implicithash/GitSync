using System;
using Newtonsoft.Json;

namespace EW.Navigator.SCM.RestfulGit.Entities
{
    /// <summary>
    /// Committer identity in Git
    /// </summary>
    public sealed class Signature : IEquatable<Signature>
    {
        public Signature(string name, string email, DateTimeOffset when)
        {
            Name = name;
            Email = email;
            Date = when;
        }

        /// <summary>
        /// Date
        /// </summary>
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Committer name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Committer email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        public bool Equals(Signature other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Date.Equals(other.Date) && string.Equals(Name, other.Name) && string.Equals(Email, other.Email);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Signature other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Signature left, Signature right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(Signature left, Signature right) => !(left == right);
    }
}

