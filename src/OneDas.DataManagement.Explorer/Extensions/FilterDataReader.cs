using Microsoft.Extensions.Logging;
using OneDas.Buffers;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Filters;
using OneDas.DataManagement.Explorer.Roslyn;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using OneDas.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification(FilterDataReader.Id, "OneDAS filters", "Dynamically loads and compiles user-defined filters.", "", "")]
    public class FilterDataReader : DataReaderExtensionBase
    {
        #region Fields

        public const string Id = "OneDas.Filters";

        private List<FilterDataReaderCacheEntry> _cacheEntries;

        #endregion

        #region Constructors

        static FilterDataReader()
        {
            FilterDataReader.FilterSettingsCache = new ConcurrentDictionary<DataReaderRegistration, FilterSettings>();
            FilterDataReader.FilterDataReaderCache = new ConcurrentDictionary<DataReaderRegistration, List<FilterDataReaderCacheEntry>>();
        }

        public FilterDataReader(DataReaderRegistration registration, ILogger logger) : base(registration, logger)
        {
            _cacheEntries = FilterDataReader.FilterDataReaderCache.GetOrAdd(registration, new List<FilterDataReaderCacheEntry>());
        }

        #endregion

        #region Properties

        public ClaimsPrincipal User { get; set; }

        public DatabaseManager DatabaseManager { get; set; }

        private static ConcurrentDictionary<DataReaderRegistration, FilterSettings> FilterSettingsCache { get; }

        private static ConcurrentDictionary<DataReaderRegistration, List<FilterDataReaderCacheEntry>> FilterDataReaderCache { get; }

        #endregion

        #region Methods

        public static bool TryGetFilterCodeDefinition(DatasetInfo datasetInfo, out CodeDefinition codeDefinition)
        {
            codeDefinition = default;

            if (FilterDataReader.FilterDataReaderCache.TryGetValue(datasetInfo.Registration, out var cacheEntries))
            {
                var cacheEntry = cacheEntries
                    .FirstOrDefault(entry => entry.SupportedChanneIds.Contains(datasetInfo.Parent.Id));

                if (cacheEntry is not null)
                {
                    codeDefinition = cacheEntry.FilterCodeDefinition;
                    return true;
                }
            }

            return false;
        }

        public static void ClearCache()
        {
            FilterDataReader.FilterSettingsCache.Clear();

            // unload DLLs
            var loadContexts = FilterDataReader.FilterDataReaderCache
                .SelectMany(entry => entry.Value.Select(cacheEntry => cacheEntry.LoadContext))
                .ToList();

            FilterDataReader.FilterDataReaderCache.Clear();

            foreach (var loadContext in loadContexts)
            {
                loadContext.Unload();
            }
        }

        public override (T[] Dataset, byte[] Status) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            var samplesPerDay = new SampleRateContainer(dataset.Id).SamplesPerDay;
            var length = (long)Math.Round((end - begin).TotalDays * samplesPerDay, MidpointRounding.AwayFromZero);
            var cacheEntry = _cacheEntries.FirstOrDefault(current => current.SupportedChanneIds.Contains(dataset.Parent.Id));

            if (cacheEntry is null)
                throw new Exception("The requested filter channel ID could not be found.");

            // prepare data
            var result = new double[length];
            var status = new byte[length];
            status.AsSpan().Fill(1);

            // fill database
            Func<string, string, string, DateTime, DateTime, double[]> getData = (string projectId, string channelId, string datasetId, DateTime begin, DateTime end) =>
            {
#warning improve this (PhysicalName)
                var project = this.DatabaseManager.Database.ProjectContainers
                    .FirstOrDefault(container => container.Id == projectId || container.PhysicalName == projectId);

                if (project == null)
                    throw new Exception($"Unable to find project with id '{projectId}'.");

                if (!this.DatabaseManager.Database.TryFindDatasetById(project.Id, channelId, datasetId, out var dataset))
                {
                    var path = $"{project.Id}/{channelId}/{datasetId}";
                    throw new Exception($"Unable to find dataset with path '{path}'.");
                }

                if (!Utilities.IsProjectAccessible(this.User, dataset.Parent.Parent.Id, this.DatabaseManager.Database))
                    throw new UnauthorizedAccessException("The current user is not allowed to access this filter.");

                var dataReader = this.DatabaseManager.GetDataReader(this.User, dataset.Registration);
                (var rawData, var status) = dataReader.ReadSingle(dataset, begin, end);
                var data = BufferUtilities.ApplyDatasetStatus2(rawData, status);

                return data;
            };

            // execute
            var filter = cacheEntry.FilterProvider.Filters.First(filter => filter.ToGuid(cacheEntry.FilterCodeDefinition).ToString() == dataset.Parent.Id);
            cacheEntry.FilterProvider.Filter(begin, end, filter, getData, result);

            return ((T[])(object)result, status);
        }

        protected override List<ProjectInfo> LoadProjects()
        {
            var projects = new Dictionary<string, ProjectInfo>();

            if (false && this.TryGetFilterSettings(out var filterSettings))
            {
                this.PopulateCache(filterSettings);
                var filterCodeDefinitions = filterSettings.CodeDefinitions.Where(filter => filter.CodeType == CodeType.Filter);

                foreach (var filterCodeDefinition in filterCodeDefinitions)
                {
                    var cacheEntry = _cacheEntries.FirstOrDefault(current => current.FilterCodeDefinition == filterCodeDefinition);

                    if (cacheEntry is null)
                        continue;
                   
                    var filterProvider = cacheEntry.FilterProvider;
                    var filterChannels = filterProvider.Filters;

                    foreach (var filterChannel in filterChannels)
                    {
                        var localFilterChannel = filterChannel;

                        // enforce group
                        if (localFilterChannel.ProjectId == FilterConstants.SharedProjectID)
                        {
                            localFilterChannel = localFilterChannel with
                            {
                                Group = filterCodeDefinition.Owner.Split('@')[0]
                            };
                        }
                        else if (string.IsNullOrWhiteSpace(localFilterChannel.Group))
                        {
                            localFilterChannel = localFilterChannel with
                            {
                                Group = "General"
                            };
                        }

                        // get or create project
                        if (!projects.TryGetValue(localFilterChannel.ProjectId, out var project))
                        {
                            project = new ProjectInfo(localFilterChannel.ProjectId);
                            projects[localFilterChannel.ProjectId] = project;
                        }

                        // create channel
                        if (!OneDasUtilities.CheckNamingConvention(localFilterChannel.ChannelName, out var message))
                        {
                            this.Logger.LogWarning($"Skipping channel '{localFilterChannel.ChannelName}' due to the following reason: {message}.");
                            continue;
                        }

                        var channel = new ChannelInfo(localFilterChannel.ToGuid(cacheEntry.FilterCodeDefinition).ToString(), project)
                        {
                            Name = localFilterChannel.ChannelName,
                            Group = localFilterChannel.Group,
                            Unit = localFilterChannel.Unit
                        };

                        // create datasets
                        var datasets = new List<DatasetInfo>()
                        {
                            new DatasetInfo(filterCodeDefinition.SampleRate, channel)
                            {
                                DataType = OneDasDataType.FLOAT64                              
                            }
                        };

                        // append
                        channel.Datasets.AddRange(datasets);
                        project.Channels.Add(channel);

                        // set begin and end
                        project.ProjectStart = DateTime.MaxValue;
                        project.ProjectEnd = DateTime.MinValue;
                    }
                }
            }

            return projects.Values.ToList();
        }

        protected override double GetAvailability(string projectId, DateTime Day)
        {
            return 1;
        }

        private bool TryGetFilterSettings(out FilterSettings filterSettings)
        {
            // search in cache
            if (FilterDataReader.FilterSettingsCache.TryGetValue(this.Registration, out filterSettings))
            {
                return true;   
            }
            else
            {
                var filePath = Path.Combine(this.RootPath, "filters.json");

                // read from disk
                if (File.Exists(filePath))
                {
                    var jsonString = File.ReadAllText(filePath);
                    filterSettings = JsonSerializer.Deserialize<FilterSettings>(jsonString);

                    // add to cache
                    var filterSettings2 = filterSettings; // to make compiler happy
                    FilterDataReader.FilterSettingsCache.AddOrUpdate(this.Registration, filterSettings, (key, value) => filterSettings2);

                    return true;
                }
            }

            return false;
        }

        private void PopulateCache(FilterSettings filterSettings)
        {
            var filterCodeDefinitions = filterSettings.CodeDefinitions
                .Where(filterSetting => filterSetting.CodeType == CodeType.Filter && filterSetting.IsEnabled)
                .ToList();

            var message = $"Compiling {filterCodeDefinitions.Count} filter(s) ...";
            this.Logger.LogInformation(message);

            var cacheEntries = new FilterDataReaderCacheEntry[filterCodeDefinitions.Count];

            Parallel.For(0, filterCodeDefinitions.Count, i =>
            {
                var filterCodeDefinition = filterCodeDefinitions[i];
                filterCodeDefinition.Code = this.PrepareCode(filterCodeDefinition.Code);

                var additionalCodeFiles = filterSettings.GetSharedFiles(filterCodeDefinition.Owner)
                    .Select(codeDefinition => codeDefinition.Code)
                    .ToList();

                var roslynProject = new RoslynProject(filterCodeDefinition, additionalCodeFiles);
                using var peStream = new MemoryStream();

                var emitResult = roslynProject.Workspace.CurrentSolution.Projects.First()
                    .GetCompilationAsync().Result
                    .Emit(peStream);

                if (!emitResult.Success)
                    return;

                peStream.Seek(0, SeekOrigin.Begin);

                var loadContext = new FilterDataReaderLoadContext();
                var assembly = loadContext.LoadFromStream(peStream);
                var assemblyType = assembly.GetType();

                // filter method info
                var interfaceType = typeof(FilterProviderBase);
                var filterType = assembly
                    .GetTypes()
                    .FirstOrDefault(type => interfaceType.IsAssignableFrom(type));

                if (filterType is null)
                    return;

                try
                {
                    var filterProvider = (FilterProviderBase)Activator.CreateInstance(filterType);
                    var supportedChanneIds = filterProvider.Filters.Select(filter => filter.ToGuid(filterCodeDefinition).ToString()).ToList();
                    cacheEntries[i] = new FilterDataReaderCacheEntry(filterCodeDefinition, loadContext, filterProvider, supportedChanneIds);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"Failed to instantiate the filter provider '{filterCodeDefinition.Name}' of user {filterCodeDefinition.Owner}. Detailed error: {ex.GetFullMessage()}");
                }
            });

            _cacheEntries.AddRange(cacheEntries.Where(cacheEntry => cacheEntry is not null));
            this.Logger.LogInformation($"{message} Done.");
        }

        private string PrepareCode(string code)
        {
            // change signature
            code = code.Replace(
                "DataProvider dataProvider",
                "System.Func<string, string, string, System.DateTime, System.DateTime, double[]> getData");

            // matches strings like "= dataProvider.IN_MEMORY_TEST_ACCESSIBLE.T1.DATASET_1_s_mean;"
            var pattern1 = @"=\s*dataProvider\.([a-zA-Z_0-9]+)\.([a-zA-Z_0-9]+)\.DATASET_([a-zA-Z_0-9]+);";
            code = Regex.Replace(code, pattern1, match =>
            {
                var projectId = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;

#warning: Whenever the space in the dataset name is removed, update this code
                var regex = new Regex("_");
                var datasetId = regex.Replace(match.Groups[3].Value, " ", 1);

                return $"= (double[])getData(\"{projectId}\", \"{channelId}\", \"{datasetId}\", begin, end);";
            });

            // matches strings like "= dataProvider.Read(campaignId, channelId, datasetId, begin, end);"
            var pattern3 = @"=\s*dataProvider\.Read\((.*?),(.*?),(.*?),(.*?),(.*?)\);";
            code = Regex.Replace(code, pattern3, match =>
            {
                var projectId = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;
                var datasetId = match.Groups[3].Value;
                var begin = match.Groups[4].Value;
                var end = match.Groups[5].Value;

                return $"= (double[])getData({projectId}, {channelId}, {datasetId}, {begin}, {end});";
            });

            // matches strings like "= dataProvider.Read(campaignId, channelId, datasetId);"
            var pattern2 = @"=\s*dataProvider\.Read\((.*?),(.*?),(.*?)\);";
            code = Regex.Replace(code, pattern2, match =>
            {
                var projectId = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;
                var datasetId = match.Groups[3].Value;

                return $"= (double[])getData({projectId}, {channelId}, {datasetId}, begin, end);";
            });

            return code;
        }

        #endregion
    }
}
