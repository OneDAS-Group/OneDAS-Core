using OneDas.Extensibility;
using System;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    [DataContract]
    [ExtensionContext(typeof(ModbusTcpServerGateway))]
    [ExtensionIdentification("ModbusTcpServer", "Modbus TCP Server", "Provide data to Modbus TCP clients.", @"WebClient.Server.ModbusTcpServerView.html", @"WebClient.Server.ModbusTcpServer.js")]
    public class ModbusTcpServerSettings : ModbusServerSettings
    {
        #region "Constructors"

        public ModbusTcpServerSettings()
        {
            this.Port = 502;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string LocalIpAddress { get; set; }

        [DataMember]
        public UInt16 Port { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            if (!OneDasUtilities.ValidateIPv4(this.LocalIpAddress, out var _))
                throw new Exception(ErrorMessage.ModbusTcpServerSettings_LocalIpAddressInvalid);

            if (this.Port == 0)
                throw new Exception(ErrorMessage.ModbusTcpServerSettings_PortInvalid);
        }

        #endregion
    }
}
