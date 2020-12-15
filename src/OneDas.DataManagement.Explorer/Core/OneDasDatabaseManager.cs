using GraphQL.Utilities;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Services;
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
     * 05.       get project names
     * 06.           for each project name
     * 07.               find project container in current database or create new one
     * 08.               get an up-to-date project instance from the data reader
     * 09.               merge both projects
     * 10. save updated database
     * *******************************************************************************
     */

    public class OneDasDatabaseManager
    {
        #region Events

        public event EventHandler<OneDasDatabase> OnDatabaseUpdated;

        #endregion

        #region Records

        public record DatabaseManagerState
        {
            public OneDasDatabase Database { get; init; }
            public Dictionary<string, Type> RootPathToDataReaderTypeMap { get; init; }
            public Dictionary<string, List<ProjectInfo>> RootPathToProjectsMap { get; init; }
        }

        #endregion

        #region Fields

        private string _folderPath;
        private ILogger _logger;
        private ILoggerFactory _loggerFactory;
        private IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        public OneDasDatabaseManager(ILogger logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        #endregion

        #region Properties

        public OneDasDatabase Database => this.State.Database;

        public OneDasDatabaseConfig Config { get; private set; }

        private DatabaseManagerState State { get; set; }

        #endregion

        #region Methods

        public void Initialize(IServiceProvider serviceProvider, string folderPath)
        {
            _serviceProvider = serviceProvider;

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new Exception("Could not initialize database. Please check the database folder path and try again.");
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                    Directory.CreateDirectory(Path.Combine(folderPath, "ATTACHMENTS"));
                    Directory.CreateDirectory(Path.Combine(folderPath, "DATA"));
                    Directory.CreateDirectory(Path.Combine(folderPath, "EXPORT"));
                    Directory.CreateDirectory(Path.Combine(folderPath, "EXTENSION"));
                    Directory.CreateDirectory(Path.Combine(folderPath, "META"));
                    Directory.CreateDirectory(Path.Combine(folderPath, "PRESETS"));

                    var filePath = Path.Combine(folderPath, "dbconfig.json");

                    if (File.Exists(filePath))
                    {
                        var jsonString = File.ReadAllText(filePath);
                        this.Config = JsonSerializer.Deserialize<OneDasDatabaseConfig>(jsonString);
                    }
                    else
                    {
                        this.Config = new OneDasDatabaseConfig();
                    }

                    // initialize
                    this.Config.Initialize();

                    // save config to disk
                    this.SaveConfig(folderPath, this.Config);
                }
                catch
                {
                    throw new Exception("Could not initialize database. Please check the database folder path and try again.");
                }
            }

            _folderPath = folderPath;
        }

        public void Update()
        {
            // Concept:
            //
            // 1) rootPathToProjectsMap, rootPathToDataReaderTypeMap and database are instantiated in this method,
            // combined into a new DatabaseManagerState and then set in an atomic operation to the State propery.
            // 
            // 2) Within this method, the rootPathToProjectsMap cache gets filled for both, aggregated and native data 
            // reader.
            //
            // 3) It may happen that during this process, which might take a while, an external caller calls 
            // GetAggregationDataReader or GetNativeDataReader. To divide both processes (external call vs this method),
            // the State property is introduced, so external calls use old maps and this method uses the new instances.

            var database = new OneDasDatabase();

            // create new empty projects map
            var rootPathToProjectsMap = new Dictionary<string, List<ProjectInfo>>();

            // load data readers
            var rootPathToDataReaderTypeMap = this.LoadDataReaders(this.Config.RootPathToDataReaderIdMap);

            // instantiate data readers
            using var aggregationDataReader = this.GetAggregationDataReader(rootPathToProjectsMap);

            var dataReaders = rootPathToDataReaderTypeMap
                                .Select(entry => this.InstantiateDataReader(entry.Key, entry.Value, rootPathToProjectsMap))
                                .Concat(new DataReaderExtensionBase[] { aggregationDataReader })
                                .ToList();

            foreach (var dataReader in dataReaders)
            {
                try
                {
                    var isNativeDataReader = dataReader != aggregationDataReader;
                    var projectNames = dataReader.GetProjectNames();

                    foreach (var projectName in projectNames)
                    {
                        // find project container or create a new one
                        var container = database.ProjectContainers.FirstOrDefault(container => container.Id == projectName);

                        if (container == null)
                        {
                            container = new ProjectContainer(projectName, dataReader.RootPath);
                            database.ProjectContainers.Add(container);

                            // try to load project meta data
                            var filePath = this.GetProjectMetaPath(projectName);

                            ProjectMetaInfo projectMeta;

                            if (File.Exists(filePath))
                            {
                                var jsonString = File.ReadAllText(filePath);
                                projectMeta = JsonSerializer.Deserialize<ProjectMetaInfo>(jsonString);
                            }
                            else
                            {
                                projectMeta = new ProjectMetaInfo(projectName);
                            }

                            container.ProjectMeta = projectMeta;
                        }

                        // ensure that found project container root path matches that of the data reader
                        if (isNativeDataReader)
                        {
                            if (dataReader.RootPath != container.RootPath) 
                                throw new Exception("The data reader root path does not match the root path of the project data stored in the database.");
                        }

                        // get up-to-date project from data reader
                        var project = dataReader.GetProject(projectName);

                        // if data reader is for aggregation data, update dataset`s flag
                        if (!isNativeDataReader)
                        {
                            var datasets = project.Channels.SelectMany(channel => channel.Datasets).ToList();

                            foreach (var dataset in datasets)
                            {
                                dataset.IsNative = false;
                            }
                        }

                        //
                        container.Project.Merge(project, ChannelMergeMode.OverwriteMissing);
                    }
                }
                finally
                {
                    dataReader.Dispose();
                }
            }

            // the purpose of this block is to initalize empty properties 
            // add missing channels and clean up empty channels
            foreach (var projectContainer in database.ProjectContainers)
            {
                // remove all channels where no native datasets are available
                // because only these provide metadata like name and group
                var channels = projectContainer.Project.Channels;

                channels
                    .Where(channel => string.IsNullOrWhiteSpace(channel.Name))
                    .ToList()
                    .ForEach(channel => channels.Remove(channel));

                // initalize project meta
                projectContainer.ProjectMeta.Initialize(projectContainer.Project);

                // save project meta to disk
                this.SaveProjectMeta(projectContainer.ProjectMeta);
            }

            this.State = new DatabaseManagerState()
            {
                Database = database,
                RootPathToProjectsMap = rootPathToProjectsMap,
                RootPathToDataReaderTypeMap = rootPathToDataReaderTypeMap
            };

            this.OnDatabaseUpdated?.Invoke(this, database);
        }

        public DataReaderExtensionBase GetAggregationDataReader(
           Dictionary<string, List<ProjectInfo>> rootPathToProjectsMap = null)
        {
            var rootPath = string.IsNullOrWhiteSpace(this.Config.AggregationDataReaderRootPath)
                ? _folderPath
                : this.Config.AggregationDataReaderRootPath;

            if (rootPathToProjectsMap == null)
                rootPathToProjectsMap = this.State.RootPathToProjectsMap;

            var dataReader = (HdfDataReader)this.InstantiateDataReader(rootPath, typeof(HdfDataReader), rootPathToProjectsMap);

            return dataReader;
        }

        public DataReaderExtensionBase GetNativeDataReader(
            string projectId)
        {
            var container = this.State.Database.ProjectContainers.FirstOrDefault(container => container.Id == projectId);

            if (container == null)
                throw new KeyNotFoundException("The requested project could not be found.");

            if (!this.State.RootPathToDataReaderTypeMap.TryGetValue(container.RootPath, out var dataReaderType))
                throw new KeyNotFoundException("The requested data reader could not be found.");

            return this.InstantiateDataReader(container.RootPath, dataReaderType, this.State.RootPathToProjectsMap);
        }

        public void SaveProjectMeta(ProjectMetaInfo projectMeta)
        {
            var filePath = this.GetProjectMetaPath(projectMeta.Id);
            var jsonString = JsonSerializer.Serialize(projectMeta, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        public void SaveConfig(string folderPath, OneDasDatabaseConfig config)
        {
            var filePath = Path.Combine(folderPath, "dbconfig.json");
            var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(filePath, jsonString);
        }

        private DataReaderExtensionBase InstantiateDataReader(
            string rootPath, 
            Type type, 
            Dictionary<string, List<ProjectInfo>> rootPathToProjectsMap)
        {
            var logger = _loggerFactory.CreateLogger(rootPath);
            var dataReader = (DataReaderExtensionBase)Activator.CreateInstance(type, rootPath, logger);

            if (type == typeof(HdfDataReader))
            {
                var fileAccessManger = _serviceProvider.GetRequiredService<FileAccessManager>();
                ((HdfDataReader)dataReader).FileAccessManager = fileAccessManger;
            }

            // initialize projects property
            if (rootPathToProjectsMap.TryGetValue(rootPath, out var value))
            {
                dataReader.InitializeProjects(value);
            }
            else
            {
                _logger.LogInformation($"Loading {dataReader.RootPath} ...");
                dataReader.InitializeProjects();
                rootPathToProjectsMap[rootPath] = dataReader.Projects;
            }

            return dataReader;
        }

        private string GetProjectMetaPath(string projectName)
        {
            return Path.Combine(_folderPath, "META", $"{projectName.TrimStart('/').Replace('/', '_')}.json");
        }

        private Dictionary<string, Type> LoadDataReaders(Dictionary<string, string> rootPathToDataReaderIdMap)
        {
            var extensionDirectoryPath = Path.Combine(_folderPath, "EXTENSION");

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
            types.Add(typeof(FilterDataReader));

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
