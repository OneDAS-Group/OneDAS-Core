using OneDas.Engine.Core;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.WebServer.Nuget;
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
        Task SendPluginIdentifications(List<PluginIdentificationAttribute> dataGatewayPluginIdentificationSet, List<PluginIdentificationAttribute> dataWriterPluginIdentificationSet);
    }
}
