using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Database;
using OneDas.Hdf.Explorer.Web;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer.Core
{
    public class DownloadService
    {
        private ILogger _logger;
        private HdfExplorerStateManager _stateManager;
        private IHubContext<Broadcaster> _context;
        private HdfExplorerOptions _options;
        private string _connectionId;

        public DownloadService(HdfExplorerStateManager stateManager, IHubContext<Broadcaster> context, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options, string connectionId)
        {
            _stateManager = stateManager;
            _context = context;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
            _options = options.Value;
            _connectionId = connectionId;
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState(_connectionId);

                using var dataSource = Program.GetDataSource(campaignName);
                return dataSource.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
            });
        }

        public async Task Download(ChannelWriter<string> writer, IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateWithUnit, string campaignPath, List<string> variableNames)
        {
            try
            {
                var campaignInfo = Program.GetCampaigns().FirstOrDefault(current => current.Name == campaignPath);

                if (campaignInfo == null)
                    throw new Exception($"Could not find campaign with path '{campaignPath}'.");

                var campaignInfoSet = new Dictionary<string, Dictionary<string, List<string>>>();
                campaignInfoSet[campaignInfo.Name] = new Dictionary<string, List<string>>();

                foreach (var variableName in variableNames)
                {
                    VariableInfo variableInfo;

                    variableInfo = campaignInfo.VariableInfos.FirstOrDefault(current => current.VariableNames.Contains(variableName));

                    if (campaignInfo == null)
                        throw new Exception($"Could not find variable with name '{variableName}' in campaign '{campaignInfo.Name}'.");

                    if (!variableInfo.DatasetInfos.Any(current => current.Name == sampleRateWithUnit))
                        throw new Exception($"Could not find dataset in variable with ID '{variableInfo.Name}' ({variableInfo.VariableNames.First()}) in campaign '{campaignInfo.Name}'.");

                    campaignInfoSet[campaignInfo.Name][variableInfo.Name] = new List<string>() { sampleRateWithUnit };
                }

                var url = await this.GetData(remoteIpAddress, dateTimeBegin, dateTimeEnd, new SampleRateContainer(sampleRateWithUnit), fileFormat, fileGranularity, campaignInfoSet);
                await writer.WriteAsync(url);
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
                throw;
            }

            writer.TryComplete();
        }

        public Task<string> GetData(IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, SampleRateContainer sampleRate, FileFormat fileFormat, FileGranularity fileGranularity, Dictionary<string, Dictionary<string, List<string>>> campaignInfoSet)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState(_connectionId);

                if (!campaignInfoSet.Any() || dateTimeBegin == dateTimeEnd)
                    return string.Empty;

                // zip file
                var zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{dateTimeBegin.ToString("yyyy-MM-ddTHH-mm")}_{sampleRate.ToUnitString(underscore: true)}_{Guid.NewGuid().ToString()}.zip");

                // sampleRate
                var samplesPerSecond = sampleRate.SamplesPerSecond;

                // epoch & hyperslab
                var epochStart = new DateTime(2000, 01, 01);
                var epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

                var start = (ulong)(Math.Floor((dateTimeBegin - epochStart).TotalSeconds * samplesPerSecond));
                var block = (ulong)(Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalSeconds * samplesPerSecond));

                try
                {
                    // byte count
                    var bytesPerRow = 0UL;

                    foreach (var campaignInfo in campaignInfoSet)
                    {
                        using var dataSource = Program.GetDataSource(campaignInfo.Key);

                        foreach (var variableInfo in campaignInfo.Value)
                        {
                            foreach (string datasetInfo in variableInfo.Value)
                            {
                                bytesPerRow = dataSource.GetElementSize(campaignInfo.Key, variableInfo.Key, datasetInfo);
                            }
                        }
                    }

                    this.GetClient().SendAsync("SendByteCount", bytesPerRow * block);

                    var segmentSize = (50 * 1024 * 1024) / bytesPerRow * bytesPerRow;
                    var segmentLength = segmentSize / bytesPerRow;

                    // ensure that dataset length is multiple of 1 minute
                    if ((segmentLength / samplesPerSecond) % 60 != 0)
                        segmentLength = (ulong)((ulong)(segmentLength / samplesPerSecond / 60) * 60 * samplesPerSecond);

                    // start
                    _stateManager.SetState(_connectionId, HdfExplorerState.Loading);

                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var campaignInfo in campaignInfoSet)
                        {
                            using var dataSource = Program.GetDataSource(campaignInfo.Key);
                            var hdfDataLoader = new HdfDataLoader(_stateManager.GetToken(_connectionId));

                            hdfDataLoader.ProgressUpdated += this.OnProgressUpdated;

                            if (!hdfDataLoader.WriteZipFileCampaignEntry(zipArchive, fileGranularity, fileFormat, new ZipSettings(dateTimeBegin, campaignInfo, dataSource, samplesPerSecond, start, block, segmentLength)))
                                return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
                finally
                {
                    _stateManager.SetState(_connectionId, HdfExplorerState.Idle);
                }

                var message = $"{remoteIpAddress} requested data: {dateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss")} to {dateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss")}";
                _logger.LogInformation(message);

                return $"download/{Path.GetFileName(zipFilePath)}";
            }, _stateManager.GetToken(_connectionId));
        }

        private void OnProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.GetClient().SendAsync("SendProgress", e.Progress, e.Message);
        }

        private IClientProxy GetClient()
        {
            return _context.Clients.Client(_connectionId);
        }
    }
}
