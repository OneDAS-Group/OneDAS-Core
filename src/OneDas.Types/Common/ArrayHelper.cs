using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Common
{
    public static class ArrayHelper
    {
        public static void ToUnmanagedArray(this Array valueSet, IntPtr targetPtr)
        {
            IntPtr sourcePtr = default;
            int counter = 0;
            int elementTypeSize = 0;
            Type elementType = valueSet.GetType().GetElementType();

            if (!(elementType.IsValueType && !elementType.IsPrimitive && !elementType.IsEnum))
            {
                throw new Exception("Array element must be a structure.");
            }

            elementTypeSize = Marshal.SizeOf(elementType);

            valueSet.Cast<ValueType>().ToList().ForEach(x =>
            {
                sourcePtr = Marshal.AllocHGlobal(elementTypeSize);
                Marshal.StructureToPtr(x, sourcePtr, false);
                SafeNativeMethods.RtlMoveMemory(IntPtr.Add(targetPtr, counter), sourcePtr, Convert.ToUInt32(elementTypeSize));
                counter += elementTypeSize;
                Marshal.FreeHGlobal(sourcePtr);
            });
        }
    }
}
