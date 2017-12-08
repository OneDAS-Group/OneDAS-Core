using System;
using System.Collections.Generic;
using System.Timers;
using OneDas.Infrastructure;
using OneDas.Main.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace OneDas.Main.Web
{
    public class HomeController: Controller
    {
        private static int _nextSubscriptionId;

        private static Timer _updatePerfInfoTimer;
        private static Timer _updateDataSnapshotTimer;
        private static Timer _updateLiveValueDataTimer;

        private static IHubContext<Broadcaster> _hubContext;

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

            Bootloader.OneDasController.OneDasStateChanged += OneDasController_OneDasStateChanged;
            Bootloader.SystemMessageReceived += HomeController.Bootloader_ClientMessageReceived;
        }

        public HomeController(IHubContext<Broadcaster> hubContext)
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
            if (Bootloader.OneDasController.OneDasState >= OneDasState.Ready)
            {
                OneDasPerformanceInformation performanceInformation = Bootloader.OneDasController.CreatePerformanceInformation();
                _hubContext.Clients.All.InvokeAsync("SendPerformanceInformation", performanceInformation);
            }
        }

        private static void _updateDataSnapshotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (Bootloader.OneDasController.OneDasState >= OneDasState.Ready)
            {
                dataSnapshot = Bootloader.OneDasController.CreateDataSnapshot();
                _hubContext.Clients.All.InvokeAsync("SendDataSnapshot", DateTime.UtcNow, dataSnapshot);
            }
        }

        private static void _updateLiveValueDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<object> dataSnapshot;

            if (Bootloader.OneDasController.OneDasState >= OneDasState.Ready)
            {
                foreach (var liveViewSubscription in HomeController.LiveViewSubscriptionSet)
                {
                    dataSnapshot = Bootloader.OneDasController.CreateDataSnapshot(liveViewSubscription.Value.ChannelHubSet);
                    _hubContext.Clients.Client(liveViewSubscription.Key).InvokeAsync("SendLiveViewData", liveViewSubscription.Value.SubscriptionId, DateTime.UtcNow, dataSnapshot);
                }
            }
        }

        private static void OneDasController_OneDasStateChanged(object sender, OneDasStateChangedEventArgs e)
        {
            _hubContext.Clients.All.InvokeAsync("SendOneDasState", e.NewState);
        }

        private static void Bootloader_ClientMessageReceived(object sender, SystemMessageReceivedEventArgs e)
        {
            _hubContext.Clients.All.InvokeAsync("SendClientMessage", e.Message);
        }
    }
}
