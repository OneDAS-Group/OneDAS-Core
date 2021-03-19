using OneDas.DataManagement.Explorer.Core;
using System;
using System.Text.Json;

namespace OneDas.DataManagement.Explorer.Services
{
    public class JobEditor
    {
        public event EventHandler Changed;

        private string _jsonString;

        public JobEditor()
        {
            this.UpdateJsonString();
        }

        public AggregationSetup AggregationSetup { get; private set; } = new AggregationSetup();

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
                    this.OnChanged();
                }
                catch
                {
                    //
                }
            }
        }

        public void UpdateJsonString()
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            _jsonString = JsonSerializer.Serialize(this.AggregationSetup, options);
        }

        private void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
