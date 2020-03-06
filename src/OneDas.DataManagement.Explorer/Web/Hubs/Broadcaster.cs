using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Web
{
    public class Broadcaster : Hub<IBroadcaster>
    {
        #region "Fields"

        private ILogger _logger;
        private IServiceProvider _serviceProvider;
        private OneDasExplorerStateManager _stateManager;
        private OneDasExplorerOptions _options;

        #endregion

        #region "Constructors"

        public Broadcaster(OneDasExplorerStateManager stateManager, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<OneDasExplorerOptions> options)
        {
            _stateManager = stateManager;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        #region "Methods"

        public override Task OnConnectedAsync()
        {
            _stateManager.Register(this.Context.ConnectionId);
            _logger.LogInformation($"{ this.Context.GetHttpContext().Connection.RemoteIpAddress } ({ this.Context.ConnectionId.Substring(0, 8) }) connected. { _stateManager.UserCount } client(s) are connected.");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _stateManager.Unregister(this.Context.ConnectionId);
            _logger.LogInformation($"{ this.Context.GetHttpContext().Connection.RemoteIpAddress } ({ this.Context.ConnectionId.Substring(0, 8) }) disconnected. { _stateManager.UserCount } client(s) are remaining.");

            return base.OnDisconnectedAsync(exception);
        }

        public Task<AppModel> GetAppModel()
        {
            return Task.Run(() =>
            {
                return new AppModel(
                    explorerState: _stateManager.GetState(this.Context.ConnectionId),
                    campaignInfoSet: Program.DatabaseManager.GetCampaigns()
                );
            });
        }

        public Task<OneDasExplorerState> GetState()
        {
            return Task.Run(() =>
            {
                return _stateManager.GetState(this.Context.ConnectionId);
            });
        }

        // TODO: Unify Download and GetData
        // TODO: do not use stream but simple task instead
        public ChannelReader<string> Download(DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateWithUnit, string campaignPath, List<string> variableNames)
        {
            Channel<string> channel;
            DownloadService downloadService;

            channel = Channel.CreateUnbounded<string>();
            downloadService = ActivatorUtilities.CreateInstance<DownloadService>(_serviceProvider, this.Context.ConnectionId);
            _ = downloadService.Download(channel.Writer, this.Context.GetHttpContext().Connection.RemoteIpAddress, dateTimeBegin, dateTimeEnd, fileFormat, fileGranularity, sampleRateWithUnit, campaignPath, variableNames);

            return channel;
        }

        public async Task<string> GetData(DateTime dateTimeBegin, DateTime dateTimeEnd, string sampleRateWithUnit, FileFormat fileFormat, FileGranularity fileGranularity, Dictionary<string, Dictionary<string, List<string>>> campaignInfoSet)
        {
            var downloadService = ActivatorUtilities.CreateInstance<DownloadService>(_serviceProvider, this.Context.ConnectionId);
            var sampleRate = new SampleRateContainer(sampleRateWithUnit);

            return await downloadService.GetData(this.Context.GetHttpContext().Connection.RemoteIpAddress, dateTimeBegin, dateTimeEnd, sampleRate, fileFormat, fileGranularity, campaignInfoSet);
        }

        public Task<string> GetInactivityMessage()
        {
            return Task.Run(() =>
            {
                DateTime startTime;
                DateTime stopTime;

                startTime = DateTime.UtcNow.Date.Add(_options.InactiveOn);
                stopTime = startTime.Add(_options.InactivityPeriod);

                return $"The database is offline for scheduled maintenance from {startTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} to {stopTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} UTC.";
            });
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            DownloadService downloadService;

            downloadService = ActivatorUtilities.CreateInstance<DownloadService>(_serviceProvider, this.Context.ConnectionId);
            return downloadService.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
        }

        public Task CancelGetData()
        {
            return Task.Run(() =>
            {
                _stateManager.Cancel(this.Context.ConnectionId);
            });
        }

        #endregion
    }
}
