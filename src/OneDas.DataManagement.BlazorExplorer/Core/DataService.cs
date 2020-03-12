﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class DataService
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerStateManager _stateManager;
        private OneDasExplorerOptions _options;

        #endregion

        #region Constructors

        public DataService(OneDasExplorerStateManager stateManager,
                           OneDasDatabaseManager databaseManager,
                           ILoggerFactory loggerFactory,
                           IOptions<OneDasExplorerOptions> options)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
            _options = options.Value;

            this.Progress = new Progress<ProgressUpdatedEventArgs>();
        }

        #endregion

        #region Properties

        public Progress<ProgressUpdatedEventArgs> Progress { get; }

        #endregion

        #region Methods

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatisticsAsync(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState();

                using var dataReader = _databaseManager.GetNativeDataReader(campaignName);
                return dataReader.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
            });
        }

        public Task<string> ExportDataAsync(IPAddress remoteIpAddress,
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

                var message = $"{remoteIpAddress} requested data: {begin.ToString("yyyy-MM-dd HH:mm:ss")} to {end.ToString("yyyy-MM-dd HH:mm:ss")} ... ";
                _logger.LogInformation(message);

                if (!datasets.Any() || begin == end)
                    return string.Empty;

                // zip file
                var zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{begin.ToString("yyyy-MM-ddTHH-mm")}_{sampleRateContainer.ToUnitString(underscore: true)}_{Guid.NewGuid().ToString()}.zip");

                // sampleRate
                var samplesPerDay = sampleRateContainer.SamplesPerDay;

                try
                {
                    // convert datasets into campaigns
                    var campaignNames = datasets.Select(dataset => dataset.Parent.Parent.Name).Distinct();
                    var fullCampaigns = _databaseManager.GetCampaigns().Where(campaign => campaignNames.Contains(campaign.Name));

                    var campaigns = fullCampaigns.Select(fullCampaign =>
                    {
                        var currentDatasets = datasets.Where(dataset => dataset.Parent.Parent.Name == fullCampaign.Name).ToList();
                        return fullCampaign.ToSparseCampaign(currentDatasets);
                    });

                    // start
                    var blockSizeLimit = 50 * 1000 * 1000UL;

                    using var dataExporter = new DataExporter(zipFilePath, fileGranularity, fileFormat, cancellationToken);
                    dataExporter.Progress += this.OnProgress;

                    try
                    {
                        foreach (var campaign in campaigns)
                        {
                            using var nativeDataReader = _databaseManager.GetNativeDataReader(campaign.Name);
                            using var aggregationDataReader = _databaseManager.AggregationDataReader;

                            var zipSettings = new ZipSettings(campaign,
                                                              nativeDataReader, aggregationDataReader,
                                                              begin, end,
                                                              samplesPerDay,
                                                              blockSizeLimit);

                            if (!dataExporter.WriteZipFileCampaignEntry(zipSettings))
                                return string.Empty;
                        }
                    }
                    finally
                    {
                        dataExporter.Progress -= this.OnProgress;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }

                _logger.LogInformation($"{message} Done.");

                return $"export/{Path.GetFileName(zipFilePath)}";
            }, cancellationToken);
        }

        private void OnProgress(object sender, ProgressUpdatedEventArgs e)
        {
            ((IProgress<ProgressUpdatedEventArgs>)this.Progress).Report(e);
        }

        #endregion
    }
}
