using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.InMemory", "OneDAS in-memory", "Provides an in-memory database.", "", "")]
    public class InMemoryDataReader : DataReaderExtensionBase
    {
        #region Fields

        private CampaignInfo _campaign;

        #endregion

        #region Constructors

        public InMemoryDataReader(string rootPath) : base(rootPath)
        {
            this.InitializeCampaign();
        }

        #endregion

        #region Methods

        public override List<string> GetCampaignNames()
        {
            return new List<string> { "/ANY_EXTERNAL_DATABASE/TEST/TEST" };
        }

        public override CampaignInfo GetCampaign(string campaignName)
        {
            if (campaignName == _campaign.Name)
                return _campaign;
            else
                throw new Exception($"The requested campaign with name '{campaignName}' could not be found.");
        }

        public override bool IsDataOfDayAvailable(string campaignName, DateTime dateTime)
        {
            return true;
        }

        public override DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime begin, DateTime end)
        {
            int[] aggregatedData = default;
            DataAvailabilityGranularity granularity = default;

            var random = new Random();
            var totalDays = (int)Math.Round((end - begin).TotalDays);

            if (totalDays <= 365)
            {
                granularity = DataAvailabilityGranularity.DayLevel;
                aggregatedData = Enumerable.Range(0, totalDays).Select(value => random.Next(90, 100)).ToArray();
            }
            else
            {
                var totalMonths = (end.Month - begin.Month) + 1 + 12 * (end.Year - begin.Year);

                granularity = DataAvailabilityGranularity.MonthLevel;
                aggregatedData = Enumerable.Range(0, totalMonths).Select(value => random.Next(90, 100)).ToArray();
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(totalDays*2));

            return new DataAvailabilityStatistics(granularity, aggregatedData);
        }

        public override void Dispose()
        {
            //
        }

        protected override (T[] dataset, byte[] statusSet) Read<T>(DatasetInfo dataset, ulong start, ulong length)
        {
            throw new NotImplementedException();
        }

        private void InitializeCampaign()
        {
            _campaign = new CampaignInfo("/ANY_EXTERNAL_DATABASE/TEST/TEST");

            var variable1 = new VariableInfo(Guid.NewGuid().ToString(), _campaign);
            var variable2 = new VariableInfo(Guid.NewGuid().ToString(), _campaign);
            var variable3 = new VariableInfo(Guid.NewGuid().ToString(), _campaign);

            var dataset1 = new DatasetInfo("25 Hz", variable1) { DataType = Infrastructure.OneDasDataType.INT32 };
            var dataset2 = new DatasetInfo("25 Hz", variable2) { DataType = Infrastructure.OneDasDataType.INT32 };
            var dataset3 = new DatasetInfo("1 s_mean", variable2) { DataType = Infrastructure.OneDasDataType.INT32 };
            var dataset4 = new DatasetInfo("25 Hz", variable3) { DataType = Infrastructure.OneDasDataType.INT32 };

            // variable 1
            variable1.Datasets = new List<DatasetInfo>()
            {
                dataset1
            };

            variable1.VariableNames.Add("varA");
            variable1.VariableGroups.Add("Group 1");
            variable1.Units.Add("°C");

            // variable 2
            variable2.Datasets = new List<DatasetInfo>()
            {
                dataset2,
                dataset3
            };

            variable2.VariableNames.Add("varB");
            variable2.VariableGroups.Add("Group 1");
            variable2.Units.Add("m/s");

            // variable 3
            variable3.Datasets = new List<DatasetInfo>()
            {
                dataset4
            };

            variable3.VariableNames.Add("varC");
            variable3.VariableGroups.Add("Group 2");
            variable3.Units.Add("°C");

            _campaign.Variables = new List<VariableInfo>()
            {
                variable1,
                variable2,
                variable3
            };
        }

        #endregion
    }
}
