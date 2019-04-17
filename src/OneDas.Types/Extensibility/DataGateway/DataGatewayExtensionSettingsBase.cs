using System;
using System.Runtime.Serialization;

namespace OneDas.Extensibility
{
    [DataContract]
    public abstract class DataGatewayExtensionSettingsBase : ExtensionSettingsBase
    {
        #region "Constructors

        public DataGatewayExtensionSettingsBase()
        {
            //
        }

        #endregion

        #region "Properties"

        [DataMember]
        public int MaximumDatasetAge { get; private set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            // IMPROVE: implement general frame rate divider setting
            //if (this.FrameRateDivider < 1 || this.FrameRateDivider > oneDasOptions.NativeSampleRate)
            //{
            //    throw new Exception(ErrorMessage.UdpModel_FrameRateDividerInvalid);
            //}

            if (this.MaximumDatasetAge < 0 || this.MaximumDatasetAge > 10000)
            {
                throw new Exception(ErrorMessage.DataReaderExtensionSettingsBase_MaximumDatasetAgeInvalid);
            }
        }

        #endregion
    }
}
