using OneDas.DataManagement.Database;

namespace OneDas.Hdf.VdsTool
{
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
}
