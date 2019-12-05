using OneDas.Extensibility;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    public abstract class ModbusClientSettings : ExtendedDataGatewayExtensionSettingsBase
    {
        #region "Constructors"

        public ModbusClientSettings()
        {
            // unit identifier must be initialized in derived classes due to different rules
        }

        #endregion

        #region "Properties"

        [DataMember]
        public byte UnitIdentifier { get; set; }

        [DataMember]
        public int FrameRateDivider { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            // unit identifier must be validated in derived classes due to different rules
        }

        #endregion
    }
}
