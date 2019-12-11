using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Can
{
    [DataContract]
    [ExtensionContext(typeof(CanGateway))]
    [ExtensionSupporter(typeof(CanSupporter))]
    [ExtensionIdentification("CAN 2.0a/b", "CAN 2.0a/b", "Send and receive CAN 2.0a and 2.0b messages.", @"WebClient.CanView.html", @"WebClient.Can.js")]
    public class CanSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public CanSettings()
        {
            this.CanDriverType = CanDriverType.IxxatUsbToCanV2Compact;
            this.HardwareId = string.Empty;
            this.BitRate = CiaBitRate.Cia125KBit;
            this.BusCoupling = BusCoupling.Highspeed;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public CanDriverType CanDriverType { get; set; }

        [DataMember]
        public string HardwareId { get; set; }

        [DataMember]
        public CiaBitRate BitRate { get; set; }

        [DataMember]
        public BusCoupling BusCoupling { get; set; }

        [DataMember]
        public int FrameRateDivider { get; set; }

        #endregion
    }
}
