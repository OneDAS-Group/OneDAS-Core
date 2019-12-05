using OneDas.Extensibility;
using System;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    [DataContract]
    [ExtensionContext(typeof(ModbusRtuClientGateway))]
    [ExtensionIdentification("ModbusRtuClient", "Modbus RTU Client", "Exchange data with a Modbus RTU server.", @"WebClient.Client.ModbusRtuClientView.html", @"WebClient.Client.ModbusRtuClient.js")]
    public class ModbusRtuClientSettings : ModbusClientSettings
    {
        #region "Constructors"

        public ModbusRtuClientSettings()
        {
            this.Port = "COM1";
            this.UnitIdentifier = 0x01;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string Port { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            if (!SerialPort.GetPortNames().Contains(this.Port))
                throw new Exception(ErrorMessage.ModbusRtuSettings_ComPortDoesNotExist);

            if (!(0 <= this.UnitIdentifier && this.UnitIdentifier <= 247))
                throw new Exception(ErrorMessage.ModbusRtuClientSettings_InvalidUnitIdentifier);
        }

        #endregion
    }
}
