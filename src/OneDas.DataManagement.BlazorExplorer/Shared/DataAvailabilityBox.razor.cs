using ChartJs.Blazor.ChartJS.Common;
using ChartJs.Blazor.ChartJS.Common.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes.Ticks;
using ChartJs.Blazor.ChartJS.Common.Enums;
using ChartJs.Blazor.ChartJS.Common.Handlers;
using ChartJs.Blazor.ChartJS.Common.Properties;
using ChartJs.Blazor.ChartJS.LineChart;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Shared
{
    public partial class DataAvailabilityBox
    {
        #region Properties

        private LineConfig Config { get; set; }

        #endregion

        #region Constructors

        public DataAvailabilityBox()
        {
            this.Config = new LineConfig
            {
                Options = new LineOptions
                {
                    Animation = new Animation()
                    {
                        Duration = 0
                    },
                    Hover = new LineOptionsHover()
                    {
                        AnimationDuration = 0
                    },
                    Legend = new Legend
                    {
                        Display = false
                    },
                    MaintainAspectRatio = false,
                    ResponsiveAnimationDuration = 0,
                    Scales = new Scales
                    {
                        xAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                Display = AxisDisplay.True,
                            }
                        },
                        yAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                Position = Position.Left,
                                ScaleLabel = new ScaleLabel
                                {
                                    Display = true,
                                    LabelString = "Availability in %",
                                    FontColor = "rgba(0, 0, 0, 0.54)",
                                    FontSize = 20
                                },
                                Ticks = new LinearCartesianTicks
                                {
                                    Max = 100,
                                    Min = 0,
                                    BeginAtZero = true,
                                    FontColor = "rgba(0, 0, 0, 0.54)",
                                    FontSize = 20
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

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            var statistics = await this.AppState.GetDataAvailabilityStatisticsAsync();

            LineDataset<Point> dataset = new LineDataset<Point>
            {
                BackgroundColor = "rgba(136, 14, 79, 0.2)",
                BorderColor = "rgba(136, 14, 79)",
                BorderWidth = 3,
                LineTension = 0.25,
                PointRadius = 0,
                Label = "Messung"
            };

            dataset.AddRange(statistics.Data
                .Select((value, i) =>
                {
                    return new Point(i, value);
                })
            );

            this.Config.Data.Datasets.Add(dataset);
            await lineChart.Update();
        }

        #endregion
    }
}
