using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.Filters", "OneDAS filters", "Dynamically loads and compiles user-defined filters.", "", "")]
    public class FilterDataReader : DataReaderExtensionBase
    {
        #region Constructors

        static FilterDataReader()
        {
            FilterDataReader.Cache = new ConcurrentDictionary<DataReaderRegistration, FilterDataReaderCacheEntry>();
        }

        public FilterDataReader(DataReaderRegistration registration, ILogger logger) : base(registration, logger)
        {
            this.ApplyStatus = false;
        }

        #endregion

        #region Properties

        public string Username { get; set; }

        public static ConcurrentDictionary<DataReaderRegistration, FilterDataReaderCacheEntry> Cache { get; }

        public OneDasDatabase Database { get; set; }

        #endregion

        #region Methods

        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            throw new NotImplementedException();
        }

        protected override List<ProjectInfo> LoadProjects()
        {
            var possibleCacheEntry = new FilterDataReaderCacheEntry();
            var cacheEntry = FilterDataReader.Cache.GetOrAdd(this.Registration, possibleCacheEntry);
            var isCached = cacheEntry != possibleCacheEntry;
            var filePath = Path.Combine(this.RootPath, "filters.json");

            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                var filterSettings = JsonSerializer.Deserialize<FilterSettings>(jsonString);

                var project = new ProjectInfo("FILTERS")
                {
                    ProjectStart = DateTime.MinValue,
                    ProjectEnd = DateTime.MaxValue
                };

                var channels = filterSettings.Filters
                    .Where(filter => filter.CodeType == CodeType.Channel)
                    .Select(filter =>
                {
                    // create channel
                    var channel = new ChannelInfo(filter.Id, project)
                    {
                        Group = filter.Group,
                        Name = filter.Name,
                        Unit = filter.Unit  
                    };

                    var datasets = new List<DatasetInfo>()
                    {
                        new DatasetInfo(filter.SampleRate, channel)
                        {
                            DataType = OneDasDataType.FLOAT64
                        }
                    };

                    channel.Datasets.AddRange(datasets);

                    // compile channel
                    if (!isCached)
                    {
                        var additionalCodeFiles = filterSettings.GetSharedFiles(this.Username)
                            .Select(filterDescription => filterDescription.Code)
                            .ToList();

                        (var preparedCode, var replacements) = this.PrepareCode(filter.Code);

                        filter.Code = preparedCode;
                        var roslynProject = new RoslynProject(filter, additionalCodeFiles);
                        var peStream = new MemoryStream();

                        var compilation = roslynProject.Workspace.CurrentSolution.Projects.First()
                            .GetCompilationAsync().Result
                            .Emit(peStream);

                        var assembly = cacheEntry.LoadContext.LoadFromStream(peStream);
                        var assemblyType = assembly.GetType();

                        var methodInfo = Utilities.GetMethodsBySignature(assemblyType, typeof(void),
                            new Type[] { typeof(DateTime), typeof(DateTime), typeof(Dictionary<string, double[]>), typeof(double[]) })
                            .First();

                        cacheEntry.FilterIdToMethodInfoMap[filter.Id] = methodInfo;
                    }

                    // return
                    return channel;
                });

                project.Channels.AddRange(channels);

                return new List<ProjectInfo>() { project };
            }
            else
            {
                return new List<ProjectInfo>();
            }
        }

        protected override double GetAvailability(string projectId, DateTime Day)
        {
            return 1;
        }

        private (string, Dictionary<string, DatasetInfo>) PrepareCode(string code)
        {
            var replacements = new Dictionary<string, DatasetInfo>();

            // change signature
            code = code.Replace(
                "DateTime begin, DateTime end, Database database, double[] result",
                "DateTime begin, DateTime end, System.Collections.Generic.Dictionary<string, double[]> database, double[] result");

            // matches strings like "= database.IN_MEMORY_TEST_ACCESSIBLE.T1.DATASET_1_s_mean;"
            var pattern = @"=\s?database\.([a-zA-Z_0-9]+)\.([a-zA-Z_0-9]+)\.DATASET_([a-zA-Z_0-9]+);";

            Regex.Replace(code, pattern, match =>
            {
                var projectPhysicalName = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;

#warning: Whenever the space in the dataset name is removed, update this code
                var regex = new Regex("_");
                var datasetId = regex.Replace(match.Groups[3].Value, " ", 1);

                var project = this.Database.ProjectContainers
                    .First(container => container.PhysicalName == projectPhysicalName);

#warning improve this
                if (!this.Database.TryFindDatasetById(project.Id, channelId, datasetId, out var dataset))
                    throw new Exception();

                var id = $"{project.Id}/{channelId}/{datasetId}";
                replacements[id] = dataset;

                return $"= database[{id}];";
            });

            return (code, replacements);
        }

        #endregion
    }
}
