using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.Interface
{
    public class Downloader
    {
        string _baseAddress;
        string _url;
        Logger _logger;
        HubConnection _connection;
        ManualResetEventSlim _manualResetEvent;
        CancellationTokenSource _cts;

        static Downloader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Downloader.CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName;

            assemblyName = args.Name.Split(',').First();

            if (!args.Name.Contains(".resources"))
            {
                return Assembly.Load(assemblyName);
            }

            return null;
        }

        public Downloader(string hostName, int port, Action<string> logAction)
        {
            _baseAddress = $"http://{hostName}:{port}/";
            _manualResetEvent = new ManualResetEventSlim();
            _cts = new CancellationTokenSource();

            _connection = this.BuildHubConnection(hostName, port, "broadcaster");

            _connection.On("SendProgress", (double percent, string message) =>
            {
                _logger?.Log($"{percent,3:0}%: {message}\n");
            });

            _connection.On("SendUrl", (string url) =>
            {
                _url = url;
                _cts.Cancel();
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
            return Task.Run(() =>
            {
                string downloadFilePath;
                string entryFilePath;

                WebClient webClient;

                // get URL
                _connection.InvokeAsync<string>(
                        "Download",
                        settings.DateTimeBegin,
                        settings.DateTimeEnd,
                        "MAT73",
                        settings.FileGranularity,
                        settings.SampleRateDescription,
                        settings.CampaignPath,
                        settings.VariableNameSet);

                try
                {
                    _manualResetEvent.Wait(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    //
                }

                if (string.IsNullOrWhiteSpace(_url))
                {
                    throw new Exception("No URL received from the server.");
                }

                downloadFilePath = Path.GetTempFileName();

                try
                {
                    // download data
                    _logger.Log($"Downloading ZIP file from {_baseAddress}{_url} to {downloadFilePath}.\n");

                    webClient = new WebClient();
                    webClient.DownloadFile($"{_baseAddress}{_url}", downloadFilePath);

                    // unzip data
                    using (var archive = ZipFile.OpenRead(downloadFilePath))
                    {
                        archive.Entries.ToList().ForEach(entry =>
                        {
                            entryFilePath = Path.Combine(targetDirectoryPath, entry.FullName);

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
                    {
                        File.Delete(downloadFilePath);
                    }
                }
            });
        }

        public void Disconnect()
        {
            _connection.StopAsync().Wait();
        }

        private HubConnection BuildHubConnection(string hostName, int port, string path)
        {
            UriBuilder uriBuilder;

            uriBuilder = new UriBuilder()
            {
                Host = hostName,
                Port = port,
                Path = path
            };

            return new HubConnectionBuilder()
                 .WithUrl(uriBuilder.ToString())
                 .Build();
        }
    }
}
