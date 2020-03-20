using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using OneDas.Infrastructure;
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

        private CampaignInfo _campaign_allowed;
        private CampaignInfo _campaign_restricted;

        #endregion

        #region Constructors

        public InMemoryDataReader(string rootPath) : base(rootPath)
        {
            var id11 = "7dec6d79-b92e-4af2-9358-21be1f3626c9";
            var id12 = "cf50190b-fd2a-477b-9655-48f4f41ba7bf";
            var id13 = "f01b6a96-1de6-4caa-9205-184d8a3eb2f8";
            _campaign_allowed = this.InitializeCampaign("/IN_MEMORY/ALLOWED/TEST", id11, id12, id13);

            var id21 = "511d6e9c-9075-41ee-bac7-891d359f0dda";
            var id22 = "99b85689-5373-4a9a-8fd7-be04a89c9da8";
            var id23 = "50d38fe5-a7a8-49e8-8bd4-3e98a48a951f";
            _campaign_restricted = this.InitializeCampaign("/IN_MEMORY/RESTRICTED/TEST", id21, id22, id23);
        }

        #endregion

        #region Methods

        public override List<string> GetCampaignNames()
        {
            return new List<string> { "/IN_MEMORY/ALLOWED/TEST", "/IN_MEMORY/RESTRICTED/TEST" };
        }

        public override CampaignInfo GetCampaign(string campaignName)
        {
            if (campaignName == _campaign_allowed.Id)
                return _campaign_allowed;
            else if (campaignName == _campaign_restricted.Id)
                return _campaign_restricted;
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

        protected override (T[] dataset, byte[] statusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            double[] dataDouble;

            var beginTime = begin.ToUnixTimeStamp();
            var endTime = end.ToUnixTimeStamp();

            var length = (int)((end - begin).TotalSeconds * (double)dataset.GetSampleRate().SamplesPerSecond);
            var dt = (double)(1 / dataset.GetSampleRate().SamplesPerSecond);

            if (dataset.Parent.Id.Contains("unix_time"))
            {
                dataDouble = Enumerable.Range(0, length).Select(i => i * dt + beginTime).ToArray();
            }
            else // temperature
            {
                var kernelSize = 1000;
                var movingAverage = new double[kernelSize];
                var random = new Random();
                var mean = 15;

                dataDouble = new double[length];

                for (int i = 0; i < length; i++)
                {
                    movingAverage[i % kernelSize] = (random.NextDouble() - 0.5) * mean * 10 + mean;

                    if (movingAverage[kernelSize-1] == 0)
                        dataDouble[i] = mean;
                    else
                        dataDouble[i] = movingAverage.Sum() / kernelSize;
                }
            }

            var data = dataDouble.Select(value => (T)Convert.ChangeType(value, typeof(T))).ToArray();
            var statusSet = Enumerable.Range(0, length).Select(value => (byte)1).ToArray();

            return (data, statusSet);
        }

        private CampaignInfo InitializeCampaign(string campaignName, string id1, string id2, string id3)
        {
            var campaign = new CampaignInfo(campaignName);

            var variableA = new VariableInfo(id1, campaign);
            var variableB = new VariableInfo(id2, campaign);
            var variableC = new VariableInfo(id3, campaign);

            var dataset1 = new DatasetInfo("25 Hz", variableA) { DataType = OneDasDataType.INT32 };
            var dataset2 = new DatasetInfo("1 s_max", variableB) { DataType = OneDasDataType.FLOAT64 };
            var dataset3 = new DatasetInfo("1 s_mean", variableB) { DataType = OneDasDataType.FLOAT64 };
            var dataset4 = new DatasetInfo("1 s", variableC) { DataType = OneDasDataType.FLOAT64 };

            // variable 1
            variableA.Datasets = new List<DatasetInfo>()
            {
                dataset1
            };

            variableA.VariableNames.Add("unix_time1");
            variableA.VariableGroups.Add("Group 1");
            variableA.Units.Add("");

            // variable 2
            variableB.Datasets = new List<DatasetInfo>()
            {
                dataset2,
                dataset3
            };

            variableB.VariableNames.Add("unix_time2");
            variableB.VariableGroups.Add("Group 1");
            variableB.Units.Add("");

            // variable 3
            variableC.Datasets = new List<DatasetInfo>()
            {
                dataset4
            };

            variableC.VariableNames.Add("T");
            variableC.VariableGroups.Add("Group 2");
            variableC.Units.Add("°C");

            // campaign
            campaign.Variables = new List<VariableInfo>()
            {
                variableA,
                variableB,
                variableC
            };

            return campaign;
        }

        #endregion
    }
}
