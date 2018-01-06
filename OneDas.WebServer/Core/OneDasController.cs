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
        #region "Fields"

        private ILogger _systemLogger;

        #endregion

        #region "Constructors"

        public OneDasController(ILoggerFactory _loggerFactory)
        {
            _systemLogger = _loggerFactory.CreateLogger("System");

            this.OneDasEngine = new OneDasEngine(ConfigurationManager<OneDasSettings>.Settings.BaseDirectoryPath, _loggerFactory);

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

            _systemLogger.LogInformation("management service started");
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