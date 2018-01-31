using System.Runtime.Serialization;

namespace OneDas.Hdf.Explorer.Core
{
    [DataContract]
    public class DataAvailabilityStatistics
    {
        public DataAvailabilityStatistics(DataAvailabilityGranularity granularity, int[] data)
        {
            this.Granularity = granularity;
            this.Data = data;
        }

        [DataMember]
        public DataAvailabilityGranularity Granularity { get; private set; }

        [DataMember]
        public int[] Data { get; private set; }
    }
}
