using System;
using System.Runtime.InteropServices;

namespace OneDas.Buffers
{
    internal class SimpleBuffer : ISimpleBuffer
    {
        #region Fields

        private Memory<double> _buffer;

        #endregion

        #region Constructors

        public SimpleBuffer(double[] data)
        {
            _buffer = new Memory<double>(data);
        }

        #endregion

        #region Properties

        public int ElementSize => 8;

        public BufferType Type => BufferType.Simple;

        public Span<byte> RawBuffer => MemoryMarshal.Cast<double, byte>(_buffer.Span);

        public Span<double> Buffer => _buffer.Span;

        #endregion

        #region Methods

        public ISimpleBuffer ToSimpleBuffer()
        {
            return this;
        }

        public void Clear()
        {
            this.Buffer.Clear();
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
