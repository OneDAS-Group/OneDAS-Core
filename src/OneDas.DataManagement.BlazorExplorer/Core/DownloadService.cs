using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class DownloadService
    {
        #region Events

        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        #endregion

        #region Fields

        private ILogger _logger;
        private OneDasExplorerStateManager _stateManager;
        private OneDasExplorerOptions _options;

        #endregion

        #region Methods

        public DownloadService(OneDasExplorerStateManager stateManager, ILoggerFactory loggerFactory, IOptions<OneDasExplorerOptions> options)
        {
            _stateManager = stateManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
            _options = options.Value;
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatisticsAsync(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState();

                using var dataReader = Program.DatabaseManager.GetNativeDataReader(campaignName);
                return dataReader.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
            });
        }

        //public async Task DownloadAsync(ChannelWriter<string> writer, IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateWithUnit, string campaignName, List<string> variableNames)
        //{
        //    try
        //    {
        //        var campaign = Program.DatabaseManager.GetCampaigns().FirstOrDefault(current => current.Name == campaignName);

        //        if (campaign == null)
        //            throw new Exception($"Could not find campaign '{campaignName}'.");

        //        var campaigns = new Dictionary<string, Dictionary<string, List<string>>>();
        //        campaigns[campaign.Name] = new Dictionary<string, List<string>>();

        //        foreach (var variableName in variableNames)
        //        {
        //            var variable = campaign.Variables.FirstOrDefault(current => current.VariableNames.Contains(variableName));

        //            if (variable == null)
        //                throw new Exception($"Could not find variable with name '{variableName}' in campaign '{campaign.Name}'.");

        //            if (!variable.Datasets.Any(current => current.Name == sampleRateWithUnit))
        //                throw new Exception($"Could not find dataset in variable with ID '{variable.Name}' ({variable.VariableNames.Last()}) in campaign '{campaign.Name}'.");

        //            campaigns[campaign.Name][variable.Name] = new List<string>() { sampleRateWithUnit };
        //        }

        //        var url = await this.GetData(remoteIpAddress, dateTimeBegin, dateTimeEnd, new SampleRateContainer(sampleRateWithUnit), fileFormat, fileGranularity, campaigns);
        //        await writer.WriteAsync(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        writer.TryComplete(ex);
        //        throw;
        //    }

        //    writer.TryComplete();
        //}

        public Task<double[]> LoadDatasetAsync(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            return Task.Run(() =>
            {
                var campaignName = dataset.Parent.Parent.Name;

                using var dataReader = dataset.IsNative 
                    ? Program.DatabaseManager.GetNativeDataReader(campaignName) 
                    : Program.DatabaseManager.AggregationDataReader;

                var genericType = OneDasUtilities.GetTypeFromOneDasDataType(dataset.DataType);
                var dataStorage = (ISimpleDataStorage)OneDasUtilities.InvokeGenericMethod(dataReader,
                                            nameof(DataReaderExtensionBase.LoadDataset),
                                            BindingFlags.Instance | BindingFlags.Public,
                                            genericType,
                                            new object[] { dataset, begin, end });

                return dataStorage.DataBuffer.ToArray();
            });
        }

        public Task<string> GetDataAsync(IPAddress remoteIpAddress,
                                         DateTime begin,
                                         DateTime end,
                                         SampleRateContainer sampleRateContainer,
                                         FileFormat fileFormat,
                                         FileGranularity fileGranularity,
                                         List<DatasetInfo> datasets,
                                         CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState();

                if (!datasets.Any() || begin == end)
                    return string.Empty;

                // zip file
                var zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{begin.ToString("yyyy-MM-ddTHH-mm")}_{sampleRateContainer.ToUnitString(underscore: true)}_{Guid.NewGuid().ToString()}.zip");

                // sampleRate
                var samplesPerDay = sampleRateContainer.SamplesPerDay;
                var samplesPerMinute = sampleRateContainer.SamplesPerSecond / 60;

                try
                {
                    // byte count
                    var bytesPerRow = 0UL;

                    foreach (var dataset in datasets)
                    {
                        bytesPerRow += (ulong)OneDasUtilities.SizeOf(dataset.DataType);
                    }

                    var segmentSize = (50 * 1024 * 1024) / bytesPerRow * bytesPerRow;
                    var segmentLength = segmentSize / bytesPerRow;

                    // ensure that dataset length is multiple of 1 minute
                    if (segmentLength / samplesPerMinute != 0)
                        segmentLength = (ulong)((segmentLength / (ulong)samplesPerMinute) * samplesPerMinute);

                    // convert datasets into campaigns
                    var campaignNames = datasets.Select(dataset => dataset.Parent.Parent.Name).Distinct();
                    var fullCampaigns = Program.DatabaseManager.GetCampaigns().Where(campaign => campaignNames.Contains(campaign.Name));

                    var campaigns = fullCampaigns.Select(fullCampaign =>
                    {
                        var currentDatasets = datasets.Where(dataset => dataset.Parent.Parent.Name == fullCampaign.Name).ToList();
                        return fullCampaign.ToSparseCampaign(currentDatasets);
                    });

                    // start
                    using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var campaign in campaigns)
                        {
                            using var nativeDataReader = Program.DatabaseManager.GetNativeDataReader(campaign.Name);
                            using var aggregationDataReader = Program.DatabaseManager.AggregationDataReader;

                            var dataLoader = new DataLoader(cancellationToken);
                            dataLoader.ProgressUpdated += this.OnProgressUpdated;

                            var zipSettings = new ZipSettings(campaign,
                                                              nativeDataReader,
                                                              aggregationDataReader,
                                                              begin,
                                                              end,
                                                              samplesPerDay,
                                                              segmentLength);

                            if (!dataLoader.WriteZipFileCampaignEntry(zipArchive, fileGranularity, fileFormat, zipSettings))
                                return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }

                var message = $"{remoteIpAddress} requested data: {begin.ToString("yyyy-MM-dd HH:mm:ss")} to {end.ToString("yyyy-MM-dd HH:mm:ss")}";
                _logger.LogInformation(message);

                return $"export/{Path.GetFileName(zipFilePath)}";
            }, cancellationToken);
        }

        private void OnProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.ProgressUpdated?.Invoke(this, e);
        }

        #endregion
    }
}
