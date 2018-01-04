using OneDas.Engine.Core;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public interface IBroadcaster
    {
        Task SendSlimOneDasSettings(SlimOneDasSettings slimOneDasSettings);
        Task SendOneDasState(OneDasState oneDasState);
        Task SendActiveProject(Project project);
        Task SendPerformanceInformation(OneDasPerformanceInformation performanceInformation);
        Task SendDataSnapshot(DateTime dateTime, IEnumerable<object> dataSnapshot);
        Task SendLiveViewData(int subscriptionId, DateTime dateTime, IEnumerable<object> dataSnapshot);
        Task SendMessage(string message);
    }
}
