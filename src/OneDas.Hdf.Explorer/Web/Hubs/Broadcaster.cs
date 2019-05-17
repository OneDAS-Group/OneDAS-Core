using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Hdf.Core;
using OneDas.Hdf.Explorer.Core;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer.Web
{
    public class Broadcaster : Hub<IBroadcaster>
    {
        #region "Fields"

        private ILogger _logger;
        private HdfExplorerStateManager _stateManager;
        private DownloadService _downloadService;
        private HdfExplorerOptions _options;

        #endregion

        #region "Constructors"

        public Broadcaster(HdfExplorerStateManager stateManager, DownloadService downloadService, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options)
        {
            _stateManager = stateManager;
            _downloadService = downloadService;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger("HDF Explorer");
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
                    hdfExplorerState: _stateManager.GetState(this.Context.ConnectionId),
                    campaignInfoSet: Program.CampaignInfoSet,
                    campaignDescriptionSet: Program.CampaignDescriptionSet
                );
            });
        }

        public Task<HdfExplorerState> GetHdfExplorerState()
        {
            return Task.Run(() =>
            {
                return _stateManager.GetState(this.Context.ConnectionId);
            });
        }

        public Task<List<CampaignInfo>> UpdateCampaignInfoSet()
        {
            return Task.Run(() =>
            {
                _stateManager.CheckState(this.Context.ConnectionId);
                _stateManager.SetState(this.Context.ConnectionId, HdfExplorerState.Updating);

                Program.UpdateCampaignInfoSet();

                _stateManager.SetState(this.Context.ConnectionId, HdfExplorerState.Idle);

                return Program.CampaignInfoSet;
            });
        }

        public Task<Dictionary<string, string>> GetCampaignDescriptionSet()
        {
            return Task.Run(() =>
            {
                return Program.CampaignDescriptionSet;
            });
        }

        // TODO: Unify Download and GetData
        public void Download(DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateDescription, string campaignPath, List<string> variableNameSet)
        {
            _ = _downloadService.Download(this.Context.GetHttpContext().Connection.RemoteIpAddress, dateTimeBegin, dateTimeEnd, fileFormat, fileGranularity, sampleRateDescription, campaignPath, variableNameSet);
        }

        public void GetData(DateTime dateTimeBegin, DateTime dateTimeEnd, string sampleRateDescription, FileFormat fileFormat, FileGranularity fileGranularity, Dictionary<string, Dictionary<string, List<string>>> campaignInfoSet)
        {
            _ = _downloadService.GetData(this.Context.GetHttpContext().Connection.RemoteIpAddress, dateTimeBegin, dateTimeEnd, sampleRateDescription, fileFormat, fileGranularity, campaignInfoSet);
        }

        public Task<string> GetCampaignDocumentation(string campaigInfoName)
        {
            return _downloadService.GetCampaignDocumentation(campaigInfoName);
        }

        public Task<string> GetInactivityMessage()
        {
            return Task.Run(() =>
            {
                DateTime startTime;
                DateTime stopTime;

                startTime = DateTime.UtcNow.Date.Add(_options.InactiveOn);
                stopTime = startTime.Add(_options.InactivityPeriod);

                return $"The database is offline for scheduled maintenance from { startTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture) } to { stopTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture) } UTC.";
            });
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            return _downloadService.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
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
