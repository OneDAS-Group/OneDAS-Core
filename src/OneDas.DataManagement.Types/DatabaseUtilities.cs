using OneDas.Data;
using OneDas.DataManagement.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OneDas.DataManagement
{
    public static class DatabaseUtilities
    {
        public static void Update()
        {
            var (database, idToDataReaderMap) = DatabaseUtilities.Load();

            foreach (var dataReader in idToDataReaderMap.Select(entry => entry.Value))
            {
                try
                {
                    var campaignNames = dataReader.GetCampaignNames();

                    foreach (var campaignName in campaignNames)
                    {
                        var container = database.CampaignContainers.FirstOrDefault(container => container.Name == campaignName);

                        if (container == null)
                        {
                            container = new CampaignContainer(campaignName, dataReader.RootPath);
                            database.CampaignContainers.Add(container);
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

            DatabaseUtilities.Save(database);
        }

        public static (OneDasDatabase database, Dictionary<string, DataReaderExtensionBase> idToDataReaderMap) Load()
        {
            OneDasDatabase database;

            var filePath = Path.Combine(Environment.CurrentDirectory, "DB_META", "database.json");

            if (!File.Exists(filePath))
            {
                database = new OneDasDatabase();
            }
            else
            {
                var jsonString = File.ReadAllText(filePath);
                database = JsonSerializer.Deserialize<OneDasDatabase>(jsonString);
            }

            var idToDataReaderMap = DatabaseUtilities.LoadDataReader(database.RootPathToDataReaderIdMap);

            return (database, idToDataReaderMap);
        }

        private static void Save(OneDasDatabase database)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "DB_META", "database.json");
            var jsonString = JsonSerializer.Serialize(database);

            File.WriteAllText(filePath, jsonString);
        }

        private static Dictionary<string, DataReaderExtensionBase> LoadDataReader(Dictionary<string, string> rootPathToDataReaderIdMap)
        {
            return new Dictionary<string, DataReaderExtensionBase>
            {
                //[@":meta:"] = new HdfDataReader(Environment.CurrentDirectory),
                //[@"D:\DATA\DB_MDAS"] = new HdfDataReader(@"D:\DATA\DB_MDAS"),
                //[@":memory:"] = new InMemoryDataReader(":memory:")
            };
        }
    }
}
