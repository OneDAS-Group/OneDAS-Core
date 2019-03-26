using System;
using System.Diagnostics;

namespace OneDas.Core.Engine
{
    public struct OneDasPerformanceInformation
    {
        #region "Contructors"

        public OneDasPerformanceInformation(DateTime dateTime, OneDasState oneDasState, ProcessPriorityClass processPriorityClass, double lateBy, double cycleTime, double timerDrift, float cpuTime, int upTime, long freeDiskSpace, long totalDiskSize)
        {
            this.DateTime = dateTime;
            this.OneDasState = oneDasState;
            this.ProcessPriorityClass = processPriorityClass;
            this.LateBy = lateBy;
            this.CycleTime = cycleTime;
            this.TimerDrift = timerDrift;
            this.CpuTime = cpuTime;
            this.UpTime = upTime;
            this.FreeDiskSpace = freeDiskSpace;
            this.TotalDiskSize = totalDiskSize;
        }

        #endregion

        #region "Properties"

        public DateTime DateTime { get; set; }
        public OneDasState OneDasState { get; set; }
        public ProcessPriorityClass ProcessPriorityClass { get; set; }
        public double LateBy { get; set; }
        public double CycleTime { get; set; }
        public double TimerDrift { get; set; }
        public float CpuTime { get; set; }
        public int UpTime { get; set; }
        public long FreeDiskSpace { get; set; }
        public long TotalDiskSize { get; set; }

        #endregion
    }
}