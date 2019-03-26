using System;

namespace OneDas.DataStorage
{
    public interface IDataStorage
    {
        #region "Properties"

        IntPtr DataBufferPtr { get; }
        int ElementSize { get; }
        Span<byte> DataBuffer { get; }

        #endregion

        #region "Methods"

        object GetValue(int index);

        Span<T> GetDataBuffer<T>() where T : struct;

        void Clear();

        ISimpleDataStorage ToSimpleDataStorage();

        #endregion
    }
}