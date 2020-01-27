using OneDas.Infrastructure;

namespace OneDas.Hdf.Explorer.Core
{
    public class AggregationPeriodData
    {
        public AggregationPeriodData(Period period, SampleRateContainer sampleRateContainer, int valueSize)
        {
            this.SampleCount = sampleRateContainer.SamplesPerSecond * (ulong)period;
            this.ByteCount = this.SampleCount * (ulong)valueSize;
        }

        public ulong SampleCount { get; }

        public ulong ByteCount { get; }
    }
}
