using Microsoft.Extensions.Logging;
using OneDas.Engine.Core;
using OneDas.WebServer.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace OneDas.WebServer.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, Namespace = "http://onedas.com")]
    public partial class OneDasController : IManagementService, IDisposable
    {
        #region "Fields"

        private ServiceHost _serviceHost_ManagementService;
        private ServiceEndpoint _serviceEndpoint_ManagementService;

        #endregion

        #region "Constructors"

        public OneDasController(ILoggerFactory _loggerFactory)
        {
            this.OneDasEngine = new OneDasEngine(_loggerFactory);
        }

        #endregion

        #region "IManagementService"

        OneDasPerformanceInformation IManagementService.CreatePerformanceInformation()
        {
            return this.OneDasEngine.CreatePerformanceInformation();
        }

        void IManagementService.BoostProcessPriority()
        {
            Bootloader.BoostProcessPriority();
        }

        void IManagementService.ToggleDebugOutput()
        {
            this.OneDasEngine.IsDebugOutputEnabled = !this.OneDasEngine.IsDebugOutputEnabled;
        }

        void IManagementService.Shutdown(bool restart)
        {
            Bootloader.Shutdown(restart, 0);
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
            IList<Task> taskSet;

            if (!isDisposed)
            {
                if (disposing)
                {
                    // OneDasEngine
                    OneDasEngine?.Dispose();

                    // WCF
                    taskSet = new List<Task>();

                    taskSet.Add(Task.Run(() =>
                    {
                        // ManagementService (close)
                        if (_serviceHost_ManagementService.State != CommunicationState.Closed)
                        {
                            try
                            {
                                _serviceHost_ManagementService.Close(TimeSpan.FromSeconds(2));
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
                    }));

                    Task.WaitAll(taskSet.ToArray());
                }
            }

            isDisposed = true;
        }

        #endregion
    }
}