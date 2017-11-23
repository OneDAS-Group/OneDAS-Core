using System;
using System.Threading;

namespace OneDas.Common
{
    public static class GcNotification
    {
        private class GenObject
        {
            private int _generation;

            public GenObject(int generation)
            {
                _generation = generation;
            }

            ~GenObject()
            {
                bool flag = GC.GetGeneration(this) >= _generation;

                if (flag)
                {
                    Action<int> action = Volatile.Read<Action<int>>(ref GcNotification._GcDone);
                    bool flag2 = action != null;
                    if (flag2)
                    {
                        action(_generation);
                    }
                }

                bool flag3 = GcNotification._GcDone != null && !AppDomain.CurrentDomain.IsFinalizingForUnload() && !Environment.HasShutdownStarted;

                if (flag3)
                {
                    bool flag4 = _generation == 0;
                    if (flag4)
                    {
                        GcNotification.GenObject GenObject = new GcNotification.GenObject(0);
                    }
                    else
                    {
                        GC.ReRegisterForFinalize(this);
                    }
                }
            }
        }

        private static Action<int> _GcDone = null;
        public static event Action<int> GcDone
        {
            add
            {
                bool flag = _GcDone == null;
                if (flag)
                {
                    GcNotification.GenObject genObject = new GcNotification.GenObject(0);
                    genObject = new GcNotification.GenObject(1);
                    genObject = new GcNotification.GenObject(2);
                }
                _GcDone = (Action<int>)Delegate.Combine(_GcDone, value);
            }
            remove
            {
                _GcDone = (Action<int>)Delegate.Remove(_GcDone, value);
            }
        }
    }
}
