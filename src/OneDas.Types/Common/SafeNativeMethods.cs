using System;
using System.Runtime.InteropServices;
using System.Security;

namespace OneDas.Common
{
    public static class SafeNativeMethods
    {
        // Mulitmedia timer

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint milliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint milliseconds);

        [DllImport("ntdll.dll")]
        public static extern int NtQueryTimerResolution(ref uint minimumResolution, ref uint maximumResolution, ref uint currentResolution);

        [DllImport("ntdll.dll")]
        public static extern int NtSetTimerResolution(uint desiredResolution, bool setResolution, ref uint currentResolution);

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

        // Thread

        [DllImport("kernel32.dll")]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [DllImport("kernel32.dll")]
        public static extern int SetThreadPriority(IntPtr hThread, UnmanagedThreadPriority nPriority);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        [DllImport("Kernel32.dll")]
        public static extern int GetCurrentThreadId();


        // Memory

        [SuppressUnmanagedCodeSecurity()]
        [DllImport("kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr destination, IntPtr source, uint length);
    }

    [Flags]
    public enum ExecutionState : uint
    {
        CONTINUOUS = 0x80000000u,
        DISPLAY_REQUIRED = 0x2u,
        SYSTEM_REQUIRED = 0x1u
    }

    public enum UnmanagedThreadPriority
    {
        THREAD_PRIORITY_LOWEST = -2,
        THREAD_PRIORITY_BELOW_NORMAL = -1,
        THREAD_PRIORITY_NORMAL = 0,
        THREAD_PRIORITY_ABOVE_NORMAL = 1,
        THREAD_PRIORITY_HIGHEST = 2,
        THREAD_PRIORITY_TIME_CRITICAL = 15
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
