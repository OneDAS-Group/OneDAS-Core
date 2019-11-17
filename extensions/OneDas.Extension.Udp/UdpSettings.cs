using OneDas.Extensibility;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace OneDas.Extension.Udp
{
    [DataContract]
    [ExtensionContext(typeof(UdpGateway))]
    [ExtensionIdentification("Udp", "User datagram protocol", "Exchange data via UDP frames.", @"WebClient.UdpView.html", @"WebClient.Udp.js")]
    public class UdpSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public UdpSettings()
        {
            this.FrameRateDivider = 1;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string RemoteIpAddress { get; set; }

        [DataMember]
        public UInt16 LocalDataPort { get; set; }

        [DataMember]
        public UInt16 RemoteDataPort { get; set; }

        [DataMember]
        public int FrameRateDivider { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            IPAddress remoteIpAddress;

            base.Validate();

            if (!OneDasUtilities.ValidateIPv4(this.RemoteIpAddress, out remoteIpAddress))
            {
                throw new Exception(ErrorMessage.UdpSettings_RemoteIpAddressInvalid);
            }

            if (this.RemoteDataPort < 1024 || this.RemoteDataPort > 65535)
            {
                throw new Exception(ErrorMessage.UdpSettings_DataPortInvalid);
            }

            if (this.LocalDataPort < 1024 || this.LocalDataPort > 65535)
            {
                throw new Exception(ErrorMessage.UdpSettings_DataPortInvalid);
            }
        }

        #endregion
    }
}
