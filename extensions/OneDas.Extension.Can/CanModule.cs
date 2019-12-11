using OneDas.Extensibility;
using OneDas.Infrastructure;
using System.Runtime.Serialization;

namespace OneDas.Extension.Can
{
    public class CanModule : OneDasModule
    {
        #region "Constructors"

        public CanModule(uint identifier, CanFrameFormat frameFormat, OneDasDataType dataType, DataDirection dataDirection, Endianness endianness, int size) 
            : base(dataType, dataDirection, endianness, size)
        {
            this.Identifier = identifier;
            this.FrameFormat = frameFormat;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public uint Identifier { get; set; }

        [DataMember]
        public CanFrameFormat FrameFormat { get; set; }

        public int ByteOffset { get; set; }

        #endregion

#warning Validation?
    }
}
