namespace OneDas.DataManagement
{
    public class DataAvailabilityStatistics
    {
        public DataAvailabilityStatistics(DataAvailabilityGranularity granularity, int[] data)
        {
            this.Granularity = granularity;
            this.Data = data;
        }

        public DataAvailabilityGranularity Granularity { get; private set; }

        public int[] Data { get; private set; }
    }
}
