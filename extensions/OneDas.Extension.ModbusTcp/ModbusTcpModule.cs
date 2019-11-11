using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Runtime.Serialization;

namespace OneDas.Extension.ModbusTcp
{
    public class ModbusTcpModule : OneDasModule
    {
        #region "Constructors"

        public ModbusTcpModule(ushort startingAddress, ModbusTcpObjectTypeEnum objectType, OneDasDataType dataType, DataDirection dataDirection, Endianness endianness, int size) : base(dataType, dataDirection, endianness, size)
        {
            this.StartingAddress = startingAddress;
            this.ObjectType = objectType;

            this.Initialize();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public ushort StartingAddress { get; set; }

        [DataMember]
        public ModbusTcpObjectTypeEnum ObjectType { get; set; }

        public int ByteOffset { get; set; }

        public ushort Quantity { get; private set; }
        public int ByteCount { get; private set; }

        #endregion

        #region "Methods"

        private void Initialize()
        {
            this.ByteCount = this.GetByteCount();
            this.Quantity = (ushort)Math.Ceiling(this.ByteCount / (double)2);
        }

        #endregion

        #region "Serialization"

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            this.Initialize();
        }

        #endregion
    }
}
