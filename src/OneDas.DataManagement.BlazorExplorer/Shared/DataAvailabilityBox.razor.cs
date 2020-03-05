using ChartJs.Blazor.ChartJS.BarChart;
using ChartJs.Blazor.ChartJS.BarChart.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes.Ticks;
using ChartJs.Blazor.ChartJS.Common.Enums;
using ChartJs.Blazor.ChartJS.Common.Handlers;
using ChartJs.Blazor.ChartJS.Common.Properties;
using ChartJs.Blazor.ChartJS.Common.Time;
using OneDas.DataManagement.BlazorExplorer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class DataAvailabilityBox
    {
        #region Constructors

        public DataAvailabilityBox()
        {
            this.Config = new BarConfig
            {
                Options = new BarOptions
                {
                    Legend = new Legend
                    {
                        Display = false
                    },
                    MaintainAspectRatio = false,
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new BarTimeAxis
                            {
                                BarPercentage = 0.9,
                                Offset = true,
                                Time = new TimeOptions
                                {
                                    Unit = TimeMeasurement.Day
                                },
                                Ticks = new TimeTicks
                                {
                                    FontColor = "var (--font-color)",
                                    FontSize = 15
                                }
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                Position = Position.Left,
                                ScaleLabel = new ScaleLabel
                                {
                                    Display = false
                                },
                                Ticks = new LinearCartesianTicks
                                {
                                    Max = 100,
                                    Min = 0,
                                    BeginAtZero = true,
                                    FontColor = "var (--font-color)",
                                    FontSize = 15
                                }
                            }
                        }
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = false
                    }
                }
            };
        }

        #endregion

        #region Properties

        private BarConfig Config { get; set; }

        private BarDataset<TimeTuple<double>> Dataset { get; set; }

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            this.PropertyChanged = async (sender, e) =>
            {
                await this.UpdateChart();

                if (e.PropertyName == nameof(AppStateViewModel.DateTimeBegin))
                {
                    await this.InvokeAsync(() => { this.StateHasChanged(); });
                }
                else if (e.PropertyName == nameof(AppStateViewModel.DateTimeEnd))
                {
                    await this.InvokeAsync(() => { this.StateHasChanged(); });
                }
                else if (e.PropertyName == nameof(AppStateViewModel.CampaignContainer))
                {
                    await this.InvokeAsync(() => { this.StateHasChanged(); });
                }
            };

            this.Dataset = new BarDataset<TimeTuple<double>>
            {
                BackgroundColor = "rgba(136, 14, 79, 0.2)",
                BorderColor = "rgba(136, 14, 79)",
                BorderWidth = 2
            };

            this.Config.Data.Datasets.Clear();
            this.Config.Data.Datasets.Add(this.Dataset);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.UpdateChart();
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task UpdateChart()
        {
            var axis = (BarTimeAxis)barChart.Config.Options.Scales.XAxes[0];
            var statistics = await this.AppState.GetDataAvailabilityStatisticsAsync();

            this.Dataset.RemoveRange(0, this.Dataset.Data.Count);

            switch (statistics.Granularity)
            {
                case DataAvailabilityGranularity.DayLevel:

                    axis.Time.Unit = TimeMeasurement.Day;
                    var dateTimeBegin1 = this.AppState.DateTimeBegin.Date;

                    this.Dataset.AddRange(statistics.Data
                        .Select((value, i) =>
                        {
                            return new TimeTuple<double>((Moment)dateTimeBegin1.AddDays(i), value);
                        })
                    );

                    break;

                case DataAvailabilityGranularity.MonthLevel:

                    axis.Time.Unit = TimeMeasurement.Month;
                    var dateTimeBegin2 = this.AppState.DateTimeBegin.Date;

                    this.Dataset.AddRange(statistics.Data
                        .Select((value, i) =>
                        {
                            return new TimeTuple<double>((Moment)dateTimeBegin2.AddMonths(i), value);
                        })
                    );

                    break;

                default:
                    break;
            }

            await barChart.Update();
        }

        #endregion
    }
}
