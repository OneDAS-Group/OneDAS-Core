using System;

namespace OneDas.Buffers
{
    public interface ISimpleBuffer : IBuffer
    {
        Span<double> Buffer { get; }
    }
}
