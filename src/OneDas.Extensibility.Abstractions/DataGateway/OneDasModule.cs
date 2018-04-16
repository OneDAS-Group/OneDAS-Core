using OneDas.Infrastructure;
using System;
using System.Runtime.Serialization;

namespace OneDas.Extensibility
{
    [DataContract]
    public class OneDasModule
    {
        #region "Constructors"

        public OneDasModule(OneDasDataType dataType, DataDirection dataDirection, Endianness endianness, int size)
        {
            this.DataType = dataType;
            this.DataDirection = dataDirection;
            this.Endianness = endianness;
            this.Size = size;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public OneDasDataType DataType { get; private set; }

        [DataMember]
        public DataDirection DataDirection { get; private set; }

        [DataMember]
        public Endianness Endianness { get; private set; }

        [DataMember]
        public int Size { get; private set; }

        #endregion

        #region "Methods"

        public int GetByteCount(int booleanBitSize = 8)
        {
            if (this.DataType == OneDasDataType.BOOLEAN)
            {
                return (int)Math.Ceiling(booleanBitSize * (double)this.Size / 8);
            }
            else
            {
                return ((int)this.DataType & 0x0FF) / 8 * this.Size;
            }
        }

        #endregion
    }
}