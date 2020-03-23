using System;
using System.Buffers;

namespace OneDas.Buffers
{
    internal abstract class ExtendedBufferBase : IExtendedBuffer
    {
        #region Fields

        private IMemoryOwner<byte> _statusBuffer;

        #endregion

        #region Constructors

        public ExtendedBufferBase(int length)
        {
            _statusBuffer = MemoryPool<byte>.Shared.Rent(length);
        }

        #endregion

        #region Properties

        public BufferType Type => BufferType.Extended;

        public Span<byte> StatusBuffer => _statusBuffer.Memory.Span;

        public abstract int ElementSize { get; }

        public abstract Span<byte> RawBuffer { get; }

        #endregion

        #region Methods

        public abstract ISimpleBuffer ToSimpleBuffer();

        public virtual void Clear()
        {
            this.StatusBuffer.Clear();
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _statusBuffer.Dispose();

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
