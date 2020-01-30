using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.InMemory", "OneDAS in-memory", "Provides an in-memory database.", "", "")]
    public class InMemoryDataReader : DataReaderExtensionBase
    {
        #region Constructors

        public InMemoryDataReader(string rootPath) : base(rootPath)
        {
            //
        }

        #endregion

        #region Methods

        public override List<string> GetCampaignNames()
        {
            return new List<string> { "/ANY_EXTERNAL_DATABASE/TEST/TEST" };
        }

        public override CampaignInfo GetCampaign(string campaignName, DateTime begin)
        {
            var campaignInfo = new CampaignInfo("/ANY_EXTERNAL_DATABASE/TEST/TEST");

            var variableInfo1 = new VariableInfo(Guid.NewGuid().ToString(), campaignInfo);
            var variableInfo2 = new VariableInfo(Guid.NewGuid().ToString(), campaignInfo);

            var dataset1 = new DatasetInfo("25 Hz", variableInfo1);
            var dataset2 = new DatasetInfo("1 s_mean", variableInfo1);
            var dataset3 = new DatasetInfo("25 Hz", variableInfo2);

            variableInfo1.Datasets = new List<DatasetInfo>()
            {
                dataset1,
                dataset2
            };

            variableInfo1.VariableNames.Add("A");
            variableInfo1.VariableGroups.Add("Group A");

            variableInfo2.Datasets = new List<DatasetInfo>()
            {
                dataset3
            };

            variableInfo2.VariableNames.Add("B");
            variableInfo2.VariableGroups.Add("Group A");

            campaignInfo.Variables = new List<VariableInfo>()
            {
                variableInfo1,
                variableInfo2
            };

            return campaignInfo;
        }

        public override bool IsDataOfDayAvailable(string campaignName, DateTime day)
        {
            throw new NotImplementedException();
        }

        public override DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime begin, DateTime end)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            //
        }

        protected override (T[] dataset, byte[] statusSet) Read<T>(DatasetInfo dataset, ulong start, ulong length)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
