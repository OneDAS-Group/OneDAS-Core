using OneDas.Extensibility;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    [DataContract]
    [ExtensionContext(typeof(ModbusTcpClientGateway))]
    [ExtensionIdentification("ModbusTcpClient", "Modbus TCP Client", "Exchange data with a Modbus TCP server.", @"WebClient.TcpClient.ModbusTcpClientView.html", @"WebClient.TcpClient.ModbusTcpClient.js")]
    public class ModbusTcpClientSettings : ModbusClientSettings
    {
        #region "Constructors"

        public ModbusTcpClientSettings()
        {
            this.Port = 502;
            this.UnitIdentifier = 0xFF;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string RemoteIpAddress { get; set; }

        [DataMember]
        public UInt16 Port { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            IPAddress remoteIpAddress;

            base.Validate();

            if (!OneDasUtilities.ValidateIPv4(this.RemoteIpAddress, out remoteIpAddress))
                throw new Exception(ErrorMessage.ModbusTcpClientSettings_RemoteIpAddressInvalid);

            if (this.Port == 0)
                throw new Exception(ErrorMessage.ModbusTcpClientSettings_PortInvalid);
        }

        #endregion
    }
}
