using OneDas.Database;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Explorer.Core
{
    public interface IDataSource : IDisposable
    {
        List<CampaignInfo> GetCampaignInfos();

        ulong GetElementSize(string campaignName, string variableName, string datasetName);

        List<VariableDescription> GetVariableDescriptions(KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo);

        ISimpleDataStorage LoadDataset(string datasetPath, ulong start, ulong block);

        DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd);

        (string name, string guid, string unit, List<TransferFunction>) GetDocumentation(CampaignInfo campaignInfo, VariableInfo variableInfo);
    }
}
