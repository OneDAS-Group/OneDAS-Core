using System.ServiceModel;
using OneDas.Main.Core;

namespace OneDas.Main.Shell
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