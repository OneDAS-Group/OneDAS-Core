using Microsoft.AspNetCore.SignalR;
using OneDas.Engine.Core;
using System;
using System.Threading.Tasks;

namespace OneDas.WebServer.Web
{
    public class ConsoleHub : Hub
    {
        public Task<OneDasPerformanceInformation> GetPerformanceInformation()
        {
            return Task.Run(() =>
            {
                return Bootloader.OneDasController.OneDasEngine.CreatePerformanceInformation();
            });
        }

        public Task ToggleDebugOutput()
        {
            return Task.Run(() =>
            {
                Bootloader.OneDasController.OneDasEngine.IsDebugOutputEnabled = !Bootloader.OneDasController.OneDasEngine.IsDebugOutputEnabled;
            });
        }

        public Task Shutdown(bool restart)
        {
            return Task.Run(() =>
            {
                Bootloader.Shutdown(restart, 0);
            });
        }
    }
}
