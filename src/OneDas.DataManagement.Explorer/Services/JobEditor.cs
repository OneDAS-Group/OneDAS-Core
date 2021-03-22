using Microsoft.Extensions.Logging.Abstractions;
using OneDas.DataManagement.Explorer.Core;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using static OneDas.DataManagement.Explorer.Services.DatabaseManager;

namespace OneDas.DataManagement.Explorer.Services
{
    public class JobEditor
    {
        #region Events

        public event EventHandler Changed;

        #endregion

        #region Fields

        private string _jsonString;
        private DatabaseManagerState _state;

        #endregion

        #region Constructors

        public JobEditor(DatabaseManager databaseManager)
        {
            _state = databaseManager.State;
            this.Update();
        }

        #endregion

        #region Properties

        public AggregationSetup AggregationSetup { get; private set; } = new AggregationSetup();

        public string Analysis { get; private set; }

        public string JsonString
        {
            get
            {
                return _jsonString;
            }
            set
            {
                _jsonString = value;

                try
                {
                    this.AggregationSetup = JsonSerializer.Deserialize<AggregationSetup>(_jsonString);

                    this.Update(skipJson: true);
                    this.OnChanged();
                }
                catch
                {
                    //
                }
            }
        }

        #endregion

        #region Methods

        public void Update(bool skipJson = false)
        {
            // analysis
            var instructions = AggregationService.ComputeInstructions(this.AggregationSetup, _state, NullLogger.Instance);
            var sb = new StringBuilder();

            foreach (var instruction in instructions)
            {
                sb.AppendLine($"Project '{instruction.Container.Id}'");

                foreach (var (registration, aggregationChannels) in instruction.DataReaderToAggregationsMap)
                {
                    if (aggregationChannels.Any())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"\tData Reader '{registration.DataReaderId}' ({registration.RootPath})");

                        foreach (var aggregationChannel in aggregationChannels)
                        {
                            sb.AppendLine();
                            sb.AppendLine($"\t\t{aggregationChannel.Channel.Name} / {aggregationChannel.Channel.Group} / {aggregationChannel.Channel.Unit}");

                            foreach (var aggregation in aggregationChannel.Aggregations)
                            {
                                foreach (var period in aggregation.Periods)
                                {
                                    sb.Append($"\t\t\tPeriod: {period} s, ");

                                    foreach (var method in aggregation.Methods)
                                    {
                                        sb.Append($" {method.Key}");
                                    }

                                    sb.AppendLine();
                                }
                            }
                        }
                    }
                }

                sb.AppendLine();
            }

            this.Analysis = sb.ToString();

            // json
            if (!skipJson)
            {
                var options = new JsonSerializerOptions() { WriteIndented = true };
                _jsonString = JsonSerializer.Serialize(this.AggregationSetup, options);
            }
        }

        private void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
