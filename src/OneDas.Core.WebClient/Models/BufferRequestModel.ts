class BufferRequestModel
{
    public SampleRate: SampleRateEnum
    public GroupFilter: string

    constructor(sampleRate: SampleRateEnum, groupFilter: string)
    {
        this.SampleRate = sampleRate;
        this.GroupFilter = groupFilter;
    }
}