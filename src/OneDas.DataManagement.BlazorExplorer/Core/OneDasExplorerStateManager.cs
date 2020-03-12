using Microsoft.Extensions.Options;
using Prism.Mvvm;
using System;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
#warning: stop all running downloads when state changes to inactive

    public class OneDasExplorerStateManager : BindableBase
    {
        #region Fields

        private bool _isActive;
        private System.Timers.Timer _activityTimer;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerOptions _options;
        private OneDasExplorerState _state;

        #endregion

        #region Constructors

        public OneDasExplorerStateManager(OneDasDatabaseManager databaseManager, IOptions<OneDasExplorerOptions> options)
        {
            _databaseManager = databaseManager;
            _options = options.Value;

            this.OnActivityTimerElapsed();
        }

        #endregion

        #region Properties

        public OneDasExplorerState State
        {
            get { return _state; }
            private set { this.SetProperty(ref _state, value); }
        }

        #endregion

        #region Methods

        public void CheckState()
        {
            switch (this.State)
            {
                case OneDasExplorerState.Inactive:
                    throw new Exception("HDF Explorer is in scheduled inactivity mode.");

                default:
                    break;
            }
        }

        private void HandleInactivity()
        {
            _isActive = true; // in case InactivityPeriod is == TimeSpan.Zero

            if (_options.InactivityPeriod > TimeSpan.Zero)
            {
                var startRemaining = DateTime.UtcNow.Date.Add(_options.InactiveOn) - DateTime.UtcNow;
                var stopRemaining = startRemaining.Add(_options.InactivityPeriod);

                if (startRemaining < TimeSpan.Zero)
                    startRemaining = startRemaining.Add(TimeSpan.FromDays(1));

                if (stopRemaining < TimeSpan.Zero)
                    stopRemaining = stopRemaining.Add(TimeSpan.FromDays(1));

                if (startRemaining < stopRemaining)
                {
                    _activityTimer = new System.Timers.Timer() { AutoReset = false, Enabled = true, Interval = startRemaining.TotalMilliseconds };
                    _isActive = true;
                }
                else
                {
                    _activityTimer = new System.Timers.Timer() { AutoReset = false, Enabled = true, Interval = stopRemaining.TotalMilliseconds };
                    _isActive = false;
                }

                _activityTimer.Elapsed += (sender, e) => this.OnActivityTimerElapsed();
            }
        }

        private void OnActivityTimerElapsed()
        {
            this.HandleInactivity();

            if (_isActive)
            {
                _databaseManager.Update();
                this.State = OneDasExplorerState.Ready;
            }
            else
            {
                this.State = OneDasExplorerState.Inactive;
            }
        }

        #endregion
    }
}
