using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OneDas.Infrastructure;

namespace OneDas.Core.ProjectManagement
{
    [DataContract]
    public class ChannelHubSettings
    {
        #region "Constructors"

        public ChannelHubSettings()
        {
            this.TransferFunctionSet = new List<TransferFunction>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Group { get; private set; }

        [DataMember]
        public OneDasDataType DataType { get; private set; }

        [DataMember]
        public Guid Guid { get; private set; }

        [DataMember]
        public DateTime CreationDateTime { get; private set; }

        [DataMember]
        public string Unit { get; private set; }

        [DataMember]
        public List<TransferFunction> TransferFunctionSet { get; private set; }

        [DataMember]
        public string AssociatedDataInputId { get; private set; }

        [DataMember]
        public IEnumerable<string> AssociatedDataOutputIdSet { get; private set; }

        #endregion
    }
}