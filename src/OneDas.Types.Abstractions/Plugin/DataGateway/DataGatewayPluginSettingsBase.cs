using OneDas.Infrastructure;
using System;
using System.Runtime.Serialization;

namespace OneDas.Plugin
{
    [DataContract]
    public abstract class DataGatewayPluginSettingsBase : PluginSettingsBase
    {
        #region "Constructors

        public DataGatewayPluginSettingsBase()
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
            //if (this.FrameRateDivider < 1 || FrameRateDivider > oneDasOptions.NativeSampleRate)
            //{
            //    throw new Exception(ErrorMessage.UdpModel_FrameRateDividerInvalid);
            //}

            if (this.MaximumDatasetAge < 0 || this.MaximumDatasetAge > 10000)
            {
                throw new Exception(ErrorMessage.DataReaderPluginSettingsBase_MaximumDatasetAgeInvalid);
            }
        }

        #endregion
    }
}
