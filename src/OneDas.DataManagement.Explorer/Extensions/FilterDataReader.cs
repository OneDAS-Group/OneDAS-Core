using Microsoft.Extensions.Logging;
using OneDas.Buffers;
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
            FilterDataReader.FilterSettingsCache = new ConcurrentDictionary<DataReaderRegistration, FilterSettings>();
            FilterDataReader.MethodInfoCache = new ConcurrentDictionary<(DataReaderRegistration, string), FilterDataReaderCacheEntry>();
        }

        public FilterDataReader(DataReaderRegistration registration, ILogger logger) : base(registration, logger)
        {
            //
        }

        #endregion

        #region Properties

        public string Username { get; set; }

        public OneDasDatabaseManager DatabaseManager { get; set; }

        public static ConcurrentDictionary<DataReaderRegistration, FilterSettings> FilterSettingsCache { get; }

        public static ConcurrentDictionary<(DataReaderRegistration, string), FilterDataReaderCacheEntry> MethodInfoCache { get; }

        #endregion

        #region Methods

        public override (T[] Dataset, byte[] Status) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            var samplesPerDay = new SampleRateContainer(dataset.Id).SamplesPerDay;
            var length = (long)Math.Round((end - begin).TotalDays * samplesPerDay, MidpointRounding.AwayFromZero);
            var cacheEntry = this.GetCacheEntry(dataset.Parent.Id);

            // prepare data
            var result = new double[length];
            var status = new byte[length];
            status.AsSpan().Fill(1);

            // fill database
            var database = cacheEntry.Replacements.ToDictionary(entry => entry.Key, entry =>
            {
                var dataset = entry.Value;
                var dataReader = this.DatabaseManager.GetDataReader(dataset.Registration);
                (var rawData, var status) = dataReader.ReadSingle(dataset, begin, end);
                var data = BufferUtilities.ApplyDatasetStatus2(rawData, status);

                return data;
            });

            // execute
            var filterInstance = Activator.CreateInstance(cacheEntry.MethodInfo.DeclaringType);
            cacheEntry.MethodInfo.Invoke(filterInstance, new object[] { begin, end, database, result });

            return ((T[])(object)result, status);
        }

        protected override List<ProjectInfo> LoadProjects()
        {
            var filterSettings = this.GetFilterSettings();
            
            if (filterSettings is not null)
            {
                var project = new ProjectInfo("/IN_MEMORY/FILTERS/SHARED")
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

        private FilterSettings GetFilterSettings()
        {
            FilterSettings filterSettings = null;

            // search in cache
            if (!FilterDataReader.FilterSettingsCache.TryGetValue(this.Registration, out filterSettings))
            {
                var filePath = Path.Combine(this.RootPath, "filters.json");

                // read from disk
                if (File.Exists(filePath))
                {
                    var jsonString = File.ReadAllText(filePath);
                    filterSettings = JsonSerializer.Deserialize<FilterSettings>(jsonString);

                    // add to cache
                    FilterDataReader.FilterSettingsCache.AddOrUpdate(this.Registration, filterSettings, (key, value) => filterSettings);
                }
            }           

            return filterSettings;
        }

        private FilterDataReaderCacheEntry GetCacheEntry(string filterId)
        {
            // search in cache
            if (!FilterDataReader.MethodInfoCache.TryGetValue((this.Registration, filterId), out var cacheEntry))
            {
                var filterSettings = this.GetFilterSettings();

                // compile channel
                var additionalCodeFiles = filterSettings.GetSharedFiles(this.Username)
                    .Select(filterDescription => filterDescription.Code)
                    .ToList();

                var filter = filterSettings.Filters.First(current => current.Id == filterId);
                (var preparedCode, var replacements) = this.PrepareCode(filter.Code);

                filter.Code = preparedCode;
                var roslynProject = new RoslynProject(filter, additionalCodeFiles);
                using var peStream = new MemoryStream();

                var compilation = roslynProject.Workspace.CurrentSolution.Projects.First()
                    .GetCompilationAsync().Result
                    .Emit(peStream);

                peStream.Seek(0, SeekOrigin.Begin);

                var loadContext = new FilterDataReaderLoadContext();
                var assembly = loadContext.LoadFromStream(peStream);
                var assemblyType = assembly.GetType();

                var methodInfo = assembly.GetTypes().SelectMany(type =>
                {
#warning https://stackoverflow.com/questions/4681031/how-do-i-check-if-a-type-provides-a-parameterless-constructor
                    var parameters = new Type[] { typeof(DateTime), typeof(DateTime), typeof(Dictionary<string, double[]>), typeof(double[]) };
                    return Utilities.GetMethodsBySignature(type, typeof(void), parameters);
                }).First();

                // add to cache
                cacheEntry = new FilterDataReaderCacheEntry(loadContext, methodInfo, replacements);
                FilterDataReader.MethodInfoCache.AddOrUpdate((this.Registration, filterId), cacheEntry, (key, value) => cacheEntry);
            }

            return cacheEntry;
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

            code = Regex.Replace(code, pattern, match =>
            {
                var projectPhysicalName = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;

#warning: Whenever the space in the dataset name is removed, update this code
                var regex = new Regex("_");
                var datasetId = regex.Replace(match.Groups[3].Value, " ", 1);

                var project = this.DatabaseManager.Database.ProjectContainers
                        .First(container => container.PhysicalName == projectPhysicalName);

#warning improve this
                if (!this.DatabaseManager.Database.TryFindDatasetById(project.Id, channelId, datasetId, out var dataset))
                    throw new Exception();

                var id = $"{match.Groups[1]}/{match.Groups[2]}/{match.Groups[3]}";
                replacements[id] = dataset;

                return $"= (double[])database[\"{id}\"];";
            });

            return (code, replacements);
        }

        #endregion
    }
}
