using Microsoft.AspNetCore.SignalR.Client;
using OneDas.DataManagement.Infrastructure;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Interface
{
    public class Downloader
    {
        #region Fields

        private string _baseAddress;
        private Logger _logger;
        private HubConnection _connection;

        #endregion

        #region Constructors

        static Downloader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Downloader.CurrentDomain_AssemblyResolve;
        }

        #endregion

        #region Methods

        public Downloader(string hostName, int port, Action<string> logAction)
        {
            _baseAddress = $"http://{hostName}:{port}/";
            _connection = this.BuildHubConnection(hostName, port, "datahub");

            _connection.On("Downloader.ProgressChanged", (double progress, string message) =>
            {
                _logger?.Log($"{progress * 100,3:0}%: {message}\n");
            });

            _connection.On("Downloader.Error", (string message) =>
            {
                _logger?.Log($"An error occured: '{message}'.\n");
            });

            _logger = new Logger(logAction);
        }

        public void Connect()
        {
            try
            {
                _connection.StartAsync().Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public Task DownloadAndExtract(DownloadSettings settings, string targetDirectoryPath)
        {
            return Task.Run(async () =>
            {
                // get URL
                var url = string.Empty;
                
                var reader = await _connection.StreamAsChannelAsync<string>(
                                    "ExportData",
                                    settings.DateTimeBegin,
                                    settings.DateTimeEnd,
                                    FileFormat.MAT73,
                                    settings.FileGranularity,
                                    settings.ChannelNames);

                while (await reader.WaitToReadAsync())
                {
                    reader.TryRead(out url);
                }

                if (string.IsNullOrWhiteSpace(url))
                    throw new Exception("No URL received from the server.");

                var downloadFilePath = Path.GetTempFileName();

                try
                {
                    // download data
                    _logger.Log($"Downloading ZIP file from {_baseAddress}{url} to {downloadFilePath}.\n");

                    var webClient = new WebClient();
                    webClient.DownloadFile($"{_baseAddress}{url}", downloadFilePath);

                    // unzip data
                    using (var archive = ZipFile.OpenRead(downloadFilePath))
                    {
                        archive.Entries.ToList().ForEach(entry =>
                        {
                            var entryFilePath = Path.Combine(targetDirectoryPath, entry.FullName);

                            _logger.Log($"Unzipping file {entry.FullName}.\n");

                            if (!File.Exists(entryFilePath))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(entryFilePath));
                                ZipFileExtensions.ExtractToFile(entry, entryFilePath);
                            }
                        });
                    }
                }
                finally
                {
                    if (File.Exists(downloadFilePath))
                        File.Delete(downloadFilePath);
                }
            });
        }

        public void Disconnect()
        {
            _connection.StopAsync().Wait();
        }

        private HubConnection BuildHubConnection(string hostName, int port, string path)
        {
            var uriBuilder = new UriBuilder()
            {
                Host = hostName,
                Port = port,
                Path = path
            };

            return new HubConnectionBuilder()
                 .WithUrl(uriBuilder.ToString())
                 .Build();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',').First();

            if (!args.Name.Contains(".resources"))
            {
                return Assembly.Load(assemblyName);
            }

            return null;
        }

        #endregion
    }
}
