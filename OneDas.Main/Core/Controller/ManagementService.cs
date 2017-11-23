using OneDas.Main.Shell;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace OneDas.Main.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, Namespace = "http://onedas.com")]
    public partial class OneDasController : IManagementService
    {
#region "Fields"

        private ServiceHost _serviceHost_ManagementService;
        private ServiceEndpoint _serviceEndpoint_ManagementService;

#endregion

#region "IManagementService"

        OneDasPerformanceInformation IManagementService.CreatePerformanceInformation()
        {
            return this.CreatePerformanceInformation();
        }

        void IManagementService.BoostProcessPriority()
        {
            Bootloader.BoostProcessPriority();
        }

        void IManagementService.ToggleDebugOutput()
        {
            _isDebugOutputEnabled = !_isDebugOutputEnabled;
        }

        void IManagementService.Shutdown(bool restart)
        {
            Bootloader.Shutdown(restart, 0);
        }

#endregion
    }
}