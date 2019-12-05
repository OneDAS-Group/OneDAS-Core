using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Runtime.Serialization;

namespace OneDas.Extension.Modbus
{
    public class ModbusModule : OneDasModule
    {
        #region "Constructors"

        public ModbusModule(ushort startingAddress, ModbusObjectTypeEnum objectType, OneDasDataType dataType, DataDirection dataDirection, Endianness endianness, int size) : base(dataType, dataDirection, endianness, size)
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
        public ModbusObjectTypeEnum ObjectType { get; set; }

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
