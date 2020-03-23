using OneDas.Infrastructure;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.Buffers
{
    public static class BufferUtilities
    {
        public unsafe static double[] ApplyDatasetStatus<T>(Span<T> dataset, Span<byte> status) where T : unmanaged
        {
            var dataset_double = new double[dataset.Length];

            fixed (T* bufferPtr = dataset)
            {
                fixed (byte* statusBufferPtr = status)
                {
                    var usafeOps = new UnsafeOps<T>(bufferPtr, statusBufferPtr, dataset_double);
                    Parallel.For(0, dataset.Length, usafeOps.Lambda);
                }
            }

            return dataset_double;
        }

        public static double[] ApplyDatasetStatus2(Array dataset, byte[] status)
        {
            var methodName = nameof(BufferUtilities.InternalApplyDatasetStatus);
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            var genericType = dataset.GetType().GetElementType();
            var parameters = new object[] { dataset, status };

            var result = OneDasUtilities.InvokeGenericMethod(typeof(BufferUtilities), null, methodName, flags, genericType, parameters);

            return (double[])result;
        }

        public static ISimpleBuffer CreateSimpleBuffer(double[] data)
        {
            return new SimpleBuffer(data);
        }

        public static IExtendedBuffer CreateExtendedBuffer(OneDasDataType dataType, int length)
        {
            var type = typeof(ExtendedBuffer<>).MakeGenericType(new Type[] { OneDasUtilities.GetTypeFromOneDasDataType(dataType) });
            return (IExtendedBuffer)Activator.CreateInstance(type, length);
        }

        private unsafe static double[] InternalApplyDatasetStatus<T>(T[] dataset, byte[] status) where T : unmanaged
        {
            return BufferUtilities.ApplyDatasetStatus<T>(dataset, status);
        }

        private unsafe class UnsafeOps<T> where T : unmanaged
        {
            private T* _bufferPtr;
            private byte* _statusBufferPtr;
            private double[] _dataset_double;

            public UnsafeOps(T* bufferPtr, byte* statusBufferPtr, double[] dataset_double)
            {
                _bufferPtr = bufferPtr;
                _statusBufferPtr = statusBufferPtr;
                _dataset_double = dataset_double;
            }

            public unsafe void Lambda(int i)
            {
                if (_statusBufferPtr[i] != 1)
                    _dataset_double[i] = double.NaN;
                else
                    _dataset_double[i] = GenericToDouble<T>.ToDouble(_bufferPtr[i]);
            }
        }
    }
}
