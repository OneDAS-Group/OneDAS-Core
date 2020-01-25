using OneDas.Infrastructure;

namespace OneDas.Hdf.Explorer.Core
{
    public class PeriodData
    {
        public PeriodData(Period period, SampleRateContainer sampleRateContainer, int valueSize)
        {
            this.Period = period;
            this.Buffer = new double[86400 / (int)period];
            this.SampleCount = sampleRateContainer.SamplesPerSecond * (ulong)period;
            this.ByteCount = this.SampleCount * (ulong)valueSize;
        }

        public Period Period { get; }

        public long DatasetId { get; set; }

        public double[] Buffer { get; }

        public int BufferPosition { get; set; }

        public ulong SampleCount { get; }

        public ulong ByteCount { get; }
    }
}
