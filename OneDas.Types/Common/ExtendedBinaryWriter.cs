using System;
using System.IO;

namespace OneDas.Common
{
    public class ExtendedBinaryWriter : BinaryWriter
    {
        public ExtendedBinaryWriter(Stream stream) : base(stream)
        {
            //
        }

        private void WriteBE(byte[] data)
        {
            Array.Reverse(data);
            base.Write(data);
        }

        public void WriteBE(short value)
        {
            this.WriteBE(BitConverter.GetBytes(value));
        }

        public void WriteBE(ushort value)
        {
            this.WriteBE(BitConverter.GetBytes(value));
        }

        public void WriteBE(long value)
        {
            this.WriteBE(BitConverter.GetBytes(value));
        }

        public void WriteBE(ulong value)
        {
            this.WriteBE(BitConverter.GetBytes(value));
        }

        public void WriteBE(float value)
        {
            this.WriteBE(BitConverter.GetBytes(value));
        }

        public void WriteBE(double value)
        {
            this.WriteBE(BitConverter.GetBytes(value));
        }
    }
}
