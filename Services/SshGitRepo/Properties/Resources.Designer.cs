﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EW.Navigator.SCM.GitRepo.Sync.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EW.Navigator.SCM.GitRepo.Sync.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AppSettings or group are not given.
        /// </summary>
        internal static string AppInfo_Empty {
            get {
                return ResourceManager.GetString("AppInfo_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Application request is null/wrong format.
        /// </summary>
        internal static string ApplicationRequest_Empty {
            get {
                return ResourceManager.GetString("ApplicationRequest_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Application settings are not given or empty.
        /// </summary>
        internal static string ApplicationSettings_Empty {
            get {
                return ResourceManager.GetString("ApplicationSettings_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server ip address is not given.
        /// </summary>
        internal static string IpAddress_Empty {
            get {
                return ResourceManager.GetString("IpAddress_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Machine name is not given or empty.
        /// </summary>
        internal static string MachineName_Empty {
            get {
                return ResourceManager.GetString("MachineName_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server application name is not given.
        /// </summary>
        internal static string ServerApplicationName_Empty {
            get {
                return ResourceManager.GetString("ServerApplicationName_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server type is not given.
        /// </summary>
        internal static string ServerType_Empty {
            get {
                return ResourceManager.GetString("ServerType_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ssh credentials are not full or not given.
        /// </summary>
        internal static string SshCredentials_Empty {
            get {
                return ResourceManager.GetString("SshCredentials_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server tcp port is not given.
        /// </summary>
        internal static string TcpPort_Empty {
            get {
                return ResourceManager.GetString("TcpPort_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The changes from the transaction could not be pushed back to the remote repository because changes were made there.
        /// </summary>
        internal static string TransactionFailedMessage {
            get {
                return ResourceManager.GetString("TransactionFailedMessage", resourceCulture);
            }
        }
    }
}
