using System;
using System.Runtime.InteropServices;

namespace OneDas.Hdf.VdsTool
{
    public static class SafeNativeMethods
    {
        // Console
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        // Window
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
    }

    public enum SystemCommand
    {
        SC_CLOSE = 0xf060
    }
}
