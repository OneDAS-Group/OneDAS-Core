using Microsoft.AspNetCore.SignalR;
using OneDas.Engine.Core;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public class ConsoleHub : Hub
    {
        OneDasEngine _oneDasEngine;

        public ConsoleHub(OneDasEngine oneDasEngine)
        {
            _oneDasEngine = oneDasEngine;
        }

        public Task<OneDasPerformanceInformation> GetPerformanceInformation()
        {
            return Task.Run(() =>
            {
                return _oneDasEngine.CreatePerformanceInformation();
            });
        }
    }
}
