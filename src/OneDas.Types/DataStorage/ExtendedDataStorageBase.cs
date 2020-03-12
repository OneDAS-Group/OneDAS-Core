using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OneDas.DataStorage
{
    /// <summary>
    /// Represents an infrastructure to buffer data.
    /// </summary>
    public abstract class ExtendedDataStorageBase : DataStorageBase, IExtendedDataStorage
    {
        #region "Fields"

        int _byteCount;

        #endregion

        #region "Constructors"

        public ExtendedDataStorageBase(Type type, int elementCount) : base(type, elementCount, DataStorageType.Extended)
        {
            _byteCount = elementCount;

            this.StatusBufferPtr = Marshal.AllocHGlobal(_byteCount);
            this.StatusBuffer.Clear();
        }

        public ExtendedDataStorageBase(Type type, byte[] statusSet) : base(type, statusSet.Length, DataStorageType.Extended)
        {
            _byteCount = statusSet.Length;

            this.StatusBufferPtr = Marshal.AllocHGlobal(_byteCount);
            statusSet.CopyTo(this.StatusBuffer);
        }

        #endregion

        #region "Properties"

        // Improve: remove this if possible to replace it by Span<T>
        public IntPtr StatusBufferPtr { get; private set; }

        public Span<byte> StatusBuffer
        {
            get
            {
                return this.GetStatusBuffer();
            }
        }

        #endregion

        #region "Methods"

        public unsafe Span<byte> GetStatusBuffer()
        {
            return new Span<byte>(this.StatusBufferPtr.ToPointer(), _byteCount);
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

        public static double[] ApplyDatasetStatus2(Array dataset, byte[] statusSet)
        {
            var methodName = nameof(ExtendedDataStorageBase.ApplyDatasetStatus);
            var flags = BindingFlags.Public | BindingFlags.Static;
            var genericType = dataset.GetType().GetElementType();
            var parameters = new object[] { dataset, statusSet };

            var result = OneDasUtilities.InvokeGenericMethod<ExtendedDataStorageBase>(null, methodName, flags, genericType, parameters);

            return (double[])result;
        }

        public abstract double[] ApplyDatasetStatus();

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Marshal.FreeHGlobal(this.StatusBufferPtr);
                disposedValue = true;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}