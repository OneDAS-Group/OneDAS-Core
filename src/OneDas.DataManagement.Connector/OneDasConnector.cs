using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using OneDas.DataManagement.Infrastructure;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Connector
{
    public class OneDasConnector
    {
        #region Fields

        private string _baseUrl;
        private Logger _logger;
        private HttpClient _httpClient;
        private HubConnection _connection;

        private int _channelCount;
        private int _channelIndex;
        private bool _isStream;

        #endregion

        #region Constructors

        static OneDasConnector()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OneDasConnector.CurrentDomain_AssemblyResolve;
        }

        #endregion

        #region Methods

        public OneDasConnector(string hostName, int port, Action<string> logAction, string userName = "", string password = "", bool secure = false)
        {
            // base url
            var scheme = secure ? "https" : "http";

            var loginUrlBuilder = new UriBuilder()
            {
                Scheme = scheme,
                Host = hostName,
                Port = port,
            };

            _baseUrl = loginUrlBuilder.ToString();

            // http client
            _httpClient = new HttpClient();

            // logging
            _logger = new Logger(logAction);

            // connection
            _connection = this.BuildHubConnection(hostName, port, "datahub", userName, password, secure);

            // callbacks
            _connection.On("Downloader.ProgressChanged", (double progress, string message) =>
            {
                if (_isStream)
                    progress = (_channelIndex + progress) / _channelCount;

                _logger?.Log($"{progress * 100,3:0}%: {message}\n");
            });
        }

        public async Task ExportAsync(DateTime begin, DateTime end, FileFormat fileFormat, FileGranularity fileGranularity, List<string> channelNames, string targetDirectoryPath)
        {
            var url = string.Empty;

            _isStream = false;

            try
            {
                await _connection.StartAsync();

                var reader = await _connection.StreamAsChannelAsync<string>(
                                "ExportData",
                                begin,
                                end,
                                fileFormat,
                                fileGranularity,
                                channelNames);

                while (await reader.WaitToReadAsync())
                {
                    reader.TryRead(out url);
                }
            }
            finally
            {
                await _connection.StopAsync();
            }

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception("No URL received from the server.");

            var downloadFilePath = Path.GetTempFileName();

            // download data
            _logger.Log($"Downloading ZIP file from {_baseUrl}{url} to {downloadFilePath}.\n");

            var webClient = new WebClient();
            webClient.DownloadFile($"{_baseUrl}{url}", downloadFilePath);

            // unzip data
            using (var archive = ZipFile.OpenRead(downloadFilePath))
            {
                var entries = archive.Entries.ToList();
                var count = entries.Count;
                var index = 0;

                entries.ForEach(entry =>
                {
                    index++;
                    var targetFilePath = Path.Combine(targetDirectoryPath, entry.FullName);

                    _logger.Log($"Extracting file {index} of {count} ('{entry.FullName}').\n");

                    if (!File.Exists(targetFilePath))
                    {
                        Directory.CreateDirectory(targetDirectoryPath);
                        ZipFileExtensions.ExtractToFile(entry, targetFilePath);
                    }
                });
            }
        }

        public async Task<Dictionary<string, ChannelInfo>> LoadAsync(DateTime begin, DateTime end, List<string> channelNames)
        {
            _isStream = true;
            _channelIndex = 0;
            _channelCount = channelNames.Count;

            try
            {
                await _connection.StartAsync();
                var variables = await _connection.InvokeAsync<List<VariableInfo>>("GetChannelInfos", channelNames);
                var result = new Dictionary<string, ChannelInfo>();

                for (int i = 0; i < channelNames.Count; i++)
                {
                    _channelIndex = i;

                    var totalBuffer = new double[0];
                    var channelName = channelNames[i];

                    var reader = await _connection.StreamAsChannelAsync<double[]>(
                                    "StreamData",
                                    begin,
                                    end,
                                    channelName);

                    while (await reader.WaitToReadAsync())
                    {
                        reader.TryRead(out var buffer);
#warning improve: preallocate array
                        totalBuffer = totalBuffer.Concat(buffer).ToArray();
                    }

                    result[channelName] = new ChannelInfo(variables[i], channelName, totalBuffer);
                }

                return result;
            }
            finally
            {
                await _connection.StopAsync();
            }
        }

        private HubConnection BuildHubConnection(string hostName, int port, string hubName, string username, string password, bool secure)
        {
            var scheme = secure ? "https" : "http";

            var hubUrlBuilder = new UriBuilder()
            {
                Scheme = scheme,
                Host = hostName,
                Port = port,
                Path = hubName
            };

            if (string.IsNullOrWhiteSpace(username))
            {
                return new HubConnectionBuilder()
                     .WithUrl(hubUrlBuilder.ToString())
                     .AddMessagePackProtocol()
                     .Build();
            }
            else
            {
                var loginUrlBuilder = new UriBuilder()
                {
                    Scheme = scheme,
                    Host = hostName,
                    Port = port,
                    Path = "identity/account/generatetoken"
                };

                return new HubConnectionBuilder()
                     .WithUrl(hubUrlBuilder.ToString(), options =>
                     {
                         options.AccessTokenProvider = async () => await this.GetJwtToken(loginUrlBuilder.ToString(), username, password);
                     })
                     .AddMessagePackProtocol()
                     .Build();
            }
        }

        private async Task<string> GetJwtToken(string url, string username, string password)
        {
            var request = $"{{ 'username': '{username}', 'password': '{password}'}}".Replace('\'', '"');

            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadAsStringAsync();
            token = token.Substring(1, token.Length - 2);
            return token;
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
