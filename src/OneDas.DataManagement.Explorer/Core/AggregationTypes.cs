using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
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
        public bool Force { get; set; }

        public List<Aggregation> Aggregations { get; set; }
    }

    public record Aggregation
    {
        /// <example>/IN_MEMORY/TEST/ACCESSIBLE</example>
        public string ProjectId { get; set; } = string.Empty;

        /// <example>Mean</example>
        public AggregationMethod Method { get; set; } = AggregationMethod.Mean;

        /// <example>depends on method</example>
        public string Argument { get; set; } = string.Empty;

        /// <example>{ "--include-group": "GroupA|GroupB", "--exclude-unit": "deg", "--include-channel": "T1" }</example>
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
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

    public class AggregationPeriodData
    {
        public AggregationPeriodData(AggregationPeriod period, SampleRateContainer sampleRateContainer, int valueSize)
        {
            this.SampleCount = sampleRateContainer.SamplesPerSecondAsUInt64 * (ulong)period;
            this.ByteCount = this.SampleCount * (ulong)valueSize;
        }

        public ulong SampleCount { get; }

        public ulong ByteCount { get; }
    }

    public class AggregationBufferData
    {
        public AggregationBufferData(AggregationPeriod period)
        {
            this.Buffer = new double[86400 / (int)period];
        }

        public double[] Buffer { get; }

        public int BufferPosition { get; set; }

        public long DatasetId { get; set; }
    }

    public class AggregationSetup
    {
        public AggregationSetup(Aggregation aggregation, AggregationPeriod period)
        {
            this.Aggregation = aggregation;
            this.Period = period;
        }

        public Aggregation Aggregation { get; }

        public AggregationPeriod Period { get; }
    }
}
