using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.Explorer.Web;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public class DownloadService
    {
        private ILogger _logger;
        private OneDasExplorerStateManager _stateManager;
        private IHubContext<Broadcaster> _context;
        private HdfExplorerOptions _options;
        private string _connectionId;

        public DownloadService(OneDasExplorerStateManager stateManager, IHubContext<Broadcaster> context, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options, string connectionId)
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

                using var dataReader = Program.DatabaseManager.GetNativeDataReader(campaignName);
                return dataReader.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
            });
        }

        public async Task Download(ChannelWriter<string> writer, IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateWithUnit, string campaignName, List<string> variableNames)
        {
            try
            {
                var campaign = Program.DatabaseManager.GetCampaigns().FirstOrDefault(current => current.Name == campaignName);

                if (campaign == null)
                    throw new Exception($"Could not find campaign '{campaignName}'.");

                var campaigns = new Dictionary<string, Dictionary<string, List<string>>>();
                campaigns[campaign.Name] = new Dictionary<string, List<string>>();

                foreach (var variableName in variableNames)
                {
                    var variable = campaign.Variables.FirstOrDefault(current => current.VariableNames.Contains(variableName));

                    if (variable == null)
                        throw new Exception($"Could not find variable with name '{variableName}' in campaign '{campaign.Name}'.");

                    if (!variable.Datasets.Any(current => current.Name == sampleRateWithUnit))
                        throw new Exception($"Could not find dataset in variable with ID '{variable.Name}' ({variable.VariableNames.Last()}) in campaign '{campaign.Name}'.");

                    campaigns[campaign.Name][variable.Name] = new List<string>() { sampleRateWithUnit };
                }

                var url = await this.GetData(remoteIpAddress, dateTimeBegin, dateTimeEnd, new SampleRateContainer(sampleRateWithUnit), fileFormat, fileGranularity, campaigns);
                await writer.WriteAsync(url);
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
                throw;
            }

            writer.TryComplete();
        }

        public Task<string> GetData(IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, SampleRateContainer sampleRateContainer, FileFormat fileFormat, FileGranularity fileGranularity, Dictionary<string, Dictionary<string, List<string>>> campaignMap)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState(_connectionId);

                if (!campaignMap.Any() || dateTimeBegin == dateTimeEnd)
                    return string.Empty;

                // zip file
                var zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{dateTimeBegin.ToString("yyyy-MM-ddTHH-mm")}_{sampleRateContainer.ToUnitString(underscore: true)}_{Guid.NewGuid().ToString()}.zip");

                // sampleRate
                var samplesPerDay = sampleRateContainer.SamplesPerDay;

                // epoch & hyperslab
                var epochStart = new DateTime(2000, 01, 01);
                var epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

                var start = (ulong)Math.Floor((dateTimeBegin - epochStart).TotalDays * samplesPerDay);
                var block = (ulong)Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalDays * samplesPerDay);

                try
                {
                    // convert lightweight campaign map to real campaign structure
                    var campaigns = campaignMap.Select(campaignEntry =>
                    {
                        var campaignName = campaignEntry.Key;
                        var fullCampaign = Program.DatabaseManager.GetCampaigns().FirstOrDefault(campaign => campaign.Name == campaignEntry.Key);

                        if (fullCampaign is null)
                            throw new KeyNotFoundException($"The requested campaign '{campaignName}' is unknown.");

                        return fullCampaign.ToSparseCampaign(campaignEntry.Value);
                    }).ToList();

                    // byte count
                    var bytesPerRow = 0UL;

                    foreach (var campaign in campaigns)
                    {
                        foreach (var variable in campaign.Variables)
                        {
                            foreach (var dataset in variable.Datasets)
                            {
                                bytesPerRow += (ulong)OneDasUtilities.SizeOf(dataset.DataType);
                            }
                        }
                    }

                    this.GetClient().SendAsync("SendByteCount", bytesPerRow * block);

                    var segmentSize = (50 * 1024 * 1024) / bytesPerRow * bytesPerRow;
                    var segmentLength = segmentSize / bytesPerRow;

                    // ensure that dataset length is multiple of 1 minute
                    if ((segmentLength / samplesPerDay) % 60 != 0)
                        segmentLength = (ulong)((ulong)(segmentLength / samplesPerDay / 60) * 60 * samplesPerDay);

                    // start
                    _stateManager.SetState(_connectionId, OneDasExplorerState.Loading);

                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var campaign in campaigns)
                        {
                            using var nativeDataReader = Program.DatabaseManager.GetNativeDataReader(campaign.Name);
                            using var aggregationDataReader = Program.DatabaseManager.AggregationDataReader;

                            var hdfDataLoader = new DataLoader(_stateManager.GetToken(_connectionId));
                            hdfDataLoader.ProgressUpdated += this.OnProgressUpdated;

                            if (!hdfDataLoader.WriteZipFileCampaignEntry(zipArchive, fileGranularity, fileFormat, new ZipSettings(dateTimeBegin, campaign, nativeDataReader, aggregationDataReader, samplesPerDay, start, block, segmentLength)))
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
                    _stateManager.SetState(_connectionId, OneDasExplorerState.Idle);
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
