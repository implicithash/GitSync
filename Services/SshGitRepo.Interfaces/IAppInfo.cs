using EW.Navigator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.SshGitRepo.Interfaces
{
    public interface IAppInfo
    {
        /// <summary>
        /// Application identifier
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Is application registered or not
        /// </summary>
        bool IsRegistered { get; set; }

        /// <summary>
        /// Group of the application (e.g. LocalServices)
        /// </summary>
        string Group { get; set; }

        /// <summary>
        /// Common settings which will be applied for updating (in configs)
        /// </summary>
        List<ApplicationSetting> AppSettings { get; set; }
    }
}
