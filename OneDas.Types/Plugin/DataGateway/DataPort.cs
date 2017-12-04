using System;
using System.Runtime.Serialization;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    [DataContract]
    public class DataPort
    {
        public DataPort(string name, OneDasDataType oneDasDataType, DataDirection dataDirection)
        {
            this.Name = name;
            this.OneDasDataType = oneDasDataType;
            this.DataDirection = dataDirection;

            this.BitOffset = -1; // i.e. bool is treated as byte-oriented
        }

        // properties
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public OneDasDataType OneDasDataType { get; private set; }

        [DataMember]
        public DataDirection DataDirection { get; private set; }

        public IntPtr DataPtr { get; set; }
        public int BitOffset { get; set; }
        public DataGatewayPluginSettingsBase AssociatedDataGateway { get; set; }

        // methods
        public virtual string GetId()
        {
            return this.Name;
        }

        public string ToUniqueIdentifier()
        {
            return $"{this.AssociatedDataGateway.Description.Id} ({this.AssociatedDataGateway.Description.InstanceId}) / {this.GetId()}";
        }
    }
}
