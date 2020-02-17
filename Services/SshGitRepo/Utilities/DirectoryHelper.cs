using System.IO;

namespace EW.Navigator.SCM.GitRepo.Sync.Utilities
{
    /// <summary>
    /// Utility class for recursively deleting of the files
    /// </summary>
    public static class DirectoryHelper
    {
        public static void DeleteRecursively(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(directoryPath) { Attributes = FileAttributes.Normal };

            // remove read-only flag from all files before deleting
            foreach (var info in directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directoryInfo.Delete(true);
        }


        /// <summary>
        /// Removes all items from the specified directory recursively if it exists.
        /// </summary>
        public static void ClearRecursively(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(directoryPath);
            foreach (var dir in directoryInfo.GetDirectories())
            {
                DeleteRecursively(dir.FullName);
            }

            foreach (var info in directoryInfo.GetFiles())
            {
                info.Attributes = FileAttributes.Normal;
                info.Delete();
            }
        }
    }
}
