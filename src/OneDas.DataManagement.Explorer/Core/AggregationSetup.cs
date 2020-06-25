using OneDas.DataManagement.Database;

namespace OneDas.DataManagement.Explorer.Core
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
