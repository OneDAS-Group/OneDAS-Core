using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.CanRaw
{
    [DataContract]
    [ExtensionContext(typeof(CanRawGateway))]
    [ExtensionIdentification("CanRaw", "CAN-Raw", "Send and receive data via CAN bus", @"WebClient.CanRawView.html", @"WebClient.CanRaw.js")]
    public class CanRawSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public CanRawSettings()
        {
            this.CanDriver = CanDriver.Ixxat;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public CanDriver CanDriver { get; set; }

        #endregion
    }
}
