using OneDas.DataManagement.Database;
using OneDas.Infrastructure;

namespace OneDas.Hdf.VdsTool
{
    public class AggregationSetup
    {
        public AggregationSetup(AggregationConfig config, Period period)
        {
            this.Config = config;
            this.Period = period;
        }

        public AggregationConfig Config { get; }

        public Period Period { get; }
    }
}
