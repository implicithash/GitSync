using System.Threading.Tasks;

namespace EW.Navigator.SCM.SshClient.Interfaces
{
    /// <summary>
    /// Declaration of ssh client 
    /// </summary>
    public interface ISshClient
    {
        /// <summary>
        /// Sending cmd command through ssh connection
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<bool> ExecuteShellCommand(string command);
    }
}

