using System;

namespace OneDas.Common
{
    public static class WindowHelper
    {
        public static void ModifyConsoleMenu(SystemCommand systemCommand, int flags)
        {
            SafeNativeMethods.DeleteMenu(SafeNativeMethods.GetSystemMenu(SafeNativeMethods.GetConsoleWindow(), false), (int)systemCommand, flags);
        }

        public static void BringWindowToFront(IntPtr windowHandle)
        {
            if (SafeNativeMethods.IsIconic(windowHandle))
            {
                SafeNativeMethods.ShowWindow(windowHandle, ShowWindowCommand.SW_RESTORE);
            }

            SafeNativeMethods.SetForegroundWindow(windowHandle);
        }
    }
}