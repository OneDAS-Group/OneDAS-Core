using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.Contracts;

namespace OneDas.Plugin
{
    public abstract class PluginLogicBase : IDisposable 
    {
        public PluginLogicBase(PluginSettingsBase settings, ILoggerFactory loggerFactory)
        {
            Contract.Requires(settings != null);
            Contract.Requires(loggerFactory != null);

            this.DefaultLogger = loggerFactory.CreateLogger($"{ settings.Description.Id } ({ settings.Description.InstanceId })");
            this.Settings = settings;
        }

        public ILogger DefaultLogger { get; }
        public PluginSettingsBase Settings { get; }

        // don't force inherited classes to overwrite this
        protected virtual void FreeManagedResources()
        {
            //
        }

        protected virtual void FreeUnmanagedResources()
        {
            //
        }

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

        ~PluginLogicBase()
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
