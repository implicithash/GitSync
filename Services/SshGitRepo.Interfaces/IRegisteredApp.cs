using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    /// <summary>
    /// Registered applications and their groups
    /// </summary>
    public interface IRegisteredApp
    {
        /// <summary>
        /// Application identifier
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Server type
        /// </summary>
        string ServerType { get; set; }

        /// <summary>
        /// Group of the application
        /// </summary>
        string Group { get; set; }

        /// <summary>
        /// Server application name
        /// </summary>
        string ServerApplicationName { get; set; }

        /// <summary>
        /// Server ip or machine name
        /// </summary>
        string ServerIp { get; set; }

        /// <summary>
        /// Server tcp port
        /// </summary>
        string ServerTcpPort { get; set; }
    }
}
