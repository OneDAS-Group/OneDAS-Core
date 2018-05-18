using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Threading;

namespace OneDas.Core.Engine
{
    public class RtTimer
    {
        #region "Fields"

        private Func<DateTime> _func;
        private TimeSpan _interval;
        private TimeSpan _offset;
        private Thread _thread;
        private AutoResetEvent _autoResetEvent_StopThread = new AutoResetEvent(false);
        private AutoResetEvent _autoResetEvent_Feedback = new AutoResetEvent(false);

        #endregion

        #region "Constructors"

        public RtTimer()
        {
            if (!Stopwatch.IsHighResolution)
            {
                throw new Exception("There was no realtime clock found on this system.");
            }
        }

        #endregion

        #region "Properties"

        public bool IsEnabled { get; private set; } = false;
        public double LateBy { get; set; }
        public double LastActionTime { get; set; }

        #endregion

        #region "Methods"

        public void Start(TimeSpan interval, TimeSpan offset, Func<DateTime> func)
        {
            this.Start(interval, offset, func, ThreadPriority.Highest, true);
        }

        public void Start(TimeSpan interval, TimeSpan offset, Func<DateTime> func, ThreadPriority threadPriority)
        {
            this.Start(interval, offset, func, threadPriority, false);
        }

        private void Start(TimeSpan interval, TimeSpan offset, Func<DateTime> func, ThreadPriority threadPriority, bool boostToRealTime)
        {
            Contract.Requires(func != null);

            if (this.IsEnabled)
            {
                throw new InvalidOperationException("The RtTimer is already started.");
            }

            _interval = interval;
            _offset = offset;
            _func = func;

            _thread = new Thread(() =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // step 1: timer and thread preperation
                    SafeNativeMethods.TimeBeginPeriod(1u);

                    ////SafeNativeMethods.NtSetTimerResolution(5000, True, Nothing)

                    if (boostToRealTime)
                    {
                        SafeNativeMethods.SetThreadPriority(SafeNativeMethods.GetCurrentThread(), UnmanagedThreadPriority.THREAD_PRIORITY_TIME_CRITICAL);
                    }
                    else
                    {
                        switch (threadPriority)
                        {
                            case ThreadPriority.AboveNormal:
                                SafeNativeMethods.SetThreadPriority(SafeNativeMethods.GetCurrentThread(), UnmanagedThreadPriority.THREAD_PRIORITY_ABOVE_NORMAL); break;

                            case ThreadPriority.BelowNormal:
                                SafeNativeMethods.SetThreadPriority(SafeNativeMethods.GetCurrentThread(), UnmanagedThreadPriority.THREAD_PRIORITY_BELOW_NORMAL); break;

                            case ThreadPriority.Highest:
                                SafeNativeMethods.SetThreadPriority(SafeNativeMethods.GetCurrentThread(), UnmanagedThreadPriority.THREAD_PRIORITY_HIGHEST); break;

                            case ThreadPriority.Lowest:
                                SafeNativeMethods.SetThreadPriority(SafeNativeMethods.GetCurrentThread(), UnmanagedThreadPriority.THREAD_PRIORITY_LOWEST); break;

                            case ThreadPriority.Normal:
                                SafeNativeMethods.SetThreadPriority(SafeNativeMethods.GetCurrentThread(), UnmanagedThreadPriority.THREAD_PRIORITY_NORMAL); break;

                            default:
                                throw new ArgumentException(nameof(threadPriority));
                        }
                    }

                    // step 2: start timer loop
                    this.TimerLoop();

                    // step 3: clean up
                    SafeNativeMethods.TimeEndPeriod(1);
                }
                else
                {
                    // step 1: timer and thread preperation
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;

                    // step 2: start timer loop
                    this.TimerLoop();

                    // step 3: clean up
                    // -
                }
            })
            { Name = "RtTimer" };

            _thread.Start();

            this.IsEnabled = true;
        }

        public void Stop()
        {
            if (this.IsEnabled)
            {
                _autoResetEvent_StopThread.Set();
                _autoResetEvent_Feedback.WaitOne(Timeout.Infinite);

                this.IsEnabled = false;
                this.LateBy = 0;
                this.LastActionTime = 0;
            }
        }

        private void TimerLoop()
        {
            DateTime currentDateTime = default;
            Stopwatch stopwatch = default;
            TimeSpan timeSpan_Sleep = default;
            TimeSpan timeSpan_Elapsed = default;

            stopwatch = Stopwatch.StartNew();

            while (true)
            {
                timeSpan_Elapsed = stopwatch.Elapsed;

                if (timeSpan_Elapsed >= timeSpan_Sleep)
                {
                    stopwatch.Restart();
                    currentDateTime = _func.Invoke();

                    this.LastActionTime = stopwatch.Elapsed.TotalMilliseconds;
                    this.LateBy = (timeSpan_Elapsed - timeSpan_Sleep).TotalMilliseconds;

                    if (currentDateTime == DateTime.MinValue)
                    {
                        timeSpan_Sleep = _interval;
                    }
                    else
                    {
                        timeSpan_Sleep = currentDateTime.RoundUp(_interval).Add(_offset) - currentDateTime;
                    }
                }

                Thread.Sleep(1);

                if (_autoResetEvent_StopThread.WaitOne(0))
                {
                    _autoResetEvent_Feedback.Set();

                    break;
                }
            }
        }

        #endregion
    }
}