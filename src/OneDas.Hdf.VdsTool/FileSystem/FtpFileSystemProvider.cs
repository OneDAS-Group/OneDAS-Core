using FluentFTP;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace OneDas.Hdf.VdsTool.FileSystem
{
    public class FtpFileSystemProvider : IFileSystemProvider
    {
        private FtpClient _client;

        public FtpFileSystemProvider(string address, string userName, string password)
        {
            _client = new FtpClient(address) { RetryAttempts = 1 };

            if (!string.IsNullOrEmpty(userName))
            {
                _client.Credentials = new NetworkCredential(userName, password);
            }
        }

        public void Connect()
        {
            _client.Connect();
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void DownloadFile(string sourceFilePath, string targetFilePath)
        {
            _client.DownloadFile(targetFilePath, sourceFilePath, verifyOptions: FtpVerify.Retry);
        }

        public bool FileExists(string filePath)
        {
            return _client.FileExists(filePath);
        }

        public string[] GetDirectories(string sourceDirectoryPath, string searchPattern, SearchOption searchOption)
        {
            FtpListOption option;

            switch (searchOption)
            {
                case SearchOption.AllDirectories:
                    option = FtpListOption.Recursive;
                    break;
                default:
                    option = FtpListOption.Auto;
                    break;
            }

            return _client.GetListing(sourceDirectoryPath, option)
                       .Where(item => item.Type == FtpFileSystemObjectType.Directory && Regex.IsMatch(item.FullName, searchPattern.Replace("*", ".*")))
                       .Select(item => item.FullName).ToArray();
        }
    }
}
