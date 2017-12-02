using OneDas.Infrastructure;
using System.Collections.Generic;
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

        public abstract IEnumerable<DataPort> GetDataPortSet();

        public override void Validate()
        {
            base.Validate();

            if (this.MaximumDatasetAge < 0 || this.MaximumDatasetAge > 10000)
            {
                throw new ValidationException(ErrorMessage.DataReaderPluginSettingsBase_MaximumDatasetAgeInvalid);
            }
        }

        #endregion
    }
}
