using System;
using System.Diagnostics;
using OneDas.Infrastructure;

namespace OneDas.Engine.Core
{
    public struct OneDasPerformanceInformation
    {
        #region "Contructors"

        public OneDasPerformanceInformation(DateTime dateTime, OneDasState oneDasState, ProcessPriorityClass processPriorityClass, double lateBy, double cycleTime, double timerDrift, float cpuTime, int upTime)
        {
            this.DateTime = dateTime;
            this.OneDasState = oneDasState;
            this.ProcessPriorityClass = processPriorityClass;
            this.LateBy = lateBy;
            this.CycleTime = cycleTime;
            this.TimerDrift = timerDrift;
            this.CpuTime = cpuTime;
            this.UpTime = upTime;
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

        #endregion
    }
}