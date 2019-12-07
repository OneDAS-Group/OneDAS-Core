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
            // defined in base class
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

            if (!(0 <= this.UnitIdentifier && this.UnitIdentifier <= 247))
                throw new Exception(ErrorMessage.ModbusRtuClientSettings_InvalidUnitIdentifier);

            if (!SerialPort.GetPortNames().Contains(this.Port))
                throw new Exception(ErrorMessage.ModbusRtuSettings_ComPortDoesNotExist);


        }

        #endregion
    }
}
