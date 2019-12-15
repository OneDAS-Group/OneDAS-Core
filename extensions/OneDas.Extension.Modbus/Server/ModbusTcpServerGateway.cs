using FluentModbus;
using Microsoft.Extensions.Logging;
using System.Net;

namespace OneDas.Extension.Modbus
{
    public class ModbusTcpServerGateway : ModbusServerGateway
    {
        #region "Fields"

        private IPEndPoint _localEndpoint;

        #endregion

        #region "Constructors"

        public ModbusTcpServerGateway(ModbusTcpServerSettings settings, ILogger logger) 
            : base(new ModbusTcpServer(isAsynchronous: false), settings, logger)
        {
            _localEndpoint = new IPEndPoint(IPAddress.Parse(settings.LocalIpAddress), (int)settings.Port);
        }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            base.OnConfigure();

            var settings = (ModbusTcpServerSettings)this.Settings;
            var modbusServer = (ModbusTcpServer)this.ModbusServer;

            modbusServer.Start(_localEndpoint);
            this.Logger.LogInformation($"Listening on TCP end point '{settings.LocalIpAddress}:{settings.Port}'.");
        }

        #endregion
    }
}
