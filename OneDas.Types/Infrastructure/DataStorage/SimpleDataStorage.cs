using System;
using System.Runtime.InteropServices;

namespace OneDas.Infrastructure
{
    public class SimpleDataStorage : DataStorageBase, IDisposable
    {
        #region "Fields"

        protected GCHandle _gcHandle;

        private readonly double[] _buffer;

        #endregion

        #region "Constuctors"

        public SimpleDataStorage(double[] dataset) : base(dataset)
        {
            _buffer = dataset;
            _gcHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        }

        #endregion

        #region "Properties"

        public new double[] Dataset
        {
            get
            {
                return _buffer;
            }
        }

        #endregion

        #region "Methods"

        public override IntPtr GetDataPointer(int index)
        {
            if (index >= _buffer.Length)
            {
                throw new IndexOutOfRangeException();   
            }
            else
            {
                return IntPtr.Add(_gcHandle.AddrOfPinnedObject(), index * this.ElementSize);
            }           
        }

        public override SimpleDataStorage ToSimpleDataStorage()
        {
            return this;
        }

        #endregion

        #region "IDisposable"

        private bool isDisposed;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                _gcHandle.Free();
            }

            isDisposed = true;
        }

        ~SimpleDataStorage()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}