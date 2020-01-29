using OneDas.DataManagement.DataReader;
using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabaseManager
    {
        #region Fields

        private Dictionary<string, DataReaderExtensionBase> _idToDataReaderMap;

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

            _idToDataReaderMap = this.LoadDataReader(this.Database.RootPathToDataReaderIdMap);
        }

        #endregion

        #region Properties

        public OneDasDatabase Database { get; }

        #endregion

        #region Methods

        public void Update()
        {
            foreach (var dataReader in _idToDataReaderMap.Select(entry => entry.Value))
            {
                try
                {
                    var campaignNames = dataReader.GetCampaignNames();

                    foreach (var campaignName in campaignNames)
                    {
                        var container = this.Database.CampaignContainers.FirstOrDefault(container => container.Name == campaignName);

                        if (container == null)
                        {
                            container = new CampaignContainer(campaignName, dataReader.RootPath);
                            this.Database.CampaignContainers.Add(container);
                        }
                        else
                        {
                            if (container.RootPath != dataReader.RootPath)      
                                throw new Exception("The campaign data root path stored in the database does not match the provided root path of the data reader extension.");
                        }

                        var campaign = dataReader.GetCampaign(campaignName, container.LastScan);
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

        public DataReaderExtensionBase GetDataReader(string campaignName)
        {
            var container = this.Database.CampaignContainers.FirstOrDefault(container => container.Name == campaignName);

            if (container == null)
                throw new KeyNotFoundException("The requested campaign could not be found.");

            if (!this.Database.RootPathToDataReaderIdMap.TryGetValue(container.RootPath, out var id))
                throw new KeyNotFoundException("The requested root path could not be found.");

            if (!_idToDataReaderMap.TryGetValue(id, out var dataReader))
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
                [@":meta:"] = new HdfDataReader(Environment.CurrentDirectory),
                [@"D:\DATA\DB_MDAS"] = new HdfDataReader(@"D:\DATA\DB_MDAS"),
                //[@":memory:"] = new InMemoryDataReader(":memory:")
            };
        }

        #endregion
    }
}
