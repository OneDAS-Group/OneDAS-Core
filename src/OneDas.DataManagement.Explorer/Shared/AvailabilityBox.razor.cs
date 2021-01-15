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
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class AvailabilityBox
    {
        #region Fields

        private string[] _backgroundColors;
        private string[] _borderColors;

        #endregion

        #region Constructors

        public AvailabilityBox()
        {
            // https://developer.mozilla.org/de/docs/Web/CSS/CSS_Colors/farbauswahl_werkzeug
            _backgroundColors = new string[]
            {
                "rgba(136, 14, 79, 0.2)",
                "rgba(14, 75, 133, 0.2)",
                "rgba(14, 133, 131, 0.2)"
            };

            _borderColors = new string[]
            {
                "rgba(136, 14, 79)",
                "rgba(14, 75, 133)",
                "rgba(14, 133, 131)"
            };

            this.Config = new BarConfig
            {
                Options = new BarOptions
                {
                    Legend = new Legend
                    {
                        Display = true
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
        public DatabaseManager DatabaseManager { get; set; }

        [Inject]
        public ToasterService ToasterService { get; set; }

        private BarConfig Config { get; set; }

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
            var totalDays = (int)(this.UserState.DateTimeEnd.Date - this.UserState.DateTimeBegin.Date).TotalDays;

            if (totalDays < 0)
                return;

            var axis = (BarTimeAxis)((BarConfig)_barChart.Config).Options.Scales.XAxes[0];

            try
            {
                var granularity = totalDays <= 365
                    ? AvailabilityGranularity.Day
                    : AvailabilityGranularity.Month;

                var availability = await this.UserState.GetAvailabilityAsync(granularity);
                var hasCleared = false;

                if (availability.Count != this.Config.Data.Datasets.Count)
                {
                    this.Config.Data.Datasets.Clear();
                    hasCleared = true;
                }

                for (int i = 0; i < availability.Count; i++)
                {
                    BarDataset<TimePoint> dataset;

                    if (hasCleared)
                    {
                        var registration = availability[i].DataReaderRegistration;
                        var isAggregation = registration.Equals(this.DatabaseManager.State.AggregationRegistration);

                        dataset = new BarDataset<TimePoint>
                        {
                            Label = isAggregation ? "Aggregations" : $"Raw ({registration.RootPath} - {registration.DataReaderId})",
                            BackgroundColor = _backgroundColors[i % _backgroundColors.Count()],
                            BorderColor = _borderColors[i % _borderColors.Count()],
                            BorderWidth = 2
                        };

                        this.Config.Data.Datasets.Add(dataset);
                    }
                    else
                    {
                        dataset = (BarDataset<TimePoint>)this.Config.Data.Datasets[i];
                        dataset.Clear();
                    }

                    switch (granularity)
                    {
                        case AvailabilityGranularity.Day:

                            axis.Time.Unit = TimeMeasurement.Day;

                            dataset.AddRange(availability[i].Data
                                .Select((entry, i) =>
                                {
                                    return new TimePoint(entry.Key, entry.Value * 100);
                                })
                            );

                            break;

                        case AvailabilityGranularity.Month:

                            axis.Time.Unit = TimeMeasurement.Month;

                            dataset.AddRange(availability[i].Data
                                .Select((entry, i) =>
                                {
                                    return new TimePoint(entry.Key, entry.Value * 100);
                                })
                            );

                            break;

                        default:
                            break;
                    }
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
