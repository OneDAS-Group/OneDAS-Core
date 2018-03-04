using OneDas.Infrastructure;
using System.Collections.Generic;
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
            this.BufferRequestSet = new List<BufferRequest>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public FileGranularity FileGranularity { get; set; }

        [DataMember]
        public List<BufferRequest> BufferRequestSet { get; set; }

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
