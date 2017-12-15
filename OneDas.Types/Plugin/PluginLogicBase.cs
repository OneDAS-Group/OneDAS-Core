using System;
using System.Diagnostics.Contracts;

namespace OneDas.Plugin
{
    public abstract class PluginLogicBase : IDisposable 
    {
        public event EventHandler<SendReportEventArgs> SendReport;

        public PluginLogicBase(PluginSettingsBase settings)
        {
            Contract.Requires(settings != null);

            this.Settings = settings;
        }

        public PluginSettingsBase Settings { get; private set; }

        protected virtual void OnSendReport(string message)
        {
            this.SendReport?.Invoke(this, new SendReportEventArgs($"{ this.Settings.Description.Id } ({ this.Settings.Description.InstanceId })", message));
        }

        protected virtual void OnSendReport(object sender, SendReportEventArgs e)
        {
            this.SendReport?.Invoke(sender, e);
        }

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
