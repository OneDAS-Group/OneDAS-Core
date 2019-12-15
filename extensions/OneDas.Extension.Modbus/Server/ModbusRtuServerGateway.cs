using FluentModbus;
using Microsoft.Extensions.Logging;

namespace OneDas.Extension.Modbus
{
    public class ModbusRtuServerGateway : ModbusServerGateway
    {
        #region Constructors

        public ModbusRtuServerGateway(ModbusRtuServerSettings settings, ILogger logger) 
            : base(new ModbusRtuServer(settings.UnitIdentifier, isAsynchronous: false), settings, logger)
        {
            var modbusServer = (ModbusRtuServer)this.ModbusServer;

            modbusServer.BaudRate = settings.BaudRate;
            modbusServer.Handshake = settings.Handshake;
            modbusServer.Parity = settings.Parity;
            modbusServer.StopBits = settings.StopBits;
        }

        #endregion

        #region Methods

        protected override void OnConfigure()
        {
            base.OnConfigure();

            var settings = (ModbusRtuServerSettings)this.Settings;
            var server = (ModbusRtuServer)this.ModbusServer;

            server.Start(settings.Port);
            this.Logger.LogInformation($"COM port '{settings.Port}' opened");
        }

        #endregion
    }
}
