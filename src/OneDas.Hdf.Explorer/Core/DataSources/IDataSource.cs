using OneDas.Database;
using OneDas.DataStorage;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Explorer.Core
{
#warning IDisposable is required to avoid that VDS.h5 file is always in use, i.e. it cannot be update over night
    public interface IDataSource : IDisposable
    {
        List<CampaignInfo> GetCampaignInfos();

        ulong GetElementSize(string campaignName, string variableName, string datasetName);

        List<VariableDescription> GetVariableDescriptions(KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo);

        ISimpleDataStorage LoadDataset(string datasetPath, ulong start, ulong block);

        DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd);

        void Open();

        void Close();
    }
}
