using System.ServiceModel;
using OneDas.WebServer.Core;

namespace OneDas.WebServer.Shell
{
    [ServiceContract]
    public interface IManagementService
    {
        [OperationContract(IsOneWay = false)]
        OneDasPerformanceInformation CreatePerformanceInformation();

        [OperationContract(IsOneWay = false)]
        void BoostProcessPriority();

        [OperationContract(IsOneWay = false)]
        void ToggleDebugOutput();

        [OperationContract(IsOneWay = false)]
        void Shutdown(bool restart);
    }
}