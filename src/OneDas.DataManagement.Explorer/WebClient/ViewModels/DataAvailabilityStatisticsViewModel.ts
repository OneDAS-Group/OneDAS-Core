class DataAvailabilityStatisticsViewModel
{
    public readonly Granularity: DataAvailabilityGranularityEnum
    public readonly Data: number[]

    constructor(dataAvailabilityStatisticsModel: any)
    {
        this.Granularity = dataAvailabilityStatisticsModel.Granularity
        this.Data = dataAvailabilityStatisticsModel.Data
    }
}