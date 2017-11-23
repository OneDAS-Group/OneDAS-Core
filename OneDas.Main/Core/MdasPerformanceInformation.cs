using System;
using System.Diagnostics;
using OneDas.Infrastructure;

namespace OneDas.Main.Core
{
    public struct OneDasPerformanceInformation
    {
        #region "Contructors"

        public OneDasPerformanceInformation(OneDasState oneDasState, ProcessPriorityClass processPriorityClass, bool isDebugOutputEnabled, double lateBy, double cycleTime, double timerDrift, float cpuTime, int upTime)
        {
            this.OneDasState = oneDasState;
            this.ProcessPriorityClass = processPriorityClass;
            this.IsDebugOutputEnabled = isDebugOutputEnabled;
            this.LateBy = lateBy;
            this.CycleTime = cycleTime;
            this.TimerDrift = timerDrift;
            this.CpuTime = cpuTime;
            this.UpTime = upTime;
        }

        #endregion

        #region "Properties"

        public OneDasState OneDasState { get; set; }
        public bool IsDebugOutputEnabled { get; set; }
        public ProcessPriorityClass ProcessPriorityClass { get; set; }
        public double LateBy { get; set; }
        public double CycleTime { get; set; }
        public double TimerDrift { get; set; }
        public float CpuTime { get; set; }
        public int UpTime { get; set; }

        #endregion
    }
}