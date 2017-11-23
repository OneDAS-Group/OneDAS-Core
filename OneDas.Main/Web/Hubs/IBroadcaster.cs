using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OneDas.Infrastructure;
using OneDas.Main.Core;

namespace OneDas.Main.Web
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
