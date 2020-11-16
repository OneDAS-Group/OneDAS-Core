using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    public record AggregationParameters
    {
        /// <example>2020-02-01T00:00:00Z</example>
        public DateTime Begin { get; set; } = DateTime.UtcNow.Date.AddDays(-2);

        /// <example>2020-02-02T00:00:00Z</example>
        public DateTime End { get; set; } = DateTime.UtcNow.Date.AddDays(-1);

        /// <example>false</example>
        public bool Force { get; set; } = false;

        /// <example>{ "/IN_MEMORY/TEST/ACCESSIBLE": { "MySetting": "MyValue" } }</example>
        public Dictionary<string, Dictionary<string, string>> ReaderParameters { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public List<Aggregation> Aggregations { get; set; } = new List<Aggregation>();
    }

    public record Aggregation
    {
        /// <example>/IN_MEMORY/TEST/ACCESSIBLE</example>
        public string ProjectId { get; set; } = string.Empty;

        /// <example>{ Mean: null, MeanPolar: "360" }</example>
        public Dictionary<AggregationMethod, string> Methods { get; set; } = new Dictionary<AggregationMethod, string>() ;

        /// <example>{ "--include-group": "GroupA|GroupB", "--exclude-unit": "deg", "--include-channel": "T1" }</example>
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();

        /// <example>[ 1, 60, 600 ]</example>
        public List<int> Periods { get; set; } = new List<int>();
    }

    public enum AggregationMethod
    {
        Mean = 0,
        MeanPolar = 1,
        Min = 2,
        Max = 3,
        Std = 4,
        Rms = 5,
        MinBitwise = 6,
        MaxBitwise = 7,
        SampleAndHold = 8,
        Sum = 9
    }

    public record AggregationChannel
    {
        public ChannelInfo Channel { get; init; }

        public List<Aggregation> Aggregations { get; init; }
    }

    public record AggregationTargetBufferInfo
    {
        public AggregationTargetBufferInfo(int period)
        {
            this.Buffer = new double[86400 / period];
        }

        public double[] Buffer { get; }

        public int BufferPosition { get; set; }

        public long DatasetId { get; set; }
    }

    public record AggregationSetup
    {
        public Aggregation Aggregation { get; init; }

        public int Period { get; init; }

        public AggregationMethod Method { get; init; }

        public string Argument { get; init; }

        public AggregationTargetBufferInfo TargetBufferInfo { get; init; }
    }
}
