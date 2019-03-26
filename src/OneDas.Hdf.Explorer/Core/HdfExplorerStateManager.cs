using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using OneDas.Hdf.Explorer.Web;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace OneDas.Hdf.Explorer.Core
{
    public class HdfExplorerStateManager
    {
        #region "Fields"

        private bool _isActive;
        private System.Timers.Timer _activityTimer;
        private HdfExplorerOptions _options;
        private ConcurrentDictionary<string, HdfExplorerState> _stateSet;
        private ConcurrentDictionary<string, CancellationTokenSource> _ctsSet;
        private IHubContext<Broadcaster> _hubContext;

        #endregion

        #region "Constructors"

        public HdfExplorerStateManager(IHubContext<Broadcaster> hubContext, IOptions<HdfExplorerOptions> options)
        {
            _hubContext = hubContext;
            _options = options.Value;

            _stateSet = new ConcurrentDictionary<string, HdfExplorerState>();
            _ctsSet = new ConcurrentDictionary<string, CancellationTokenSource>();

            this.HandleInactivity();
        }

        #endregion

        #region "Properties"

        public int UserCount { get; private set; }

        #endregion

        #region "Methods"

        public void Register(string connectionId)
        {
            _stateSet[connectionId] = _isActive ? HdfExplorerState.Idle : HdfExplorerState.Inactive;
            _ctsSet[connectionId] = new CancellationTokenSource();

            this.UserCount += 1;
        }

        public void Unregister(string connectionId)
        {
            _stateSet.TryRemove(connectionId, out _);

            _ctsSet[connectionId].Cancel();
            _ctsSet.TryRemove(connectionId, out _);

            this.UserCount -= 1;
        }

        public void SetState(string connectionId, HdfExplorerState state)
        {
            _stateSet[connectionId] = state;
            this.GetClient(connectionId).SendAsync("SendState", state);
        }

        public HdfExplorerState GetState(string connectionId)
        {
            return _stateSet[connectionId];
        }

        public void Cancel(string connectionId)
        {
            if (_ctsSet.ContainsKey(connectionId))
            {
                _ctsSet[connectionId].Cancel();
            }

            _ctsSet[connectionId] = new CancellationTokenSource();
        }

        public CancellationToken GetToken(string connectionId)
        {
            return _ctsSet[connectionId].Token;
        }

        private void HandleInactivity()
        {
            TimeSpan startRemaining;
            TimeSpan stopRemaining;

            _isActive = true; // in case InactivityPeriod is == TimeSpan.Zero

            if (_options.InactivityPeriod > TimeSpan.Zero)
            {
                startRemaining = DateTime.UtcNow.Date.Add(_options.InactiveOn) - DateTime.UtcNow;
                stopRemaining = startRemaining.Add(_options.InactivityPeriod);

                if (startRemaining < TimeSpan.Zero)
                {
                    startRemaining = startRemaining.Add(TimeSpan.FromDays(1));
                }

                if (stopRemaining < TimeSpan.Zero)
                {
                    stopRemaining = stopRemaining.Add(TimeSpan.FromDays(1));
                }

                if (startRemaining < stopRemaining)
                {
                    _activityTimer = new System.Timers.Timer() { AutoReset = false, Enabled = true, Interval = startRemaining.TotalMilliseconds };
                    _isActive = true;
                }
                else
                {
                    _activityTimer = new System.Timers.Timer() { AutoReset = false, Enabled = true, Interval = stopRemaining.TotalMilliseconds };
                    _isActive = false;

                    _ctsSet.ToList().ForEach(entry =>
                    {
                        this.Cancel(entry.Key);
                    });
                }

                _activityTimer.Elapsed += (sender, e) => this.OnActivityTimerElapsed();
            }
        }

        private void OnActivityTimerElapsed()
        {
            this.HandleInactivity();

            if (_isActive)
            {
                _stateSet.ToList().ForEach(entry =>
                {
                    this.SetState(entry.Key, HdfExplorerState.Idle);
                });
            }
            else
            {
                _stateSet.ToList().ForEach(entry =>
                {
                    this.SetState(entry.Key, HdfExplorerState.Inactive);
                });
            }
        }

        private IClientProxy GetClient(string connectionId)
        {
            return _hubContext.Clients.Client(connectionId);
        }

        #endregion
    }
}
