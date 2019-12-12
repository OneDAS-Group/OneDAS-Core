using OneDas.Extensibility;
using System;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    [DataContract]
    [ExtensionContext(typeof(ModbusTcpServerGateway))]
    [ExtensionIdentification("ModbusRtuServer", "Modbus RTU Server", "Provide data to Modbus RTU clients.", @"WebClient.RtuServer.ModbusRtuServerView.html", @"WebClient.RtuServer.ModbusRtuServer.js")]
    public class ModbusRtuServerSettings : ModbusServerSettings
    {
        #region "Constructors"

        public ModbusRtuServerSettings()
        {
            this.UnitIdentifier = 0x01;
            this.Port = "COM1";
            this.BaudRate = 9600;
            this.Handshake = Handshake.None;
            this.Parity = Parity.Even;
            this.StopBits = StopBits.One;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public byte UnitIdentifier { get; set; }

        [DataMember]
        public string Port { get; set; }

        [DataMember]
        public int BaudRate { get; set; }

        [DataMember]
        public Handshake Handshake { get; set; }

        [DataMember]
        public Parity Parity { get; set; }

        [DataMember]
        public StopBits StopBits { get; set; }

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
