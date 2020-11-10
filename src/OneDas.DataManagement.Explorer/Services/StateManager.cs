using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Explorer.Core;
using Prism.Mvvm;
using System;

namespace OneDas.DataManagement.Explorer.Services
{
    public class StateManager : BindableBase
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerUserManager _userManager;
        private OneDasExplorerOptions _options;
        private bool _state;

        #endregion

        #region Constructors

        public StateManager(OneDasDatabaseManager databaseManager,
                            OneDasExplorerUserManager userManager,
                            OneDasExplorerOptions options,
                            ILoggerFactory loggerFactory)
        {
            _databaseManager = databaseManager;
            _userManager = userManager;
            _options = options;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");

            this.TryRunApp(out var _);
        }

        #endregion

        #region Properties

        public bool IsInitialized
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
                _databaseManager.Update();
                _userManager.Initialize();
                _options.Save(Program.OptionsFilePath);
                exception = null;
                this.IsInitialized = true;
            }
            catch (Exception ex)
            {
                this.IsInitialized = false;
                exception = ex;
            }

            return this.IsInitialized;
        }

        #endregion
    }
}
