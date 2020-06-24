using OneDas.Infrastructure;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.Buffers
{
    public static class BufferUtilities
    {
        public static unsafe double[] ToDouble<T>(Span<T> dataset) where T : unmanaged
        {
            var doubleData = new double[dataset.Length];

            fixed (T* dataPtr = dataset)
            {
                BufferUtilities.InternalToDouble(dataPtr, doubleData);
            }

            return doubleData;
        }

        public unsafe static double[] ApplyDatasetStatus<T>(Span<T> dataset, Span<byte> status) where T : unmanaged
        {
            var doubleData = new double[dataset.Length];

            fixed (T* dataPtr = dataset)
            {
                fixed (byte* statusPtr = status)
                {
                    BufferUtilities.InternalApplyDatasetStatus(dataPtr, statusPtr, doubleData);
                }
            }

            return doubleData;
        }

        public static double[] ApplyDatasetStatus2(Array dataset, byte[] status)
        {
            var methodName = nameof(BufferUtilities.ForwardApplyDatasetStatus);
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

        internal static unsafe void InternalToDouble<T>(T* dataPtr, double[] doubleData) where T : unmanaged
        {
            Parallel.For(0, doubleData.Length, i =>
            {
                doubleData[i] = GenericToDouble<T>.ToDouble(dataPtr[i]);
            });
        }

        private unsafe static void InternalApplyDatasetStatus<T>(T* dataPtr, byte* statusPtr, double[] doubleData) where T : unmanaged
        {
            Parallel.For(0, doubleData.Length, i =>
            {
                if (statusPtr[i] != 1)
                    doubleData[i] = double.NaN;
                else
                    doubleData[i] = GenericToDouble<T>.ToDouble(dataPtr[i]);
            });
        }

        private static double[] ForwardApplyDatasetStatus<T>(T[] dataset, byte[] status) where T : unmanaged
        {
            // This method is only required to correctly cast:
            // dataset: Array  -> Span<T>    (desired)         Array  -> object -> T[]    -> Span<T> (actual)   
            //  status: byte[] -> Span<byte> (desired)         byte[] -> object -> byte[] -> Span<T> (actual)   
            return BufferUtilities.ApplyDatasetStatus<T>(dataset, status);
        }
    }
}
