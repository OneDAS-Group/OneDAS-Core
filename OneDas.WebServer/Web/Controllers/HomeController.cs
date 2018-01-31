using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OneDas.Engine.Core;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Timers;

namespace OneDas.WebServer.Web
{
    public class HomeController: Controller
    {
        private static int _nextSubscriptionId;

        private static Timer _updatePerfInfoTimer;
        private static Timer _updateDataSnapshotTimer;
        private static Timer _updateLiveValueDataTimer;

        private static IHubContext<WebClientHub> _hubContext;

        private static OneDasEngine _oneDasEngine;

        static HomeController()
        {
            HomeController.LiveViewSubscriptionSet = new Dictionary<string, (int SubscriptionId, IList<ChannelHub> ChannelHubSet)>();

            _nextSubscriptionId = 1;
        }

        public HomeController(IHubContext<WebClientHub> hubContext, OneDasEngine oneDasEngine)
        {
            _hubContext = hubContext;

            if (_oneDasEngine == null)
            {
                _oneDasEngine = oneDasEngine;
                _oneDasEngine.OneDasStateChanged += HomeController.OneDasEngine_OneDasStateChanged;

                _updatePerfInfoTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
                _updateDataSnapshotTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
                _updateLiveValueDataTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromMilliseconds(200).TotalMilliseconds };

                _updatePerfInfoTimer.Elapsed += _updatePerfInfoTimer_Elapsed;
                _updateDataSnapshotTimer.Elapsed += _updateDataSnapshotTimer_Elapsed;
                _updateLiveValueDataTimer.Elapsed += _updateLiveValueDataTimer_Elapsed;
            }
        }

        public static Dictionary<string, (int SubscriptionId, IList<ChannelHub> ChannelHubSet)> LiveViewSubscriptionSet { get; private set; }

        public IActionResult Index()
        {
            return this.View("~/Web/Views/Home/Index.cshtml");
        }

        public IActionResult Error()
        {
            return this.View();
        }

        public static int GetNextSubscriptionId()
        {
            return _nextSubscriptionId++;
        }

        private static void _updatePerfInfoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                OneDasPerformanceInformation performanceInformation = _oneDasEngine.CreatePerformanceInformation();
                _hubContext?.Clients.All.InvokeAsync("SendPerformanceInformation", performanceInformation);
            }
        }

        private static void _updateDataSnapshotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                dataSnapshot = _oneDasEngine.CreateDataSnapshot();
                _hubContext?.Clients.All.InvokeAsync("SendDataSnapshot", DateTime.UtcNow, dataSnapshot);
            }
        }

        private static void _updateLiveValueDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (_oneDasEngine.OneDasState >= OneDasState.Ready)
            {
                foreach (var liveViewSubscription in HomeController.LiveViewSubscriptionSet)
                {
                    dataSnapshot = _oneDasEngine.CreateDataSnapshot(liveViewSubscription.Value.ChannelHubSet);
                    _hubContext?.Clients.Client(liveViewSubscription.Key).InvokeAsync("SendLiveViewData", liveViewSubscription.Value.SubscriptionId, DateTime.UtcNow, dataSnapshot);
                }
            }
        }

        public static void SendMessage(string message)
        {
            _hubContext?.Clients.All.InvokeAsync("SendMessage", message);
        }

        private static void OneDasEngine_OneDasStateChanged(object sender, OneDasStateChangedEventArgs e)
        {
            _hubContext?.Clients.All.InvokeAsync("SendOneDasState", e.NewState);
        }
    }
}
