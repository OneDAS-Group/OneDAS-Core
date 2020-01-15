using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using System;
using System.Collections.Generic;

namespace OneDas.Hdf.Explorer.Core
{
    public interface IDatabase : IDisposable
    {
        ulong GetElementSize(string campaignName, string variableName, string datasetName);
        List<VariableDescription> GetVariableDescriptions(KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo);
        ISimpleDataStorage LoadDataset(string datasetPath, ulong start, ulong block);
        (string name, string guid, string unit, List<hdf_transfer_function_t>) GetDocumentation(CampaignInfo campaignInfo, VariableInfo variableInfo);
        DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd);
    }
}
