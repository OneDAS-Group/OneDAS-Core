using OneDas.Infrastructure;
using System.Runtime.Serialization;

namespace OneDas.Plugin
{
    [DataContract]
    public class DataWriterPluginSettingsBase : PluginSettingsBase
    {
        #region "Constructors

        public DataWriterPluginSettingsBase()
        {
            this.FileGranularity = FileGranularity.Day;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public FileGranularity FileGranularity { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            if (this.FileGranularity == 0)
            {
                throw new ValidationException(ErrorMessage.DataWriterPluginSettingsBase_FileGranularityInvalid);
            }
        }

        #endregion
    }
}
