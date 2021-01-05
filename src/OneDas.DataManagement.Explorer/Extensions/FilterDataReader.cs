using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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

        public ClaimsPrincipal User { get; set; }

        public DatabaseManager DatabaseManager { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public static ConcurrentDictionary<DataReaderRegistration, FilterSettings> FilterSettingsCache { get; }

        public static ConcurrentDictionary<(DataReaderRegistration, string), FilterDataReaderCacheEntry> MethodInfoCache { get; }

        #endregion

        #region Methods

        public static void ClearCache()
        {
            FilterDataReader.FilterSettingsCache.Clear();
            FilterDataReader.MethodInfoCache.Clear();
        }

        public override (T[] Dataset, byte[] Status) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            var samplesPerDay = new SampleRateContainer(dataset.Id).SamplesPerDay;
            var length = (long)Math.Round((end - begin).TotalDays * samplesPerDay, MidpointRounding.AwayFromZero);

            if (!this.TryGetCacheEntry(dataset.Parent.Id, out var cacheEntry))
                throw new Exception("Unable to compile filter code.");

            // prepare data
            var result = new double[length];
            var status = new byte[length];
            status.AsSpan().Fill(1);

            // fill database
            Func<string, string, string, double[]> getData = (string projectId, string channelId, string datasetId) =>
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
#error fix this
            cacheEntry.FilterProvider.Filter(null, begin, end, getData, result);
#error link to xml file: C:\Users\wilvin\.nuget\packages\microsoft.netcore.app.ref\5.0.0\ref\net5.0

            return ((T[])(object)result, status);
        }

        protected override List<ProjectInfo> LoadProjects()
        {
            var projects = new Dictionary<string, ProjectInfo>();

            if (this.TryGetFilterSettings(out var filterSettings))
            {
                var filterDescriptions = filterSettings.Filters.Where(filter => filter.CodeType == CodeType.Filter);

                foreach (var filterDescription in filterDescriptions)
                {
                    if (!this.TryGetCacheEntry(filterDescription.Id, out var cacheEntry))
                        continue;

                    ClaimsPrincipal user;

                    using (var scope = this.ServiceProvider.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                        user = Utilities.GetClaimsPrincipalAsync(filterDescription.Owner, userManager).Result;
                    }

                    
                    var filterProvider = cacheEntry.FilterProvider;
                    var filterChannels = filterProvider.Filters;

                    foreach (var filterChannel in filterChannels)
                    {
                        var sharedProjectID = "/IN_MEMORY/FILTERS/SHARED";
                        var localFilterChannel = filterChannel;

#error fix this
                        //Utilities.IsProjectEditable(user, filterChannel.ProjectId, this.DatabaseManager.Database);

                        // enforce project id
                        if (!OneDasUtilities.CheckProjectNamingConvention(filterChannel.ProjectId, out var _) ||
                            !Utilities.IsProjectEditable(user, filterChannel.ProjectId, this.DatabaseManager.Database))
                        {
                            localFilterChannel = localFilterChannel with
                            {
                                ProjectId = sharedProjectID
                            };
                        }

                        // enforce group
                        if (localFilterChannel.ProjectId == sharedProjectID)
                        {
                            localFilterChannel = localFilterChannel with
                            {
                                Group = filterDescription.Owner.Split('@')[0]
                            };
                        }

                        // get or create project
                        if (!projects.TryGetValue(localFilterChannel.ProjectId, out var project))
                        {
                            project = new ProjectInfo(localFilterChannel.ProjectId);
                            projects[localFilterChannel.ProjectId] = project;
                        }

                        // create channel
                        if (!OneDasUtilities.CheckNamingConvention(localFilterChannel.ChannelId, out var _))
                            continue;

#error fix this
                        var channel = new ChannelInfo(id, project)
                        {
                            Name = localFilterChannel.ChannelId,
                            Group = localFilterChannel.Group,
                            Unit = localFilterChannel.Unit
                        };

                        // create datasets
                        var datasets = new List<DatasetInfo>()
                        {
                            new DatasetInfo(filterDescription.SampleRate, channel)
                            {
                                DataType = OneDasDataType.FLOAT64
                            }
                        };

                        // append
                        channel.Datasets.AddRange(datasets);
                        project.Channels.Add(channel);
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

        private bool TryGetCacheEntry(string filterDescriptionId, out FilterDataReaderCacheEntry cacheEntry)
        {
            cacheEntry = null;

            // search in cache
            if (FilterDataReader.MethodInfoCache.TryGetValue((this.Registration, filterDescriptionId), out cacheEntry))
            {
                return true;
            }
            else
            {
                if (!this.TryGetFilterSettings(out var filterSettings))
                    throw new Exception("No filter settings available.");

                // compile channel
                var filterDescription = filterSettings.Filters.FirstOrDefault(current => current.Id == filterDescriptionId);

                if (filterDescription is null)
                    throw new Exception($"No filter found with ID '{filterDescriptionId}'.");

                filterDescription.Code = this.PrepareCode(filterDescription.Code);

                var additionalCodeFiles = filterSettings.GetSharedFiles(filterDescription.Owner)
                    .Select(filterDescription => filterDescription.Code)
                    .ToList();

                var roslynProject = new RoslynProject(filterDescription, additionalCodeFiles);
                using var peStream = new MemoryStream();

                var emitResult = roslynProject.Workspace.CurrentSolution.Projects.First()
                    .GetCompilationAsync().Result
                    .Emit(peStream);

                if (!emitResult.Success) 
                    return false;

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
                    throw new Exception($"No type found that implements the IFilterProvider interface for filter with ID '{filterDescriptionId}'.");

                try
                {
                    var filterProvider = (FilterProviderBase)Activator.CreateInstance(filterType);

                    // add to cache
                    cacheEntry = new FilterDataReaderCacheEntry(loadContext, filterProvider);
                    var localCacheEntry = cacheEntry;
                    FilterDataReader.MethodInfoCache.AddOrUpdate((this.Registration, filterDescriptionId), cacheEntry, (key, value) => localCacheEntry);
                    return true;
                }
                catch (MissingMethodException)
                {
                    throw new Exception($"No parameterless constructor found for filter with ID '{filterDescriptionId}'.");
                }
            }
        }

        private string PrepareCode(string code)
        {
            // change signature
            code = code.Replace(
                "DateTime begin, DateTime end, DataProvider dataProvider, double[] result",
                "DateTime begin, DateTime end, System.Func<string, string, string, double[]> getData, double[] result");

            // matches strings like "= dataProvider.IN_MEMORY_TEST_ACCESSIBLE.T1.DATASET_1_s_mean;"
            var pattern1 = @"=\s*dataProvider\.([a-zA-Z_0-9]+)\.([a-zA-Z_0-9]+)\.DATASET_([a-zA-Z_0-9]+);";
            code = Regex.Replace(code, pattern1, match =>
            {
                var projectId = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;

#warning: Whenever the space in the dataset name is removed, update this code
                var regex = new Regex("_");
                var datasetId = regex.Replace(match.Groups[3].Value, " ", 1);

                return $"= (double[])getData(\"{projectId}\", \"{channelId}\", \"{datasetId}\");";
            });

            // matches strings like "= dataProvider.Read(campaignId, channelId, datasetId);"
            var pattern2 = @"=\s*dataProvider\.Read\((.*),(.*),(.*)\);";
            code = Regex.Replace(code, pattern2, match =>
            {
                var projectId = match.Groups[1].Value;
                var channelId = match.Groups[2].Value;

#warning: Whenever the space in the dataset name is removed, update this code
                var regex = new Regex("_");
                var datasetId = regex.Replace(match.Groups[3].Value, " ", 1);

                return $"= (double[])getData({projectId}, {channelId}, {datasetId});";
            });

            return code;
        }

        #endregion
    }
}
