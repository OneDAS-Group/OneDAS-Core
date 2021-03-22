using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    public record AggregationSetup
    {
        /// <example>2020-02-01T00:00:00Z</example>
        public DateTime Begin { get; set; } = DateTime.UtcNow.Date.AddDays(-2);

        /// <example>2020-02-02T00:00:00Z</example>
        public DateTime End { get; set; } = DateTime.UtcNow.Date.AddDays(-1);

        /// <example>false</example>
        public bool Force { get; set; } = false;

        public List<ReaderConfiguration> ReaderConfigurations { get; set; } = new List<ReaderConfiguration>();

        public List<Aggregation> Aggregations { get; set; } = new List<Aggregation>();
    }

    public record ReaderConfiguration
    {
        /// <example>/IN_MEMORY/TEST/ACCESSIBLE</example>
        public string ProjectId { get; set; } = string.Empty;

        /// <example>OneDas.InMemory</example>
        public string DataReaderId { get; set; } = string.Empty;

        /// <example>:memory:</example>
        public string DataReaderRootPath { get; set; } = string.Empty;

        /// <example>{ "myParameter1": "myParameterValue1", "myParameter2": "myParameterValue2" }</example>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public record Aggregation
    {
        /// <example>/IN_MEMORY/TEST/ACCESSIBLE</example>
        public string ProjectId { get; set; } = string.Empty;

        /// <example>{ Mean: null, MeanPolar: "360" }</example>
        public Dictionary<AggregationMethod, string> Methods { get; set; } = new Dictionary<AggregationMethod, string>();

        /// <example>{ "IncludeGroup": "GroupA|GroupB", "ExcludeUnit": "deg", "IncludeChannels": "T1" }</example>
        public Dictionary<AggregationFilter, string> Filters { get; set; } = new Dictionary<AggregationFilter, string>();

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

    public enum AggregationFilter
    {
        IncludeChannel = 0,
        ExcludeChannel = 1,
        IncludeGroup = 2,
        ExcludeGroup = 3,
        IncludeUnit = 4,
        ExcludeUnit = 5
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

    public record AggregationUnit
    {
        public Aggregation Aggregation { get; init; }

        public int Period { get; init; }

        public AggregationMethod Method { get; init; }

        public string Argument { get; init; }

        public AggregationTargetBufferInfo TargetBufferInfo { get; init; }
    }

    public record AggregationInstruction(ProjectContainer Container, Dictionary<DataReaderRegistration, List<AggregationChannel>> DataReaderToAggregationsMap);
}
