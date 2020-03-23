using System;

namespace OneDas.Buffers
{
    public interface IBuffer : IDisposable
    {
        #region Properties

        int ElementSize { get; }

        Span<byte> RawBuffer { get; }

        BufferType Type { get; }

        #endregion

        #region Methods

        ISimpleBuffer ToSimpleBuffer();

        void Clear();

        #endregion
    }
}
