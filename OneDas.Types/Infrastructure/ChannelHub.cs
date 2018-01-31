using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using OneDas.Plugin;

namespace OneDas.Infrastructure
{
    [DataContract]
    public class ChannelHub
    {
        #region "Fields"

        private List<DataPort> _associatedDataOutputSet;

        #endregion

        #region "Constructors"

        public ChannelHub()
        {
            this.TransferFunctionSet = new List<TransferFunction>();

            _associatedDataOutputSet = new List<DataPort>();
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
        public SampleRate SampleRate { get; private set; }

        [DataMember]
        public Guid Guid { get; private set; }

        [DataMember]
        public DateTime CreationDateTime { get; private set; }

        [DataMember]
        public string Unit { get; private set; }

        [DataMember]
        public List<TransferFunction> TransferFunctionSet { get; private set; }

        public DataPort AssociatedDataInput { get; private set; }

        public IEnumerable<DataPort> AssociatedDataOutputSet
        {
            get {
                return _associatedDataOutputSet;
            }
        }

        public List<ExtendedDataStorageBase> AssociatedDataStorageSet { get; set; }

        #endregion

        #region "Methods"

        public void SetAssociation(DataPort dataPort)
        {
            Contract.Requires(dataPort != null);

            switch (dataPort.DataDirection)
            {
                case DataDirection.Input:

                    this.AssociatedDataInput = dataPort;

                    break;

                case DataDirection.Output:

                    _associatedDataOutputSet.Add(dataPort);

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region "Serialization"

        [DataMember(Name = "SerializerDataInputId")]
        internal string AssociatedDataInputId { get; private set; }

        [DataMember(Name = "SerializerDataOutputIdSet")]
        internal IEnumerable<string> AssociatedDataOutputIdSet { get; private set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            _associatedDataOutputSet = new List<DataPort>();
        }

        #endregion
    }
}