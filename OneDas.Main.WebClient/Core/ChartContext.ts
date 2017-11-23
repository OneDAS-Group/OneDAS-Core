class ChartContext
{
    public ChannelHub: ChannelHubViewModel
    public Chart: Chart
    public ValueSet: Chart.ChartPoint[]

    constructor(channelHub: ChannelHubViewModel, chart: Chart, valueSet: Chart.ChartPoint[])
    {
        this.ChannelHub = channelHub
        this.Chart = chart
        this.ValueSet = valueSet
    }
}