using FluentModbus;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace OneDas.Extension.Modbus
{
    public class ModbusTcpClientGateway : ModbusClientGateway
    {
        #region "Fields"

        private IPEndPoint _remoteEndpoint;

        #endregion

        #region "Constructors"

        public ModbusTcpClientGateway(ModbusTcpClientSettings settings, ILogger logger) 
            : base(new ModbusTcpClient(), settings, logger)
        {
            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(settings.RemoteIpAddress), (int)settings.Port);
        }

        #endregion

        #region "Methods"

        protected override void Connect()
        {
            var modbusClient = (ModbusTcpClient)this.ModbusClient;

            if (!modbusClient.IsConnected)
            {
                modbusClient.Connect(_remoteEndpoint);
                this.Logger.LogInformation($"TCP connection established");
            }
        }

        protected override void Reconnect()
        {
            var modbusClient = (ModbusTcpClient)this.ModbusClient;

            this.Logger.LogWarning($"TCP connection or Modbus TCP communication failure");
            this.CTS.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            try
            {
                modbusClient.Connect(_remoteEndpoint);
                this.Logger.LogInformation($"TCP connection re-established");
            }
            catch
            {
                this.Logger.LogWarning($"TCP connection recovery failed");
                this.CTS.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
            }
        }

        protected override void Disconnect()
        {
            var modbusClient = (ModbusTcpClient)this.ModbusClient;
            modbusClient.Disconnect();
        }

        #endregion
    }
}
