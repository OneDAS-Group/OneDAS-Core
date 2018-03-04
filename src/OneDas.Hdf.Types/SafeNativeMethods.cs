using System;
using System.Runtime.InteropServices;
using System.Security;

namespace OneDas.Hdf
{
    public static class SafeNativeMethods
    {
        [SuppressUnmanagedCodeSecurity()]
        [DllImport("kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr destination, IntPtr source, uint length);
    }
}