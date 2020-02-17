using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.SshGit.Entities
{
    /// <summary>
    /// Public key authentication
    /// </summary>
    public class GitSshCredentials
    {
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Path to public key file
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Path to private key file
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Passphrase
        /// </summary>
        public string Passphrase { get; set; }
    }
}

