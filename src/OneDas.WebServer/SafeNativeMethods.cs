using System;
using System.Runtime.InteropServices;

namespace OneDas.WebServer
{
    public static class SafeNativeMethods
    {
        // Console
        public delegate void HandlerRoutine(CtrlType CtrlType);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool @add);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        // Window
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handle, ShowWindowCommand nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr handle);
    }

    public enum ShowWindowCommand
    {
        SW_HIDE = 0,
        SW_MAXIMIZE = 3,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_RESTORE = 9
    }

    public enum SystemCommand
    {
        SC_CLOSE = 0xf060
    }

    public enum CtrlType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }
}
