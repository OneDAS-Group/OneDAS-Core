using System.IO;

namespace OneDas.Hdf.VdsTool.FileSystem
{
    public class LocalFileSystemProvider : IFileSystemProvider
    {
        public void Connect()
        {
            //
        }

        public void Disconnect()
        {
            //
        }

        public void DownloadFile(string sourceFilePath, string targetFilePath)
        {
            File.Copy(sourceFilePath, targetFilePath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string[] GetDirectories(string sourceDirectoryPath, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(sourceDirectoryPath, searchPattern, searchOption);
        }
    }
}
