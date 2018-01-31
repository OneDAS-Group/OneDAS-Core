class OneDasPerformanceInformationViewModel
{
    public LateBy: number
    public CycleTime: number
    public TimerDrift: number
    public CpuTime: number
    public TimerDriftMicroseconds: number

    public UpTimeHours: number
    public UpTimeMinutes: number
    public UpTimeSeconds: number

    constructor(performanceInformationModel: any)
    {
        let timeSpan: number
        let secPerMinute = 60;
        let secPerHour = secPerMinute * 60;

        this.LateBy = performanceInformationModel.LateBy
        this.CycleTime = performanceInformationModel.CycleTime
        this.TimerDrift = performanceInformationModel.TimerDrift
        this.CpuTime = performanceInformationModel.CycleTime

        this.TimerDriftMicroseconds = Math.abs(this.TimerDrift) / 1000

        // calculate system uptime
        timeSpan = performanceInformationModel.UpTime

        this.UpTimeHours = Math.floor(timeSpan / secPerHour)
        timeSpan -= this.UpTimeHours * secPerHour

        this.UpTimeMinutes = Math.floor(timeSpan / secPerMinute)
        timeSpan -= this.UpTimeMinutes * secPerMinute

        this.UpTimeSeconds = timeSpan
    }
}