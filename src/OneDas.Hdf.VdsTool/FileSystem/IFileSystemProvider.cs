using System.IO;

namespace OneDas.Hdf.VdsTool.FileSystem
{
    public interface IFileSystemProvider
    {
        void Connect();
        void Disconnect();
        void DownloadFile(string sourceFilePath, string targetFilePath);
        bool FileExists(string filePath);
        string[] GetDirectories(string sourceDirectoryPath, string searchPattern, SearchOption searchOption);
    }
}
