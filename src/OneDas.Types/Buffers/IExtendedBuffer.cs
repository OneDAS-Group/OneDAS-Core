using System;

namespace OneDas.Buffers
{
    public interface IExtendedBuffer : IBuffer
    {
        Span<byte> StatusBuffer { get; }
    }
}
