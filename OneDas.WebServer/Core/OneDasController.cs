using Microsoft.Extensions.Logging;
using OneDas.Engine;
using OneDas.Engine.Core;
using OneDas.Engine.Serialization;
using OneDas.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OneDas.WebServer.Core
{
    public partial class OneDasController : IDisposable
    {
        #region "Constructors"

        public OneDasController(ILoggerFactory loggerFactory)
        {
            this.OneDasEngine = new OneDasEngine(ConfigurationManager<OneDasSettings>.Settings.BaseDirectoryPath, loggerFactory);

            if (ConfigurationManager<OneDasSettings>.Settings.IsAutostartEnabled && File.Exists(ConfigurationManager<OneDasSettings>.Settings.CurrentProjectFilePath))
            {
                Task.Run(() =>
                {
                    try
                    {
                        this.OneDasEngine.ActivateProject(ProjectSerializationHelper.Load(ConfigurationManager<OneDasSettings>.Settings.CurrentProjectFilePath), 4);
                        this.OneDasEngine.OneDasState = OneDasState.Run;
                    }
                    catch (Exception ex)
                    {
                        this.OneDasEngine.HandleException(ex);
                    }
                });
            }
        }

        #endregion

        #region "Properties"

        public OneDasEngine OneDasEngine { get; }

        #endregion

        #region "IDisposable Support"

        private bool isDisposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    this.OneDasEngine?.Dispose();
                }
            }

            isDisposed = true;
        }

        #endregion
    }
}