
namespace EW.Navigator.SCM.SshClient.Interfaces
{
    /// <summary>
    /// Ssh credentials for remote connection
    /// </summary>
    public interface ISshClientCredentials
    {
        /// <summary>
        /// Ip address to be entered
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// User name of the remote host
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Private key authentication (path to the private key)
        /// </summary>
        string PrivateKey { get; set; }

        /// <summary>
        /// Private key authentication (passphrase)
        /// </summary>
        string PassPhrase { get; set; }
    }
}

