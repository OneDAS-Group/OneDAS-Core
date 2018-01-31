using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;

namespace OneDas.WebServer.Shell
{
    public class OneDasService : WebHostService
    {
        public OneDasService(string serviceName, IWebHost webHost) : base(webHost)
        {
            this.ServiceName = serviceName;
        }

        protected override void OnStarting(string[] args)
        {
            base.OnStarting(args);
            BasicBootloader.SystemLogger.LogInformation("started in non-interactive mode (service)");
        }

        protected override void OnStopping()
        {
            base.OnStopping();
            BasicBootloader.Shutdown();
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            BasicBootloader.Shutdown();
        }
    }
}