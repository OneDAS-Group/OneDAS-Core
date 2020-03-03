using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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

        public Task<string> GetDataAsync(IPAddress remoteIpAddress,
                                         DateTime dateTimeBegin,
                                         DateTime dateTimeEnd,
                                         SampleRateContainer sampleRateContainer,
                                         FileFormat fileFormat,
                                         FileGranularity fileGranularity,
                                         List<DatasetInfo> datasets,
                                         CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState();

                if (!datasets.Any() || dateTimeBegin == dateTimeEnd)
                    return string.Empty;

                // zip file
                var zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{dateTimeBegin.ToString("yyyy-MM-ddTHH-mm")}_{sampleRateContainer.ToUnitString(underscore: true)}_{Guid.NewGuid().ToString()}.zip");

                // sampleRate
#warning Is this safe?
                var samplesPerSecond = (ulong)sampleRateContainer.SamplesPerSecond;

                // epoch & hyperslab
#warning When removing this, search for all occurences, also in external projects
                var epochStart = new DateTime(2000, 01, 01);
                var epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

#warning Replace this by DateTime. In DataLoader Line 180 everything is converted back to datetime!
                var start = (ulong)Math.Floor((dateTimeBegin - epochStart).TotalSeconds * samplesPerSecond);
                var block = (ulong)Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalSeconds * samplesPerSecond);

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
                    if ((segmentLength / samplesPerSecond) % 60 != 0)
                        segmentLength = (segmentLength / samplesPerSecond / 60) * 60 * samplesPerSecond;

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

                            var zipSettings = new ZipSettings(dateTimeBegin, campaign, nativeDataReader, aggregationDataReader, samplesPerSecond, start, block, segmentLength);

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

                var message = $"{remoteIpAddress} requested data: {dateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss")} to {dateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss")}";
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
