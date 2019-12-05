using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    public abstract class ModbusServerSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        #region Properties

        [DataMember]
        public int FrameRateDivider { get; set; }

        #endregion
    }
}
