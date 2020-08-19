using OneDas.Buffers;
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
            this.FilePeriod = TimeSpan.FromDays(1);
            this.BufferRequestSet = new List<BufferRequest>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public TimeSpan FilePeriod { get; set; }

        [DataMember]
        public bool SingleFile { get; set; }

        [DataMember]
        public List<BufferRequest> BufferRequestSet { get; set; }

        #endregion

        #region "Methods"

        public override void Validate()
        {
            base.Validate();

            if (this.FilePeriod == TimeSpan.Zero)
                throw new Exception(ErrorMessage.DataWriterExtensionSettingsBase_FileGranularityInvalid);
        }

        #endregion
    }
}
