using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Common
{
    public static class SingeltonHelper
    {
        private static List<Mutex> _mutexSet;

        static SingeltonHelper()
        {
            _mutexSet = new List<Mutex>();
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