using OneDas.DataManagement.Database;
using OneDas.Infrastructure;

namespace OneDas.Hdf.VdsTool
{
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
}
