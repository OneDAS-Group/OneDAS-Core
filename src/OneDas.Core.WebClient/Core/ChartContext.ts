class ChartContext
{
    public ChannelHub: ChannelHubViewModel
    public Chart: Chart
    public Canvas: HTMLCanvasElement
    public ValueSet: Chart.ChartPoint[]

    public readonly ChartHeight: KnockoutObservable<number>

    constructor(channelHub: ChannelHubViewModel, chart: Chart, canvas: HTMLCanvasElement, valueSet: Chart.ChartPoint[])
    {
        this.ChannelHub = channelHub
        this.Chart = chart
        this.Canvas = canvas;
        this.ValueSet = valueSet

        this.ChartHeight = ko.observable<number>(150)
    }

    // commands
    public ToggleYAxisLimits = (channelHub: ChannelHubViewModel) =>
    {
        this.Chart.config.options.scales.yAxes.forEach(yAxis =>
        {
            (<any>(yAxis.ticks)).beginAtZero = !(<any>(yAxis.ticks)).beginAtZero
        })

        this.Chart.update()
    }

    public ToggleChartHeight = (channelHub: ChannelHubViewModel) =>
    {
        if (this.ChartHeight() === 150)
        {
            this.ChartHeight(300);
        }
        else
        {
            this.ChartHeight(150);
        }

        this.Chart.resize()
    }

    public InsertCanvas = (domElement: HTMLElement, valueAccessor, allBindingsAccessor, chartContext: ChartContext, bindingContext) =>
    {
        domElement.appendChild<HTMLCanvasElement>(chartContext.Canvas)
    }
}