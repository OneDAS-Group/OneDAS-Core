using OneDas.Infrastructure;
using System;
using System.Runtime.Serialization;

namespace OneDas.Plugin
{
    [DataContract]
    public class DataPort
    {
        public DataPort(string name, OneDasDataType dataType, DataDirection dataDirection, Endianness endianness)
        {
            this.Name = name;
            this.DataType = dataType;
            this.DataDirection = dataDirection;
            this.Endianness = endianness;

            this.BitOffset = -1; // i.e. bool is treated as byte-oriented
        }

        // properties
        [DataMember]
        public string Name { get; private set; }
        public OneDasDataType DataType { get; }
        [DataMember]
        public OneDasDataType OneDasDataType { get; private set; }

        [DataMember]
        public DataDirection DataDirection { get; private set; }

        [DataMember]
        public Endianness Endianness { get; private set; }

        public DataGatewayPluginLogicBase AssociatedDataGateway { get; set; }
        public IntPtr DataPtr { get; set; }
        public int BitOffset { get; set; }

        // methods
        public virtual string GetId()
        {
            return this.Name;
        }
    }
}
