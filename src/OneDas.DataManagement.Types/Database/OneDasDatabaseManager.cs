using OneDas.DataManagement.DataReader;
using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OneDas.DataManagement.Database
{
    /* 
     * Algorithm:
     * *******************************************************************************
     * 01. load database.json (this.Database)
     * 02. load and instantiate data reader extensions (_rootPathToDataReaderMap)
     * 03. call Update() method
     * 04.   for each data reader in _rootPathToDataReaderMap
     * 05.       get campaign names
     * 06.           for each campaign name
     * 07.               find campaign container in current database or create new one
     * 08.               get an up-to-date campaign instance from the data reader
     * 09.               merge both campaigns
     * 10. save updated database
     * *******************************************************************************
     */

    public class OneDasDatabaseManager
    {
        #region Fields

        private Dictionary<string, DataReaderExtensionBase> _rootPathToDataReaderMap;

        #endregion

        #region Constructors

        public OneDasDatabaseManager()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "database.json");

            if (!File.Exists(filePath))
            {
                this.Database = new OneDasDatabase();
            }
            else
            {
                var jsonString = File.ReadAllText(filePath);
                this.Database = JsonSerializer.Deserialize<OneDasDatabase>(jsonString);
                this.Database.Initialize();
            }

            _rootPathToDataReaderMap = this.LoadDataReader(this.Database.RootPathToDataReaderIdMap);
        }

        #endregion

        #region Properties

        public OneDasDatabase Database { get; }

        #endregion

        #region Methods

        public void Update()
        {
            foreach (var dataReader in _rootPathToDataReaderMap.Select(entry => entry.Value))
            {
                try
                {
                    var isNativeDataReader = dataReader != this.GetAggregationDataReader();
                    var campaignNames = dataReader.GetCampaignNames();

                    foreach (var campaignName in campaignNames)
                    {
                        // find campaign container or create a new one
                        var container = this.Database.CampaignContainers.FirstOrDefault(container => container.Name == campaignName);

                        if (container == null)
                        {
                            container = new CampaignContainer(campaignName, dataReader.RootPath);
                            this.Database.CampaignContainers.Add(container);
                        }

                        // ensure that found campaign container root path matches that of the data reader
                        if (isNativeDataReader)
                        {
                            if (dataReader.RootPath != container.RootPath) 
                                throw new Exception("The data reader root path does not match the root path of the campaign data stored in the database.");
                        }

                        // get up-to-date campaign from data reader
                        var campaign = dataReader.GetCampaign(campaignName, container.LastScan);

                        // if data reader is for aggregation data, update the dataset`s flag
                        if (!isNativeDataReader)
                        {
                            var datasets = campaign.Variables.SelectMany(variable => variable.Datasets).ToList();

                            foreach (var dataset in datasets)
                            {
                                dataset.IsNative = false;
                            }

                            campaign.ChunkDataset.IsNative = false;
                        }

                        //
                        container.Campaign.Merge(campaign);
                        container.CampaignMeta.Purge();
                        container.LastScan = DateTime.Now.Date;
                    }
                }
                finally
                {
                    dataReader.Dispose();
                }
            }

            this.Save(this.Database);
        }

        public DataReaderExtensionBase GetNativeDataReader(string campaignName)
        {
            var container = this.Database.CampaignContainers.FirstOrDefault(container => container.Name == campaignName);

            if (container == null)
                throw new KeyNotFoundException("The requested campaign could not be found.");

            if (!_rootPathToDataReaderMap.TryGetValue(container.RootPath, out var dataReader))
                throw new KeyNotFoundException("The requested data reader could not be found.");

            return dataReader;
        }

        public DataReaderExtensionBase GetAggregationDataReader()
        {
            if (!_rootPathToDataReaderMap.TryGetValue(":root:", out var dataReader))
                throw new KeyNotFoundException("The requested data reader could not be found.");

            return dataReader;
        }

        public List<CampaignInfo> GetCampaigns()
        {
            return this.Database.CampaignContainers.Select(container => container.Campaign).ToList();
        }

        private void Save(OneDasDatabase database)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "database.json");
            var jsonString = JsonSerializer.Serialize(database, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(filePath, jsonString);
        }

        private Dictionary<string, DataReaderExtensionBase> LoadDataReader(Dictionary<string, string> rootPathToDataReaderIdMap)
        {
            return new Dictionary<string, DataReaderExtensionBase>
            {
                [@":root:"] = new HdfDataReader(Environment.CurrentDirectory),
                [@"D:\DATA\DB_MDAS"] = new HdfDataReader(@"D:\DATA\DB_MDAS"),
                //[@":memory:"] = new InMemoryDataReader(":memory:")
            };
        }

        #endregion
    }
}
