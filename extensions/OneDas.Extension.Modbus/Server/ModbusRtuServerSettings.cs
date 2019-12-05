using OneDas.Extensibility;
using System;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    [DataContract]
    [ExtensionContext(typeof(ModbusTcpServerGateway))]
    [ExtensionIdentification("ModbusRtuServer", "Modbus RTU Server", "Provide data to Modbus RTU clients.", @"WebClient.Server.ModbusRtuServerView.html", @"WebClient.Server.ModbusRtuServer.js")]
    public class ModbusRtuServerSettings : ModbusServerSettings
    {
        #region "Constructors"

        public ModbusRtuServerSettings()
        {
            this.UnitIdentifier = 0x01;
            this.Port = "COM1";
        }

        #endregion

        #region "Properties"

        [DataMember]
        public byte UnitIdentifier { get; set; }

        [DataMember]
        public string Port { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            if (!(1 <= this.UnitIdentifier && this.UnitIdentifier <= 247))
                throw new Exception(ErrorMessage.ModbusRtuServerSettings_InvalidUnitIdentifier);

            if (!SerialPort.GetPortNames().Contains(this.Port))
                throw new Exception(ErrorMessage.ModbusRtuSettings_ComPortDoesNotExist);
        }

        #endregion
    }
}
