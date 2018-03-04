using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.WebServer
{
    public static class WebServerUtilities
    {
        private static List<Mutex> _mutexSet;

        static WebServerUtilities()
        {
            _mutexSet = new List<Mutex>();
        }

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

        public static string Reverse(this string value)
        {
            char[] charArray;

            charArray = value.ToCharArray();
            Array.Reverse(charArray);

            return new string(charArray);
        }

        public static bool EnsureSingeltonInstance(Guid identifier, Action newInstanceNotification)
        {
            string name;
            Mutex mutex;
            EventWaitHandle eventWaitHandle;

            name = identifier.ToString();
            mutex = new Mutex(true, name);
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, name.Reverse());

            _mutexSet.Add(mutex);

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex.ReleaseMutex();

                Task.Run(() =>
                {
                    while (true)
                    {
                        eventWaitHandle.WaitOne();
                        newInstanceNotification.Invoke();
                    }
                });

                return true;
            }
            else
            {
                eventWaitHandle.Set();

                return false;
            }
        }
    }
}