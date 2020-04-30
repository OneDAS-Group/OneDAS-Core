using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prism.Mvvm;
using System;

namespace OneDas.DataManagement.Explorer.Core
{
#warning: stop all running downloads when state changes to inactive

    public class OneDasExplorerStateManager : BindableBase
    {
        #region Fields

        private bool _isActive;
        private ILogger _logger;
        private System.Timers.Timer _activityTimer;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerOptions _options;
        private OneDasExplorerState _state;

        #endregion

        #region Constructors

        public OneDasExplorerStateManager(OneDasDatabaseManager databaseManager,
                                          ILoggerFactory loggerFactory,
                                          IOptions<OneDasExplorerOptions> options)
        {
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
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
                    throw new Exception("OneDAS Explorer is in scheduled inactivity mode.");

                default:
                    break;
            }
        }

        private void HandleInactivity()
        {
            _isActive = true; // in case InactivityPeriod is == TimeSpan.Zero

            if (_options.InactivityPeriod > TimeSpan.Zero)
            {
                var now = DateTime.Now;
                var startRemaining = now.Date.Add(_options.InactiveOn) - now;
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
                var message = "Updating database ...";

                try
                {
                    _logger.LogInformation(message);
                    _databaseManager.Update();
                    _logger.LogInformation($"{message} Done.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{message} Error: {ex.Message}");
                }
                finally
                {
                    this.State = OneDasExplorerState.Ready;
                    _logger.LogInformation($"Entered active mode.");
                }
            }
            else
            {
                this.State = OneDasExplorerState.Inactive;
                _logger.LogInformation($"Entered inactive mode.");
            }
        }

        #endregion
    }
}
