using Microsoft.Extensions.Logging;
using System;

namespace OneDas.Extensibility
{
    public abstract class ExtensionLogicBase : IDisposable 
    {
        #region Constructors

        public ExtensionLogicBase(ExtensionSettingsBase settings, ILogger logger)
        {
            this.Settings = settings;
            this.Settings.Validate();

            this.Logger = logger;

            this.DisplayName = $"{ settings.Description.Id } ({ settings.Description.InstanceId })";
        }

        #endregion

        #region Properties

        public string DisplayName { get; }

        public ExtensionSettingsBase Settings { get; }

        protected ILogger Logger { get; }

        #endregion

        #region Methods

        // don't force inherited classes to overwrite this
        protected virtual void FreeManagedResources()
        {
            //
        }

        protected virtual void FreeUnmanagedResources()
        {
            //
        }

        #endregion

        #region IDisposable Support

        private bool hasDisposed = false;

        protected void Dispose(bool disposing)
        {
            if (!hasDisposed)
            {
                if (disposing)
                {
                    this.FreeManagedResources();
                }

                this.FreeUnmanagedResources();

                hasDisposed = true;
            }
        }

        ~ExtensionLogicBase()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
