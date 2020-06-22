using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Extensions;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace OneDas.DataManagement
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

        private Dictionary<string, Type> _rootPathToDataReaderTypeMap;
        private ILoggerFactory _loggerFactory;

        #endregion

        #region Constructors

        public OneDasDatabaseManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            // config
            var filePath = Path.Combine(Environment.CurrentDirectory, "config.json");

            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                this.Config = JsonSerializer.Deserialize<OneDasDatabaseConfig>(jsonString);
            }
            else
            {
                this.Config = new OneDasDatabaseConfig();
            }

            // initalize
            this.Config.Initialize();

            // save config to disk
            this.SaveConfig(this.Config);

            // create empty database
            this.Database = new OneDasDatabase();
        }

        #endregion

        #region Properties

        public OneDasDatabase Database { get; private set; }

        public OneDasDatabaseConfig Config { get; }

        #endregion

        #region Methods

        public void Update()
        {
            var database = new OneDasDatabase();

            // load data readers
            _rootPathToDataReaderTypeMap = this.LoadDataReaders(this.Config.RootPathToDataReaderIdMap);

            // instantiate data readers
            using var aggregationDataReader = this.GetAggregationDataReader();

            var dataReaders = _rootPathToDataReaderTypeMap
                                .Select(entry => this.InstantiateDataReader(entry.Key, entry.Value))
                                .Concat(new DataReaderExtensionBase[] { aggregationDataReader })
                                .ToList();

            foreach (var dataReader in dataReaders)
            {
                try
                {
                    var isNativeDataReader = dataReader != aggregationDataReader;
                    var campaignNames = dataReader.GetCampaignNames();

                    foreach (var campaignName in campaignNames)
                    {
                        // find campaign container or create a new one
                        var container = database.CampaignContainers.FirstOrDefault(container => container.Id == campaignName);

                        if (container == null)
                        {
                            container = new CampaignContainer(campaignName, dataReader.RootPath);
                            database.CampaignContainers.Add(container);

                            // try to load campaign meta data
                            var filePath = this.GetCampaignMetaPath(campaignName);

                            CampaignMetaInfo campaignMeta;

                            if (File.Exists(filePath))
                            {
                                var jsonString = File.ReadAllText(filePath);
                                campaignMeta = JsonSerializer.Deserialize<CampaignMetaInfo>(jsonString);
                            }
                            else
                            {
                                campaignMeta = new CampaignMetaInfo(campaignName);
                            }

                            container.CampaignMeta = campaignMeta;
                        }

                        // ensure that found campaign container root path matches that of the data reader
                        if (isNativeDataReader)
                        {
                            if (dataReader.RootPath != container.RootPath) 
                                throw new Exception("The data reader root path does not match the root path of the campaign data stored in the database.");
                        }

                        // get up-to-date campaign from data reader
                        var campaign = dataReader.GetCampaign(campaignName);

                        // if data reader is for aggregation data, update dataset`s flag
                        if (!isNativeDataReader)
                        {
                            var datasets = campaign.Variables.SelectMany(variable => variable.Datasets).ToList();

                            foreach (var dataset in datasets)
                            {
                                dataset.IsNative = false;
                            }
                        }

                        //
                        container.Campaign.Merge(campaign);
                    }
                }
                finally
                {
                    dataReader.Dispose();
                }
            }

            // the purpose of this block is to initalize empty properties 
            // add missing variables and clean up empty variables
            foreach (var campaignContainer in database.CampaignContainers)
            {
                // remove all variables where no native datasets are available
                // because only these provide metadata like name and group
                var variables = campaignContainer.Campaign.Variables;

                variables
                    .Where(variable => string.IsNullOrWhiteSpace(variable.Name))
                    .ToList()
                    .ForEach(variable => variables.Remove(variable));

                // initalize campaign meta
                campaignContainer.CampaignMeta.Initialize(campaignContainer.Campaign);

                // save campaign meta to disk
                this.SaveCampaignMeta(campaignContainer.CampaignMeta);
            }

            this.Database = database;
        }

        public DataReaderExtensionBase GetAggregationDataReader()
        {
            return new HdfDataReader(Environment.CurrentDirectory, _loggerFactory.CreateLogger("aggregation"));
        }

        public DataReaderExtensionBase GetNativeDataReader(string campaignName)
        {
            var container = this.Database.CampaignContainers.FirstOrDefault(container => container.Id == campaignName);

            if (container == null)
                throw new KeyNotFoundException("The requested campaign could not be found.");

            if (!_rootPathToDataReaderTypeMap.TryGetValue(container.RootPath, out var dataReaderType))
                throw new KeyNotFoundException("The requested data reader could not be found.");

            return this.InstantiateDataReader(container.RootPath, dataReaderType);
        }

        public void SaveCampaignMeta(CampaignMetaInfo campaignMeta)
        {
            var filePath = this.GetCampaignMetaPath(campaignMeta.Id);
            var jsonString = JsonSerializer.Serialize(campaignMeta, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        public void SaveConfig(OneDasDatabaseConfig config)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "config.json");
            var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(filePath, jsonString);
        }

        private DataReaderExtensionBase InstantiateDataReader(string rootPath, Type type)
        {
            var logger = _loggerFactory.CreateLogger(rootPath);
            return (DataReaderExtensionBase)Activator.CreateInstance(type, rootPath, logger);
        }

        private string GetCampaignMetaPath(string campaignName)
        {
            return Path.Combine(Environment.CurrentDirectory, "META", $"{campaignName.TrimStart('/').Replace('/', '_')}.json");
        }

        private Dictionary<string, Type> LoadDataReaders(Dictionary<string, string> rootPathToDataReaderIdMap)
        {
            var extensionDirectoryPath = Path.Combine(Environment.CurrentDirectory, "EXTENSION");

            var extensionFilePaths = Directory.EnumerateFiles(extensionDirectoryPath, "*.deps.json", SearchOption.AllDirectories)
                                              .Select(filePath => filePath.Replace(".deps.json", ".dll")).ToList();

            var idToDataReaderTypeMap = new Dictionary<string, Type>();
            var types = new List<Type>();

            // load assemblies
            foreach (var filePath in extensionFilePaths)
            {
                var loadContext = new ExtensionLoadContext(filePath);
                var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(filePath));
                var assembly = loadContext.LoadFromAssemblyName(assemblyName);

                types.AddRange(this.ScanAssembly(assembly));
            }

#warning Improve this.
            // add additional data reader
            types.Add(typeof(HdfDataReader));
            types.Add(typeof(InMemoryDataReader));

            // get ID for each extension
            foreach (var type in types)
            {
                var attribute = type.GetFirstAttribute<ExtensionIdentificationAttribute>();
                idToDataReaderTypeMap[attribute.Id] = type;
            }

            // return root path to type map
            return rootPathToDataReaderIdMap.ToDictionary(entry => entry.Key, entry =>
            {
                var rootPath = entry.Key;
                var dataReaderId = entry.Value;

                if (!idToDataReaderTypeMap.TryGetValue(dataReaderId, out var type))
                    throw new Exception($"No data reader extension with ID '{dataReaderId}' could be found.");

                return type;
            });
        }

        private List<Type> ScanAssembly(Assembly assembly)
        {
            return assembly.ExportedTypes.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(DataReaderExtensionBase))).ToList();
        }

        #endregion
    }
}
