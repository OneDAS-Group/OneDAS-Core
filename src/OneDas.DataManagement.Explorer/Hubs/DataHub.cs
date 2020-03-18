using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OneDas.Buffers;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Infrastructure;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Hubs
{
    public class DataHub : Hub
    {
        #region Fields

        private ILogger _logger;

        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerStateManager _stateManager;
        private DataService _dataService;

        #endregion

        #region Constructors

        public DataHub(OneDasExplorerStateManager stateManager,
                       OneDasDatabaseManager databaseManager,
                       DataService dataService,
                       ILoggerFactory loggerFactory)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _dataService = dataService;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        #region Methods

        public ChannelReader<string> ExportData(DateTime begin,
                                                DateTime end,
                                                FileFormat fileFormat,
                                                FileGranularity fileGranularity,
                                                List<string> channelNames,
                                                CancellationToken cancellationToken)
        {
            var remoteIpAddress = this.Context.GetHttpContext().Connection.RemoteIpAddress;
            var channel = Channel.CreateUnbounded<string>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = Task.Run(() => this.InternalExportData(channel.Writer, remoteIpAddress, begin, end, fileFormat, fileGranularity, channelNames, cancellationToken));

            return channel.Reader;
        }

        public ChannelReader<double[]> StreamData(string channelName,
                                                  DateTime begin,
                                                  DateTime end,
                                                  CancellationToken cancellationToken)
        {
            var remoteIpAddress = this.Context.GetHttpContext().Connection.RemoteIpAddress;
            var channel = Channel.CreateUnbounded<double[]>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = Task.Run(() => this.InternalStreamData(channel.Writer, remoteIpAddress, channelName, begin, end, cancellationToken));

            return channel.Reader;
        }

        public void InternalStreamData(ChannelWriter<double[]> writer,
                                       IPAddress remoteIpAddress,
                                       string channelName,
                                       DateTime begin,
                                       DateTime end,
                                       CancellationToken cancellationToken)
        {
            Exception localException = null;

            var message = $"{remoteIpAddress} streamed data: {begin.ToString("yyyy-MM-dd HH:mm:ss")} to {end.ToString("yyyy-MM-dd HH:mm:ss")} ... ";

            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // datasetInfo
                if (!_databaseManager.Database.TryFindDataset(channelName, out var dataset))
                    throw new Exception($"Could not find channel with name '{channelName}'.");

                var campaign = (CampaignInfo)dataset.Parent.Parent;

                // security check
                if (!this.IsCampaignAccessible(campaign, _databaseManager.Config.RestrictedCampaigns))
                    throw new UnauthorizedAccessException($"The current user is not authorized to access the channel '{channelName}'.");

                // dataReader
                var dataReader = dataset.IsNative ? _databaseManager.GetNativeDataReader(campaign.Id) : _databaseManager.AggregationDataReader;

                // progress changed event handling
                var handler = (EventHandler<double>)((sender, e) =>
                {
                    this.Clients.Client(this.Context.ConnectionId).SendAsync("Downloader.ProgressChanged", e, cancellationToken);
                });

                dataReader.Progress.ProgressChanged += handler;

                try
                {
                    // read and stream data
                    dataReader.Read(dataset, begin, end, 5 * 1000 * 1000UL, async progressRecord =>
                    {
                        var dataRecord = progressRecord.DatasetToRecordMap.First().Value;
                        var doubleData = BufferUtilities.ApplyDatasetStatus2(dataRecord.Dataset, dataRecord.Status);

                        await writer.WriteAsync(doubleData, cancellationToken);
                    }, cancellationToken);
                }
                finally
                {
                    dataReader.Progress.ProgressChanged -= handler;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                localException = ex;
            }

            _logger.LogInformation($"{message} Done.");
            writer.TryComplete(localException);
        }

        private async void InternalExportData(ChannelWriter<string> writer,
                                              IPAddress remoteIpAddress,
                                              DateTime begin,
                                              DateTime end,
                                              FileFormat fileFormat,
                                              FileGranularity fileGranularity,
                                              List<string> channelNames,
                                              CancellationToken cancellationToken)
        {
            Exception localException = null;

            var message = $"{remoteIpAddress} exported data: {begin.ToString("yyyy-MM-dd HH:mm:ss")} to {end.ToString("yyyy-MM-dd HH:mm:ss")} ... ";

            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                if (!channelNames.Any())
                    throw new Exception("The list of channel names is empty.");

                var handler = (EventHandler<ProgressUpdatedEventArgs>)((sender, e) =>
                {
                    this.Clients.Client(this.Context.ConnectionId).SendAsync("Downloader.ProgressChanged", e.Progress, e.Message);
                });

                _dataService.Progress.ProgressChanged += handler;

                try
                {
                    var datasets = channelNames.Select(channelName =>
                    {
                        if (!_databaseManager.Database.TryFindDataset(channelName, out var dataset))
                            throw new Exception($"Could not find the channel with name '{channelName}'.");

                        var campaign = (CampaignInfo)dataset.Parent.Parent;

                        // security check
                        if (!this.IsCampaignAccessible(campaign, _databaseManager.Config.RestrictedCampaigns))
                            throw new UnauthorizedAccessException($"The current user is not authorized to access the channel '{channelName}'.");

                        return dataset;
                    }).ToList();

                    var sampleRates = datasets.Select(dataset => dataset.SampleRate);

                    if (sampleRates.Select(sampleRate => sampleRate.SamplesPerSecond).Distinct().Count() > 1)
                        throw new Exception("Channels with different sample rates have been requested.");

                    var sampleRate = sampleRates.First();

                    var url = await _dataService.ExportDataAsync(remoteIpAddress, begin, end, sampleRate, fileFormat, fileGranularity, datasets, cancellationToken);
                    await writer.WriteAsync(url, cancellationToken);
                }
                catch (Exception ex)
                {
                    await this.Clients.Client(this.Context.ConnectionId).SendAsync("Downloader.Error", ex.Message);
                    throw;
                }
                finally
                {
                    _dataService.Progress.ProgressChanged -= handler;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                localException = ex;
            }

            _logger.LogInformation($"{message} Done.");
            writer.TryComplete(localException);
        }

        private bool IsCampaignAccessible(CampaignInfo campaign, List<string> restrictedCampaigns)
        {
            var principal = this.Context.User;
            return Utilities.IsCampaignAccessible(principal, campaign, restrictedCampaigns);
        }

        #endregion
    }
}
