using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;

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
            Bootloader.WriteLogEntry("started in non-interactive mode (service)", EventLogEntryType.Information);
        }

        protected override void OnStopping()
        {
            base.OnStopping();
            Bootloader.Dispose();
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            Bootloader.Dispose();
        }
    }
}