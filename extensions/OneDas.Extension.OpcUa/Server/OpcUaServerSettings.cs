using OneDas.Extensibility;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace OneDas.Extension.OpcUa
{
    [DataContract]
    [ExtensionContext(typeof(OpcUaServerGateway))]
    [ExtensionIdentification("OpcUaServer", "OPC-UA Server", "Provide data to OPC-UA clients.", @"WebClient.Server.OpcUaServerView.html", @"WebClient.Server.OpcUaServer.js")]
    public class OpcUaServerSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public OpcUaServerSettings()
        {
            //
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string LocalIpAddress { get; set; }

        [DataMember]
        public int FrameRateDivider { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            IPAddress localIpAddress;

            base.Validate();

            if (!OneDasUtilities.ValidateIPv4(this.LocalIpAddress, out localIpAddress))
                throw new Exception(ErrorMessage.OpcUaServerSettings_LocalIpAddressInvalid);
        }

        #endregion
    }
}
