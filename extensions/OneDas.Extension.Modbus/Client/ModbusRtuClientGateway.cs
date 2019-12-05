using FluentModbus;
using Microsoft.Extensions.Logging;
using System;

namespace OneDas.Extension.Modbus
{
    public class ModbusRtuClientGateway : ModbusClientGateway
    {
        #region "Constructors"

        public ModbusRtuClientGateway(ModbusRtuClientSettings settings, ILoggerFactory loggerFactory) 
            : base(new ModbusRtuClient(), settings, loggerFactory)
        {
            //
        }

        #endregion

        #region "Methods"

        protected override void Connect()
        {
            var settings = (ModbusRtuClientSettings)this.Settings;
            var modbusClient = (ModbusRtuClient)this.ModbusClient;

            if (!modbusClient.IsConnected)
            {
                modbusClient.Connect(settings.Port);
                this.Logger.LogInformation($"COM port '{settings.Port}' opened");
            }
        }

        protected override void Reconnect()
        {
            var settings = (ModbusRtuClientSettings)this.Settings;
            var modbusClient = (ModbusRtuClient)this.ModbusClient;

            this.Logger.LogWarning($"COM port connection or Modbus RTU communication failure");
            this.CTS.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            try
            {
                modbusClient.Connect(settings.Port);
                this.Logger.LogInformation($"COM port connection re-established");
            }
            catch
            {
                this.Logger.LogWarning($"COM port connection recovery failed");
                this.CTS.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
            }
        }

        protected override void Disconnect()
        {
            var modbusClient = (ModbusRtuClient)this.ModbusClient;
            modbusClient.Close();
        }

        #endregion
    }
}
