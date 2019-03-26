using OneDas.DataStorage;
using System;
using System.Runtime.InteropServices;

namespace OneDas.Extensibility
{
    /// <summary>
    /// Represents an infrastructure to buffer data.
    /// </summary>
    public abstract class DataStorageBase : IDataStorage, IDisposable
    {
        #region "Fields"

        int _byteCount;

        #endregion

        #region "Constructors"

        public unsafe DataStorageBase(Type type, int elementCount)
        {
            this.ElementSize = OneDasUtilities.SizeOf(type);

            _byteCount = this.ElementSize * elementCount;

            this.DataBufferPtr = Marshal.AllocHGlobal(_byteCount);
            this.DataBuffer.Clear();
        }

        #endregion

        #region "Properties"

        // Improve: remove this if possible to replace it by Span<T>
        public IntPtr DataBufferPtr { get; }

        // Improve: remove this if possible to replace it by Span<T>
        public int ElementSize { get; }

        public unsafe Span<byte> DataBuffer
        {
            get
            {
                return new Span<byte>(this.DataBufferPtr.ToPointer(), _byteCount);
            }
        }

        #endregion

        #region "Methods"

        public abstract object GetValue(int index);

        public unsafe Span<T> GetDataBuffer<T>() where T : struct
        {
            return MemoryMarshal.Cast<byte, T>(new Span<byte>(this.DataBufferPtr.ToPointer(), _byteCount));
        }

        public virtual void Clear()
        {
            this.DataBuffer.Clear();
        }

        public abstract ISimpleDataStorage ToSimpleDataStorage();

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Marshal.FreeHGlobal(this.DataBufferPtr);
                disposedValue = true;
            }
        }

        ~DataStorageBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}