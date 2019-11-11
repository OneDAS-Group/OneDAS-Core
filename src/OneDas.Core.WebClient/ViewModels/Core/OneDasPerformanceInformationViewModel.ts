class OneDasPerformanceInformationViewModel
{
    public LateBy: number
    public CycleTime: number
    public TimerDrift: number
    public CpuTime: number
    public TimerDriftMicroseconds: number
    public ConsumedDiskSpace: number
    public TotalDiskSize: number

    public Time: string

    public UpTimeHours: number
    public UpTimeMinutes: number
    public UpTimeSeconds: number

    constructor(performanceInformationModel: any)
    {
        let timeSpan: number
        let secPerMinute = 60;
        let secPerHour = secPerMinute * 60;
        let dateTime : Date

        this.LateBy = performanceInformationModel.LateBy
        this.CycleTime = performanceInformationModel.CycleTime
        this.TimerDrift = performanceInformationModel.TimerDrift
        this.CpuTime = performanceInformationModel.CycleTime

        this.TimerDriftMicroseconds = Math.abs(this.TimerDrift) / 1000

        this.ConsumedDiskSpace = (performanceInformationModel.TotalDiskSize - performanceInformationModel.FreeDiskSpace) / 1024 / 1024 / 1024
        this.TotalDiskSize = performanceInformationModel.TotalDiskSize / 1024 / 1024 / 1024

        // get current time
        dateTime = new Date(performanceInformationModel.DateTime);

        this.Time = dateTime.toLocaleString(
            "de-DE",
            {
                timeZone: "UTC",
                hour: "2-digit",
                minute: "2-digit",
                second: "2-digit"
            }
        )

        // calculate system uptime
        timeSpan = performanceInformationModel.UpTime

        this.UpTimeHours = Math.floor(timeSpan / secPerHour)
        timeSpan -= this.UpTimeHours * secPerHour

        this.UpTimeMinutes = Math.floor(timeSpan / secPerMinute)
        timeSpan -= this.UpTimeMinutes * secPerMinute

        this.UpTimeSeconds = timeSpan
    }
}