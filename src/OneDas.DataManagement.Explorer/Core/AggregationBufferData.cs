using OneDas.Infrastructure;

namespace OneDas.Hdf.Explorer.Core
{
    public class AggregationBufferData
    {
        public AggregationBufferData(Period period)
        {
            this.Buffer = new double[86400 / (int)period];
        }

        public double[] Buffer { get; }

        public int BufferPosition { get; set; }

        public long DatasetId { get; set; }
    }
}
