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

        public DataWriterPluginSettingsBase(FileGranularity fileGranularity)
        {
            // Improve! currently this constructor is only for HDF Explorer
            this.FileGranularity = fileGranularity;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public FileGranularity FileGranularity { get; private set; }

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
