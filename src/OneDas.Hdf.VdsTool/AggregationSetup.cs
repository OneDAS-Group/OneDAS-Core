using OneDas.DataManagement.Database;

namespace OneDas.Hdf.VdsTool
{
    public class AggregationSetup
    {
        public AggregationSetup(AggregationConfig config, AggregationPeriod period)
        {
            this.Config = config;
            this.Period = period;
        }

        public AggregationConfig Config { get; }

        public AggregationPeriod Period { get; }
    }
}
