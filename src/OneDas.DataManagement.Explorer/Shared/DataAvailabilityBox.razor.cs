using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Common.Time;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Explorer.ViewModels;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
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

        [Inject]
        public ToasterService ToasterService { get; set; }

        private BarConfig Config { get; set; }

        private BarDataset<TimePoint> Dataset { get; set; }

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            this.PropertyChanged = async (sender, e) =>
            {
                if (e.PropertyName == nameof(UserState.DateTimeBegin))
                {
                    await this.UpdateChart();
                }
                else if (e.PropertyName == nameof(UserState.DateTimeEnd))
                {
                    await this.UpdateChart();
                }
                else if (e.PropertyName == nameof(UserState.ProjectContainer))
                {
                    await this.UpdateChart();
                }
            };

            this.Dataset = new BarDataset<TimePoint>
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
            if (firstRender)
                await this.UpdateChart();

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task UpdateChart()
        {
            if (this.UserState.DateTimeBegin > this.UserState.DateTimeEnd)
                return;

            var axis = (BarTimeAxis)((BarConfig)_barChart.Config).Options.Scales.XAxes[0];

            try
            {
                var statistics = await this.UserState.GetDataAvailabilityStatisticsAsync();

                this.Dataset.Clear();

                switch (statistics.Granularity)
                {
                    case DataAvailabilityGranularity.DayLevel:

                        axis.Time.Unit = TimeMeasurement.Day;
                        var dateTimeBegin1 = this.UserState.DateTimeBegin.Date;

                        this.Dataset.AddRange(statistics.Data
                            .Select((value, i) =>
                            {
                                return new TimePoint(dateTimeBegin1.AddDays(i), value);
                            })
                        );

                        break;

                    case DataAvailabilityGranularity.MonthLevel:

                        axis.Time.Unit = TimeMeasurement.Month;
                        var dateTimeBegin2 = this.UserState.DateTimeBegin.Date;

                        this.Dataset.AddRange(statistics.Data
                            .Select((value, i) =>
                            {
                                return new TimePoint(dateTimeBegin2.AddMonths(i), value);
                            })
                        );

                        break;

                    default:
                        break;
                }

                await _barChart.Update();
            }
            catch (TaskCanceledException)
            {
                // prevent that the whole app crashes in the followig case:
                // - OneDAS calculates aggregations and locks current file
                // GUI wants to load data from that locked file and times out
                // TaskCanceledException is thrown: app crashes.
                this.UserState.ClientState = ClientState.Normal;
            }
            catch (Exception ex)
            {
                this.UserState.Logger.LogError(ex.GetFullMessage());
                this.ToasterService.ShowError(message: "Unable to load availability data.", icon: MatIconNames.Error_outline);
            }
        }

        #endregion
    }
}
