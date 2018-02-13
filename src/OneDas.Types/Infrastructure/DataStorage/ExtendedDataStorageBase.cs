using OneDas.Common;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OneDas.Infrastructure
{
    /// <summary>
    /// Represents an infrastructure to buffer data.
    /// </summary>
    public abstract class ExtendedDataStorageBase : DataStorageBase, IDisposable
    {
        #region "Fields"

        int _byteCount;
        IntPtr _statusBufferPtr;

        #endregion

        #region "Constructors"

        public ExtendedDataStorageBase(Type type, int elementCount) : base(type, elementCount)
        {
            _byteCount = elementCount;
            _statusBufferPtr = Marshal.AllocHGlobal(elementCount);

            this.GetStatusBuffer().Clear();
        }

        #endregion

        #region "Methods"

        public unsafe Span<byte> GetStatusBuffer()
        {
            return new Span<byte>(_statusBufferPtr.ToPointer(), _byteCount);
        }

        public override void Clear()
        {
            base.Clear();
            this.GetStatusBuffer().Clear();
        }

        public static double[] ApplyDatasetStatus<T>(T[] dataset, byte[] statusSet)
        {
            double[] dataset_double;

            dataset_double = new double[dataset.Length];

            Parallel.For(0, dataset.Length, x =>
            {
                if (statusSet[x] != 1)
                {
                    dataset_double[x] = double.NaN;
                }
                else
                {
                    dataset_double[x] = GenericToDouble<T>.ToDouble(dataset[x]);
                }
            });

            return dataset_double;
        }

        public abstract double[] ApplyDatasetStatus();

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected new virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Marshal.FreeHGlobal(_statusBufferPtr);
                Marshal.FreeHGlobal(this.DataBufferPtr);
                disposedValue = true;
            }
        }

        ~ExtendedDataStorageBase()
        {
            Dispose(false);
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}