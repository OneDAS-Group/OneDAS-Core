class LiveViewViewModel extends WorkspaceBase
{
    public SelectedChartContextSet: KnockoutObservableArray<ChartContext>
    public SelectedLiveViewPeriod: KnockoutObservable<LiveViewPeriodEnum>

    private _subscriptionId: number
    private _speed: number
    private _iteration: number

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('liveview', 'Live View', 'liveview.html', activeProject)

        this._speed = 200
        this._iteration = 0

        this.SelectedChartContextSet = ko.observableArray<ChartContext>()
        this.SelectedLiveViewPeriod = ko.observable<LiveViewPeriodEnum>(LiveViewPeriodEnum.Period_60)

        this.ActiveProject.subscribe(newValue =>
        {
            this.SelectedChartContextSet([])
        })

        this.SelectedLiveViewPeriod.subscribe(newValue =>
        {
            this.ReinitializeCharts(false, false)
        })

        ConnectionManager.WebClientHub.on("SendLiveViewData", (subscriptionId: number, dateTime: string, dataSnapshot: any[]) =>
        {
            let index: number

            index = 0
            this._iteration = (this._iteration + 1) % 4

            if (this._subscriptionId === subscriptionId)
            {
                this.SelectedChartContextSet().forEach(chartContext =>
                {
                    if (chartContext.ChannelHub.DataType() === OneDasDataTypeEnum.BOOLEAN)
                    {
                        //chartContext.ValueSet.push({ x: dateTime, y: dataSnapshot[index] ? 1 : 0 })
                        ///////////////////////////
                        chartContext.ValueSet.push(<any>(dataSnapshot[index] ? 1 : 0))
                    }
                    else
                    {
                        let value: any

                        //chartContext.ValueSet.push({ x: dateTime, y: chartContext.ChannelHub.GetTransformedValue(dataSnapshot[index]) })
                        ///////////////////////////

                        if (chartContext.ChannelHub.DataType() === OneDasDataTypeEnum.BOOLEAN)
                        {
                            value = dataSnapshot[index];
                        }
                        else
                        {
                            value = chartContext.ChannelHub.GetTransformedValue(dataSnapshot[index]).toFixed(2);
                        }

                        chartContext.ValueSet.push(value)

                        if (chartContext.ChannelHub.Unit()) 
                        {
                            chartContext.Chart.config.options.title.text = chartContext.ChannelHub.Name() + " (" + value + " " + chartContext.ChannelHub.Unit() + ")"
                        }
                        else
                        {
                            chartContext.Chart.config.options.title.text = chartContext.ChannelHub.Name() + " (" + value + ")"
                        }
                    }

                    index++
                    chartContext.ValueSet.shift()

                    if (this.SelectedLiveViewPeriod() <= LiveViewPeriodEnum.Period_60 || this._iteration == 0)
                    {
                        chartContext.Chart.update()
                    }
                })
            }
        })
    }

    // methods
    public async ReinitializeCharts(reuseChartSet: boolean, reuseDatasetSet: boolean)
    {
        let referenceSet: ChartContext[]
        let activeChannelHubSet: ChannelHubViewModel[]
        //let valueSet: Chart.ChartPoint[]
        let valueSet: number[] ///////////////////////////
        let context: any
        let chart: Chart
        let selectedChannelHubSet: ChannelHubViewModel[]
        let canvas: HTMLCanvasElement

        this._subscriptionId = 0 // important!

        referenceSet = this.SelectedChartContextSet()
        activeChannelHubSet = this.ActiveProject().ChannelHubSet().filter(channelHub => channelHub.AssociatedDataInput())
        selectedChannelHubSet = this.ActiveProject().ChannelHubSet().filter(channelHub => channelHub.IsSelected())

        this.SelectedChartContextSet([])

        selectedChannelHubSet.forEach(channelHub =>
        {
            let reference: ChartContext

            reference = referenceSet.find(chartContext => chartContext.ChannelHub === channelHub)

            if (!reuseChartSet && reuseDatasetSet && reference)
            {
                valueSet = <any[]>reference.ValueSet
            }
            else
            {
                valueSet = []
                valueSet.length = 1000 / this._speed * this.SelectedLiveViewPeriod()
                valueSet.fill(NaN)
            }

            if (reuseChartSet && reference)
            {
                this.SelectedChartContextSet.push(reference)
            }
            else
            {
                canvas = <HTMLCanvasElement>document.createElement("canvas");
                canvas.setAttribute("data-bind", "style: { height: ChartHeight() + 'px' }")

                chart = this.CreateChart(canvas, channelHub, <any>valueSet)

                this.SelectedChartContextSet.push(new ChartContext(channelHub, chart, canvas, <any>valueSet))
            }
        })

        this.SelectedChartContextSet().forEach(chartContext =>
        {
            chartContext.Chart.update(0)
        })

        try
        {
            this._subscriptionId = await ConnectionManager.InvokeWebClientHub("UpdateLiveViewSubscription", selectedChannelHubSet.map(channelHub => channelHub.Guid))
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public CreateChart = (context: any, channelHub: ChannelHubViewModel, valueSet: Chart.ChartPoint[]) =>
    {
        ///////////////////////////
        let labels = []
        labels.length = valueSet.length
        labels.fill(NaN)

        return new Chart(context,
            {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [{
                        data: valueSet,
                        backgroundColor: "rgba(54, 162, 235, 0.2)",
                        borderColor: "rgba(54, 162, 235)",
                        borderWidth: 1,
                        lineTension: 0.25,
                        pointRadius: 0
                    }]
                },
                options: {
                    animation: {
                        duration: this._speed * 1.5,
                        easing: "linear"
                    },
                    legend: {
                        display: false
                    },
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            //type: "time",
                            //time: {
                            //    displayFormats: {
                            //        "millisecond": "HH:mm:ss",
                            //        "second": "HH:mm:ss",
                            //        "minute": "HH:mm:ss",
                            //        "hour": "HH:mm:ss"
                            //    }
                            //},
                            ///////////////////////////
                            //ticks: {
                            //    autoSkip: true,
                            //    minRotation: 45,
                            //    maxRotation: 45,
                            //},
                            display: false
                        }],
                        yAxes: [{
                            type: "linear",
                            position: "left",
                            scaleLabel: {
                                display: true,
                                labelString: channelHub.Unit()
                            },
                            ticks: <any>{
                                beginAtZero: true
                            }
                        }]
                    },
                    title: {
                        display: true,
                        text: channelHub.Name()
                    },
                    tooltips: {
                        enabled: false
                    }
                }
            })
    }

    // commands
    public ToggleChannelHubIsSelected = (channelHub: ChannelHubViewModel) =>
    {
        channelHub.IsSelected(!channelHub.IsSelected())

        this.ReinitializeCharts(true, false)
    }
}