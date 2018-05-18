using System;
using System.Collections.Generic;
using System.Threading;

namespace OneDas.WebServer
{
    public static class WebServerUtilities
    {
        private static List<Mutex> _mutexSet;

        static WebServerUtilities()
        {
            _mutexSet = new List<Mutex>();
        }

        public static bool EnsureSingeltonInstance(Guid identifier)
        {
            string name;
            Mutex mutex;

            name = identifier.ToString();
            mutex = new Mutex(true, name);

            _mutexSet.Add(mutex);

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex.ReleaseMutex();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}