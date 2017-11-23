using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OneDas.Common;

namespace OneDas.Infrastructure
{
    /// <summary>
    /// Represents an infrastructure to buffer data.
    /// </summary>
    public abstract class ExtendedDataStorageBase : DataStorageBase, IDisposable
    {
        #region "Fields"

        private GCHandle _gcHandle;
        private GCHandle _gcHandle_Status;

        private byte[] _statusSet;

        #endregion

        #region "Constructors"

        public ExtendedDataStorageBase(Array dataset, Type elementType) : this(dataset, new byte[dataset.Length], elementType)
        {
        }

        public ExtendedDataStorageBase(Array dataset, byte[] statusSet, Type elementType) : base(dataset)
        {
            Contract.Requires(dataset != null);
            Contract.Requires(statusSet != null);
            
            if (!elementType.IsPrimitive)
            {
                throw new Exception(ErrorMessage.DataStorage_ParameterTNonPrimitive);
            }

            if (dataset.Length != statusSet.Length)
            {
                throw new Exception(ErrorMessage.DataStorage_ArrayLengthMismatch);
            }

            _statusSet = statusSet;
            this.InternalExtendedDataStorageBase();
        }

        private void InternalExtendedDataStorageBase()
        {
            _gcHandle = GCHandle.Alloc(this.Dataset, GCHandleType.Pinned);
            _gcHandle_Status = GCHandle.Alloc(_statusSet, GCHandleType.Pinned);
        }

        #endregion

        #region "Properties"

        protected byte[] StatusSet
        {
            get
            {
                return _statusSet;
            }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Clears the content of the data and status buffers.
        /// </summary>
        /// <param name="storage"></param>
        public void Clear()
        {
            Array.Clear(this.Dataset, 0, this.Dataset.Length);
            Array.Clear(_statusSet, 0, this.Dataset.Length);
        }

        public static double[] ApplyDatasetStatus<T>(T[] dataset, byte[] dataset_status)
        {
            double[] dataset_double;

            dataset_double = new double[dataset.Count()];

            Parallel.For(0, dataset.Count(), x =>
            {
                if (dataset_status[x] != 1)
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

        /// <summary>
        /// The read methods provides a possibility to read a value from the buffer.
        /// </summary>
        /// <param name="index">The position within the buffer.</param>
        /// <param name="storage">The <see cref="StorageType"/>.</param>
        /// <returns>Returns a value from the buffer.</returns>
        public object Read(int index)
        {
            return this.Dataset.GetValue(index);
        }

        public byte ReadStatus(int index)
        {
            return _statusSet[index];
        }

        public void WriteStatus(int index, byte value)
        {
            _statusSet[index] = value;
        }

        public override IntPtr GetDataPointer(int index)
        {
            if (index >= this.Dataset.Length)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                return IntPtr.Add(_gcHandle.AddrOfPinnedObject(), index * this.ElementSize);
            }
        }

        /// <summary>
        /// Gets an <see cref="IntPtr"/> for the status buffer.
        /// </summary>
        /// <param name="storage">The <see cref="StorageType"/>.</param>
        /// <returns>Returns an <see cref="IntPtr"/> for the status buffer.</returns>
        public IntPtr GetStatusPointer(int index)
        {
            if (index >= this.Dataset.Length)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                return IntPtr.Add(_gcHandle_Status.AddrOfPinnedObject(), index * 1);
            }
        }

        public abstract double[] ApplyDatasetStatus();

        #endregion

        #region "IDisposable"

        private bool isDisposed;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                _gcHandle.Free();
                _gcHandle_Status.Free();
            }

            isDisposed = true;
        }

        ~ExtendedDataStorageBase()
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