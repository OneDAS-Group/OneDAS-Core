using System;
using System.Runtime.Serialization;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    [DataContract]
    public class OneDasModule
    {
        #region "Constructors"

        public OneDasModule(OneDasDataType dataType, DataDirection dataDirection, int size)
        {
            this.DataType = dataType;
            this.DataDirection = DataDirection;
            this.Size = size;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public OneDasDataType DataType { get; set; }

        [DataMember]
        public DataDirection DataDirection { get; set; }

        [DataMember]
        public int Size { get; set; }

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