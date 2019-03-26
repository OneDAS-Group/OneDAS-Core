using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Extensibility
{
    [DataContract]
    public abstract class DataWriterExtensionSettingsBase : ExtensionSettingsBase
    {
        #region "Constructors

        public DataWriterExtensionSettingsBase()
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
                throw new Exception(ErrorMessage.DataWriterExtensionSettingsBase_FileGranularityInvalid);
            }
        }

        #endregion
    }
}
