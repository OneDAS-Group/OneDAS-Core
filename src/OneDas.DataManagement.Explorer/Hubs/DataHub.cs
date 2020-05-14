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

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignId, DateTime begin, DateTime end)
        {
#warning Always check that begin comes before end (applies also to other hub methods). Otherwise this could cause an overflow exception.
            return _dataService.GetDataAvailabilityStatisticsAsync(campaignId, begin, end);
        }

        public Task<List<VariableInfo>> GetChannelInfos(List<string> channelNames)
        {
            // translate channel names
            var variables = channelNames.Select(channelName =>
            {
                if (!_databaseManager.Database.TryFindDataset(channelName, out var dataset))
                    throw new Exception($"Could not find channel with name '{channelName}'.");

                return (VariableInfo)dataset.Parent;
            }).ToList();

            // security check
            var campaigns = variables.Select(variable => (CampaignInfo)variable.Parent).Distinct();

            foreach (var campaign in campaigns)
            {
                if (!Utilities.IsCampaignAccessible(this.Context.User, campaign.Id, _databaseManager.Config.RestrictedCampaigns))
                    throw new UnauthorizedAccessException($"The current user is not authorized to access campaign '{campaign.Id}'.");
            }

            return Task.FromResult(variables);
        }

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
            _ = Task.Run(() => 
            {
                Exception localException = null;

                try
                {
                    _stateManager.CheckState();

                    var datasets = channelNames.Select(channelName =>
                    {
                        if (!_databaseManager.Database.TryFindDataset(channelName, out var dataset))
                            throw new Exception($"Could not find the channel with name '{channelName}'.");

                        // security check comes later in DataService

                        return dataset;
                    }).ToList();

                    if (!datasets.Any())
                        throw new Exception("The list of channel names is empty.");

                    this.InternalExportData(channel.Writer, remoteIpAddress, begin, end, fileFormat, fileGranularity, datasets, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    localException = ex;
                }

                channel.Writer.TryComplete(localException);
            });

            return channel.Reader;
        }

        public ChannelReader<string> ExportDataByGroup(DateTime begin,
                                                DateTime end,
                                                FileFormat fileFormat,
                                                FileGranularity fileGranularity,
                                                string groupPath,
                                                CancellationToken cancellationToken)
        {
            var remoteIpAddress = this.Context.GetHttpContext().Connection.RemoteIpAddress;
            var channel = Channel.CreateUnbounded<string>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = Task.Run(() =>
            {
                Exception localException = null;

                try
                {
                    _stateManager.CheckState();

                    if (!_databaseManager.Database.TryFindDatasetsByGroup(groupPath, out var datasets))
                        throw new Exception($"Could not find any channels for group path '{groupPath}'.");                     

                    this.InternalExportData(channel.Writer, remoteIpAddress, begin, end, fileFormat, fileGranularity, datasets, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    localException = ex;
                }

                channel.Writer.TryComplete(localException);
            });

            return channel.Reader;
        }

        public ChannelReader<double[]> StreamData(DateTime begin,
                                                  DateTime end,
                                                  string channelName,
                                                  CancellationToken cancellationToken)
        {
            var remoteIpAddress = this.Context.GetHttpContext().Connection.RemoteIpAddress;
            var channel = Channel.CreateUnbounded<double[]>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = Task.Run(() => this.InternalStreamData(channel.Writer, remoteIpAddress, begin, end, channelName, cancellationToken));

            return channel.Reader;
        }

        private async void InternalExportData(ChannelWriter<string> writer,
                                              IPAddress remoteIpAddress,
                                              DateTime begin,
                                              DateTime end,
                                              FileFormat fileFormat,
                                              FileGranularity fileGranularity,
                                              List<DatasetInfo> datasets,
                                              CancellationToken cancellationToken)
        {
            DateTime.SpecifyKind(begin, DateTimeKind.Utc);
            DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var handler = (EventHandler<ProgressUpdatedEventArgs>)((sender, e) =>
            {
                this.Clients.Client(this.Context.ConnectionId).SendAsync("Downloader.ProgressChanged", e.Progress, e.Message);
            });

            _dataService.Progress.ProgressChanged += handler;

            try
            {
                var sampleRates = datasets.Select(dataset => dataset.GetSampleRate());

                if (sampleRates.Select(sampleRate => sampleRate.SamplesPerSecond).Distinct().Count() > 1)
                    throw new Exception("Channels with different sample rates have been requested.");

                var sampleRate = sampleRates.First();

                var url = await _dataService.ExportDataAsync(remoteIpAddress, begin, end, sampleRate, fileFormat, fileGranularity, datasets, cancellationToken);
                await writer.WriteAsync(url, cancellationToken);
            }
            finally
            {
                _dataService.Progress.ProgressChanged -= handler;
            }
        }

        public void InternalStreamData(ChannelWriter<double[]> writer,
                                       IPAddress remoteIpAddress,
                                       DateTime begin,
                                       DateTime end,
                                       string channelName,
                                       CancellationToken cancellationToken)
        {
            Exception localException = null;

            // log
            string userName;

            if (this.Context.User.Identity.IsAuthenticated)
                userName = this.Context.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) streams data: {begin.ToString("yyyy-MM-ddTHH:mm:ssZ")} to {end.ToString("yyyy-MM-ddTHH:mm:ssZ")} ...";
            _logger.LogInformation(message);

            DateTime.SpecifyKind(begin, DateTimeKind.Utc);
            DateTime.SpecifyKind(end, DateTimeKind.Utc);

            try
            {
                _stateManager.CheckState();

                // dataset
                if (!_databaseManager.Database.TryFindDataset(channelName, out var dataset))
                    throw new Exception($"Could not find channel with name '{channelName}'.");

                var campaign = (CampaignInfo)dataset.Parent.Parent;

                // security check
                if (!Utilities.IsCampaignAccessible(this.Context.User, campaign.Id, _databaseManager.Config.RestrictedCampaigns))
                    throw new UnauthorizedAccessException($"The current user is not authorized to access the campaign '{campaign.Id}'.");

                // dataReader
                using var dataReader = dataset.IsNative ? _databaseManager.GetNativeDataReader(campaign.Id) : _databaseManager.AggregationDataReader;

                // progress changed event handling
                var handler = (EventHandler<double>)((sender, e) =>
                {
                    this.Clients.Client(this.Context.ConnectionId).SendAsync("Downloader.ProgressChanged", e, "Loading data ...", cancellationToken);
                });

                dataReader.Progress.ProgressChanged += handler;

                try
                {
                    // read and stream data
                    dataReader.Read(dataset, begin, end, 5 * 1000 * 1000UL, async progressRecord =>
                    {
                        double[] doubleData = default;
                        var dataRecord = progressRecord.DatasetToRecordMap.First().Value;

                        if (dataset.IsNative)
                            doubleData = BufferUtilities.ApplyDatasetStatus2(dataRecord.Dataset, dataRecord.Status);
                        else
                            doubleData = (double[])dataRecord.Dataset;

                        // avoid throwing an uncatched exception here because this would crash the app
                        // the task in cancelled anyway
                        try
                        {
                           await writer.WriteAsync(doubleData, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogWarning($"{message} Cancelled.");
                        }
                    }, cancellationToken);
                }
                finally
                {
                    dataReader.Progress.ProgressChanged -= handler;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.Message}");
                localException = ex;
            }

            _logger.LogInformation($"{message} Done.");
            writer.TryComplete(localException);
        }

        #endregion
    }
}
