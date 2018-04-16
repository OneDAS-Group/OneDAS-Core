using System;
using System.Runtime.InteropServices;

namespace OneDas.Core
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


        // Thread
        [DllImport("kernel32.dll")]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [DllImport("kernel32.dll")]
        public static extern int SetThreadPriority(IntPtr hThread, UnmanagedThreadPriority nPriority);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        [DllImport("Kernel32.dll")]
        public static extern int GetCurrentThreadId();
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
}
