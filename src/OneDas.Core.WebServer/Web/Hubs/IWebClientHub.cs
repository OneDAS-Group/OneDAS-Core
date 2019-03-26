using OneDas.Core.Engine;
using OneDas.Core.ProjectManagement;
using OneDas.Extensibility;
using OneDas.PackageManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public interface IWebClientHub
    {
        Task SendWebServerOptionsLight(WebServerOptionsLight webServerOptionsLight);
        Task SendOneDasState(OneDasState oneDasState);
        Task SendActiveProject(OneDasProjectSettings projectSettings);
        Task SendPerformanceInformation(OneDasPerformanceInformation performanceInformation);
        Task SendDataSnapshot(DateTime dateTime, IEnumerable<object> dataSnapshot);
        Task SendLiveViewData(int subscriptionId, DateTime dateTime, IEnumerable<object> dataSnapshot);
        Task SendClientMessage(string message);
        Task SendNugetMessage(string message);
        Task SendInstalledPackages(List<OneDasPackageMetaData> packageMetadata);
        Task SendExtensionIdentifications(List<ExtensionIdentificationAttribute> dataGatewayExtensionIdentificationSet, List<ExtensionIdentificationAttribute> dataWriterExtensionIdentificationSet);
    }
}
