using OneDas.Extensibility;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace OneDas.Extension.ModbusTcp
{
    [DataContract]
    [ExtensionContext(typeof(ModbusTcpServerGateway))]
    [ExtensionIdentification("ModbusTcpServer", "Modbus TCP Server", "Provide data to Modbus TCP clients.", @"WebClient.Server.ModbusTcpServerView.html", @"WebClient.Server.ModbusTcpServer.js")]
    public class ModbusTcpServerSettings : ExtendedDataGatewayExtensionSettingsBase
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

        [DataMember]
        public int FrameRateDivider { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            IPAddress localIpAddress;

            base.Validate();

            if (!OneDasUtilities.ValidateIPv4(this.LocalIpAddress, out localIpAddress))
            {
                throw new Exception(ErrorMessage.ModbusTcpServerSettings_LocalIpAddressInvalid);
            }

            if (this.Port == 0)
            {
                throw new Exception(ErrorMessage.ModbusTcpServerSettings_PortInvalid);
            }
        }

        #endregion
    }
}
