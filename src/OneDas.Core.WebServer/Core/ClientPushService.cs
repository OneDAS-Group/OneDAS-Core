using Microsoft.AspNetCore.SignalR;
using OneDas.Core.Engine;
using OneDas.Core.ProjectManagement;
using OneDas.WebServer.Web;
using System;
using System.Collections.Generic;
using System.Timers;

namespace OneDas.WebServer.Core
{
    public class ClientPushService
    {
        #region "Fields"

        private int _nextSubscriptionId;

        private Timer _updatePerfInfoTimer;
        private Timer _updateDataSnapshotTimer;
        private Timer _updateLiveValueDataTimer;

        private OneDasEngine _oneDasEngine;
        private IHubContext<WebClientHub> _hubContext;

        public Dictionary<string, LiveViewSubscription> _connectionToSubscriptionMap;

        #endregion

        #region "Constructors"

        public ClientPushService(IHubContext<WebClientHub> hubContext, OneDasEngine engine)
        {
            _hubContext = hubContext;
            _oneDasEngine = engine;

            _connectionToSubscriptionMap = new Dictionary<string, LiveViewSubscription>();
            _nextSubscriptionId = 1;

            _oneDasEngine.OneDasStateChanged += this.OneOneDasStateChanged;

            _updatePerfInfoTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
            _updateDataSnapshotTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
            _updateLiveValueDataTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromMilliseconds(200).TotalMilliseconds };

            _updatePerfInfoTimer.Elapsed += OnUpdatePerfInfo;
            _updateDataSnapshotTimer.Elapsed += OnUpdataDataSnapshot;
            _updateLiveValueDataTimer.Elapsed += OnUpdateLiveValueData;
        }

        #endregion

        #region "Methods"

        public void SendClientMessage(string message)
        {
            _hubContext?.Clients.All.SendAsync("SendClientMessage", message);
        }

        public void SendNugetMessage(string message)
        {
            _hubContext?.Clients.All.SendAsync("SendNugetMessage", message);
        }

        public int Subscribe(string connectionId, IList<ChannelHubBase> channelHubSettingsSet)
        {
            int subscriptionId;

            subscriptionId = this.GetNextSubscriptionId();

            lock (_connectionToSubscriptionMap)
            {
                _connectionToSubscriptionMap[connectionId] = new LiveViewSubscription(subscriptionId, channelHubSettingsSet);
            }

            return subscriptionId;
        }

        public void Unsubscribe(string connectionId)
        {
            lock (_connectionToSubscriptionMap)
            {
                if (_connectionToSubscriptionMap.ContainsKey(connectionId))
                {
                    _connectionToSubscriptionMap.Remove(connectionId);
                }
            }
        }

        private int GetNextSubscriptionId()
        {
            return _nextSubscriptionId++;
        }

        private void OnUpdatePerfInfo(object sender, ElapsedEventArgs e)
        {
            OneDasPerformanceInformation performanceInformation;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                performanceInformation = _oneDasEngine.CreatePerformanceInformation();
                _hubContext.Clients.All.SendAsync("SendPerformanceInformation", performanceInformation);
            }
        }

        private void OnUpdataDataSnapshot(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                dataSnapshot = _oneDasEngine.CreateDataSnapshot();
                _hubContext.Clients.All.SendAsync("SendDataSnapshot", DateTime.UtcNow, dataSnapshot);
            }
        }

        private void OnUpdateLiveValueData(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                lock (_connectionToSubscriptionMap)
                {
                    foreach (var entry in _connectionToSubscriptionMap)
                    {
                        dataSnapshot = _oneDasEngine.CreateDataSnapshot(entry.Value.ChannelHubSet);
                        _hubContext.Clients.Client(entry.Key).SendAsync("SendLiveViewData", entry.Value.Id, DateTime.UtcNow, dataSnapshot);
                    }
                }
            }
        }

        private void OneOneDasStateChanged(object sender, OneDasStateChangedEventArgs e)
        {
            _hubContext.Clients.All.SendAsync("SendOneDasState", e.NewState);
        }

        #endregion
    }
}
