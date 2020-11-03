using Microsoft.Extensions.Logging;
using OneDas.Types;
using Prism.Mvvm;
using System;

namespace OneDas.DataManagement.Explorer.Core
{
    public class StateManager : BindableBase
    {
        #region Fields

        private bool _isFollowUp;
        private bool _isActive;
        private ILogger _logger;
        private System.Timers.Timer _activityTimer;
        private JobService _jobService;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerUserManager _userManager;
        private OneDasExplorerOptions _options;
        private OneDasExplorerState _state;
        private Aggregator _aggregator;

        #endregion

        #region Constructors

        public StateManager(JobService jobService,
                            OneDasDatabaseManager databaseManager,
                            OneDasExplorerUserManager userManager,
                            OneDasExplorerOptions options,
                            ILoggerFactory loggerFactory)
        {
            _jobService = jobService;
            _databaseManager = databaseManager;
            _userManager = userManager;
            _options = options;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
            _aggregator = new Aggregator(_options.AggregationChunkSizeMB, _logger, loggerFactory);

            this.TryRunApp(out var _);
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

        public bool TryRunApp(out Exception exception)
        {
            try
            {
                _databaseManager.Initialize(_options.DataBaseFolderPath);
                _userManager.Initialize();
                _options.Save(Program.OptionsFilePath);
                this.OnActivityTimerElapsed();
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                this.State = OneDasExplorerState.FirstStart;
                exception = ex;
                return false;
            }                
        }

        public void CheckState()
        {
            switch (this.State)
            {
                case OneDasExplorerState.FirstStart:
                    throw new Exception("OneDAS Explorer is not fully configured yet.");

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

                    if (_isFollowUp)
                    {
                        _aggregator.Run(_options.DataBaseFolderPath, _options.AggregationPeriodDays, false);
                        _databaseManager.Update();
                    }
                    else
                    {
                        _isFollowUp = true;
                    }

                    _logger.LogInformation($"{message} Done.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{message} Error: {ex.GetFullMessage()}");
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
                _jobService.Reset();
                _logger.LogInformation($"Entered inactive mode.");
            }
        }

        #endregion
    }
}
