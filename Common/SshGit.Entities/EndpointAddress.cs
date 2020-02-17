namespace EW.Navigator.SCM.SshGit.Entities
{
    /// <summary>
    /// Endpoint address of service application
    /// </summary>
    public class EndpointAddress
    {
        /// <summary>
        /// Service ip address
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Port number
        /// </summary>
        public string Port { get; set; }
    }
}