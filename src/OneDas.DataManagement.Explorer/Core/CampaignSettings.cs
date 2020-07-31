using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;

namespace OneDas.DataManagement.Explorer.Core
{
    public class CampaignSettings
    {
        public CampaignSettings(SparseCampaignInfo campaign,
                                DataReaderExtensionBase nativeDataReader,
                                DataReaderExtensionBase aggregationDataReader)
        {
            this.Campaign = campaign;
            this.NativeDataReader = nativeDataReader;
            this.AggregationDataReader = aggregationDataReader;
        }


        public SparseCampaignInfo Campaign { get; }

        public DataReaderExtensionBase NativeDataReader { get; }

        public DataReaderExtensionBase AggregationDataReader { get; }
    }
}
