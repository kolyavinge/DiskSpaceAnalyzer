using System;
using System.IO;
using System.Linq;

namespace DiskSpaceAnalyzer.Model
{
    class FileSystem
    {
        private static readonly string _windowsDirectory = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName;
        public string[] GetDirectories(string path)
        {
            try
            {
                return Directory.GetDirectories(path).Where(x => x.Equals(_windowsDirectory, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        public string[] GetFiles(string path)
        {
            try
            {
                return Directory.GetFiles(path);
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        public long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }
    }
}
