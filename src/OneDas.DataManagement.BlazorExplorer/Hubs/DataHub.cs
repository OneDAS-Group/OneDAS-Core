using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.BlazorExplorer.Core;
using OneDas.Buffers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Hubs
{
    public class DataHub : Hub
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerStateManager _stateManager;

        #endregion

        #region Constructors

        public DataHub(OneDasExplorerStateManager stateManager, OneDasDatabaseManager databaseManager, ILoggerFactory loggerFactory)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        #region Methods

        public ChannelReader<double[]> StreamData(
                string channelName,
                DateTime begin,
                DateTime end,
                CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<double[]>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = Task.Run(() => this.InternalStreamData(channel.Writer, channelName, begin, end, cancellationToken));

            return channel.Reader;
        }

        public void InternalStreamData(ChannelWriter<double[]> writer, string channelName, DateTime begin, DateTime end, CancellationToken cancellationToken)
        {
            Exception localException = null;

            var httpContext = this.Context.GetHttpContext();
            var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
            var message = $"{remoteIpAddress} requested data: {begin.ToString("yyyy-MM-dd HH:mm:ss")} to {end.ToString("yyyy-MM-dd HH:mm:ss")} ... ";

            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // datasetInfo
                var channelNameParts = channelName.Split("/");

                if (channelNameParts.Length != 6)
                    throw new Exception($"The channel name '{channelName}' is invalid.");

                var campaignName = $"/{channelNameParts[1]}/{channelNameParts[2]}/{channelNameParts[3]}";
                var variableName = channelNameParts[4];
                var datasetName = channelNameParts[5];

                if (!_databaseManager.Database.TryFindDataset(campaignName, variableName, datasetName, out var dataset))
                    throw new Exception($"Could not find channel with name '{channelName}'.");

                // dataReader
                var dataReader = dataset.IsNative ? _databaseManager.GetNativeDataReader(campaignName) : _databaseManager.AggregationDataReader;

                // progress changed event handling
                var handler = (EventHandler<double>)((sender, e) =>
                {
                    this.Clients.Client(this.Context.ConnectionId).SendAsync("ProgressChanged", e, cancellationToken);
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

        //public async Task Download(ChannelWriter<string> writer, DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateWithUnit, string campaignName, List<string> variableNames)
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

        #endregion
    }
}
