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

        static HomeController()
        {
            HomeController.LiveViewSubscriptionSet = new Dictionary<string, (int SubscriptionId, IList<ChannelHub> ChannelHubSet)>();

            _nextSubscriptionId = 1;

            _updatePerfInfoTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
            _updateDataSnapshotTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
            _updateLiveValueDataTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromMilliseconds(200).TotalMilliseconds };

            _updatePerfInfoTimer.Elapsed += _updatePerfInfoTimer_Elapsed;
            _updateDataSnapshotTimer.Elapsed += _updateDataSnapshotTimer_Elapsed;
            _updateLiveValueDataTimer.Elapsed += _updateLiveValueDataTimer_Elapsed;

            Bootloader.OneDasController.OneDasEngine.OneDasStateChanged += HomeController.OneDasController_OneDasStateChanged;
        }

        public HomeController(IHubContext<WebClientHub> hubContext)
        {
            _hubContext = hubContext;
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
            if (Bootloader.OneDasController.OneDasEngine.OneDasState >= OneDasState.Ready)
            {
                OneDasPerformanceInformation performanceInformation = Bootloader.OneDasController.OneDasEngine.CreatePerformanceInformation();
                _hubContext?.Clients.All.InvokeAsync("SendPerformanceInformation", performanceInformation);
            }
        }

        private static void _updateDataSnapshotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (Bootloader.OneDasController.OneDasEngine.OneDasState >= OneDasState.Ready)
            {
                dataSnapshot = Bootloader.OneDasController.OneDasEngine.CreateDataSnapshot();
                _hubContext?.Clients.All.InvokeAsync("SendDataSnapshot", DateTime.UtcNow, dataSnapshot);
            }
        }

        private static void _updateLiveValueDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (Bootloader.OneDasController.OneDasEngine.OneDasState >= OneDasState.Ready)
            {
                foreach (var liveViewSubscription in HomeController.LiveViewSubscriptionSet)
                {
                    dataSnapshot = Bootloader.OneDasController.OneDasEngine.CreateDataSnapshot(liveViewSubscription.Value.ChannelHubSet);
                    _hubContext?.Clients.Client(liveViewSubscription.Key).InvokeAsync("SendLiveViewData", liveViewSubscription.Value.SubscriptionId, DateTime.UtcNow, dataSnapshot);
                }
            }
        }

        public static void SendClientMessage(string message)
        {
            _hubContext?.Clients.All.InvokeAsync("SendClientMessage", message);
        }

        private static void OneDasController_OneDasStateChanged(object sender, OneDasStateChangedEventArgs e)
        {
            _hubContext?.Clients.All.InvokeAsync("SendOneDasState", e.NewState);
        }
    }
}
