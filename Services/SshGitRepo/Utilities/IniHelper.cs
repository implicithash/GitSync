using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EW.Navigator.SCM.GitRepo.Sync.Utilities
{
    public class IniFile
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultVal, StringBuilder retVal, int size, string filePath);

        public string Path { get; set; }

        /// <summary>
        /// The size of the buffer pointed to by the retVal parameter
        /// </summary>
        private const int Buffer = 255;
        public IniFile(string iniPath)
        {
            Path = iniPath ?? throw new FileNotFoundException("File 'hosts' is not found");
        }
        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder();
            GetPrivateProfileString(section, key, "", retVal, Buffer, Path);
            return retVal.ToString();
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, Path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(section, key, null);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section);
        }
        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}
