using OneDas.Database;
using OneDas.DataStorage;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Extensibility
{
    public class TestDatabase : IDatabase
    {
        #region Constructors

        public TestDatabase()
        {
            //
        }

        #endregion

        #region Methods

        public List<CampaignInfo> GetCampaignInfos()
        {
            var campaignInfo = new CampaignInfo("/ANY_EXTERNAL_DATABASE/TEST/TEST", null);

            var variableInfo1 = new VariableInfo(Guid.NewGuid().ToString(), campaignInfo);
            var variableInfo2 = new VariableInfo(Guid.NewGuid().ToString(), campaignInfo);

            var dataset1 = new DatasetInfo("25 Hz", variableInfo1);
            var dataset2 = new DatasetInfo("1 s_mean", variableInfo1);
            var dataset3 = new DatasetInfo("25 Hz", variableInfo2);

            variableInfo1.DatasetInfos = new List<DatasetInfo>()
            {
                dataset1,
                dataset2
            };

            variableInfo1.VariableNames.Add("A");
            variableInfo1.VariableGroups.Add("Group A");

            variableInfo2.DatasetInfos = new List<DatasetInfo>()
            {
                dataset3
            };

            variableInfo2.VariableNames.Add("B");
            variableInfo2.VariableGroups.Add("Group A");

            campaignInfo.VariableInfos = new List<VariableInfo>()
            {
                variableInfo1,
                variableInfo2
            };

            return new List<CampaignInfo>()
            {
                campaignInfo
            };
        }

        public DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            throw new NotImplementedException();
        }

        public ulong GetElementSize(string campaignName, string variableName, string datasetName)
        {
            throw new NotImplementedException();
        }

        public List<VariableDescription> GetVariableDescriptions(KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo)
        {
            throw new NotImplementedException();
        }

        public ISimpleDataStorage LoadDataset(string datasetPath, ulong start, ulong block)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataReaderExtensionBase GetDataReader(string campaignName, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
