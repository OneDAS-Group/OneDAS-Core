using HDF.PInvoke;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Services;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.HDF", "OneDAS HDF", "Provides access to databases with OneDAS HDF files.", "", "")]
    public class HdfDataReader : DataReaderExtensionBase
    {
        #region Fields

        private List<string> _lastLocalFilePaths;
        private bool _copyToLocal;

        #endregion

        #region Constructors

        public HdfDataReader(DataReaderRegistration registration, ILogger logger) : base(registration, logger)
        {
            //
        }

        #endregion

        #region Properties

        public FileAccessManager FileAccessManager { get; set; }

        #endregion

        #region Methods

#warning Unify this with other readers
        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            // read optional parameters
            if (this.OptionalParameters != null && this.OptionalParameters.TryGetValue("CopyToLocal", out var value))
                _copyToLocal = value == "true";

            // 
            long fileId = -1;

            var folderPath = Path.Combine(this.RootPath, "DATA");
            var samplesPerDay = new SampleRateContainer(dataset.Id).SamplesPerDay;
            var length = (long)Math.Round((end - begin).TotalDays * samplesPerDay, MidpointRounding.AwayFromZero);
            var data = new T[length];
            var statusSet = new byte[length];

            if (!Directory.Exists(folderPath))
                return (data, statusSet);

            var periodPerFile = TimeSpan.FromDays(1);

            // read data
            var currentBegin = begin.RoundDown(periodPerFile);
            var fileLength = (int)Math.Round(periodPerFile.TotalDays * samplesPerDay, MidpointRounding.AwayFromZero);
            var fileOffset = (int)Math.Round((begin - currentBegin).TotalDays * samplesPerDay, MidpointRounding.AwayFromZero);
            var bufferOffset = 0;
            var remainingBufferLength = (int)length;

            while (remainingBufferLength > 0)
            {
                // get data
                var project = (ProjectInfo)dataset.Parent.Parent;
                var projectId = project.Id;

                /* aggregated data have no version number in file names */
                var fileNamePattern = $"{this.ToUnderscoredId(projectId)}_*{currentBegin.ToISO8601()}.h5";
                var currentFolderPath = Path.Combine(folderPath, currentBegin.ToString("yyyy-MM"));

                List<string> filePaths;

                if (Directory.Exists(currentFolderPath))
                    filePaths = Directory
                    .EnumerateFiles(currentFolderPath, fileNamePattern, SearchOption.TopDirectoryOnly)
                    .ToList();
                else
                    filePaths = new List<string>();

                var compensation = fileLength - fileOffset;

                if (_copyToLocal)
                    filePaths = this.HandleCopyToLocal(filePaths);

                // go on
                foreach (var filePath in filePaths)
                {
                    try
                    {
                        this.FileAccessManager?.Register(filePath, CancellationToken.None);
                        fileId = H5F.open(filePath, H5F.ACC_RDONLY);

                        if (H5I.is_valid(fileId) > 0)
                        {
                            int lastIndex = -1;

                            // the averages database has no "is_chunk_completed_set"
#warning use format version instead to check this?
                            var isChunkCompletedSetPath = $"{project.GetPath()}/is_chunk_completed_set";

                            if (IOHelper.CheckLinkExists(fileId, isChunkCompletedSetPath))
                            {
                                var isChunkCompletedSet = IOHelper.ReadDataset<byte>(fileId, isChunkCompletedSetPath).ToList();
                                lastIndex = isChunkCompletedSet.FindLastIndex(value => value > 0);
                            }
                            else
                            {
                                lastIndex = 1440 - 1;
                            }

                            if (lastIndex > -1)
                            {
                                var actualFileLength = (int)Math.Round(fileLength * (double)(lastIndex + 1) / 1440, MidpointRounding.AwayFromZero);
                                var fileBlock = actualFileLength - fileOffset;
                                
                                if (fileBlock >= 0) /* data available in file */
                                {
                                    var currentBlock = Math.Min(remainingBufferLength, fileBlock);
                                    var datasetPath = dataset.GetPath();

                                    if (IOHelper.CheckLinkExists(fileId, datasetPath))
                                    {
                                        var currentDataset = IOHelper.ReadDataset<T>(fileId, datasetPath, (ulong)fileOffset, (ulong)currentBlock);

                                        byte[] currentStatusSet = null;

                                        if (IOHelper.CheckLinkExists(fileId, datasetPath + "_status"))
                                            currentStatusSet = IOHelper.ReadDataset(fileId, datasetPath + "_status", (ulong)fileOffset, (ulong)currentBlock).Cast<byte>().ToArray();

                                        // write data
                                        currentDataset.CopyTo(data.AsSpan(bufferOffset));

                                        if (currentStatusSet != null)
                                            currentStatusSet.CopyTo(statusSet.AsSpan(bufferOffset));

                                        // update loop state
                                        fileOffset += currentBlock; // file offset
                                        bufferOffset += currentBlock; // buffer offset
                                        compensation -= currentBlock; // file remaining 
                                        remainingBufferLength -= currentBlock; // buffer remaining
                                    }
                                    else
                                    {
                                        this.Logger.LogDebug($"Could not find dataset '{datasetPath}'.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.Logger.LogWarning($"Could not open file '{filePath}'.");
                        }
                    }
                    finally
                    {
                        if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
                        this.FileAccessManager?.Unregister(filePath);
                    }
                }

                // compensate remaining offset to get a full day
                fileOffset = (fileOffset + compensation) % fileLength;
                bufferOffset += compensation;
                remainingBufferLength -= compensation;

                // update loop state
                currentBegin += periodPerFile;
            }

            return (data, statusSet);
        }

        protected override List<ProjectInfo> LoadProjects()
        {
            // (0) load versioning file
            var versioningFilePath = Path.Combine(this.RootPath, "versioning.json");

            var versioning = File.Exists(versioningFilePath)
                ? HdfVersioning.Load(versioningFilePath)
                : new HdfVersioning();

            // (1) find beginning of database
            var dataFolderPath = Path.Combine(this.RootPath, "DATA");
            Directory.CreateDirectory(dataFolderPath);

            var firstMonthString = Path.GetFileName(Directory.EnumerateDirectories(dataFolderPath).FirstOrDefault());

            if (firstMonthString == null)
                return new List<ProjectInfo>();

            var firstMonth = DateTime.ParseExact(firstMonthString, "yyyy-MM", CultureInfo.InvariantCulture);

            // (2) for each month
            var now = DateTime.UtcNow;
            var months = ((now.Year - firstMonth.Year) * 12) + now.Month - firstMonth.Month + 1;
            var currentMonth = firstMonth;

            var cacheFolderPath = Path.Combine(this.RootPath, "CACHE");
            var mainCacheFilePath = Path.Combine(cacheFolderPath, "main.json");
            Directory.CreateDirectory(cacheFolderPath);

            bool cacheChanged = false;

            for (int i = 0; i < months; i++)
            {
                var currentDataFolderPath = Path.Combine(dataFolderPath, currentMonth.ToString("yyyy-MM"));

                // (3) find available project ids by scanning file contents (optimized)
                List<string> projectIds;

                if (Directory.Exists(currentDataFolderPath))
                    projectIds = this.FindProjectIds(Path.Combine(dataFolderPath, currentMonth.ToString("yyyy-MM")));
                else
                    projectIds = new List<string>();

                // (4) find corresponding cache file
                var cacheFilePath = Path.Combine(cacheFolderPath, $"{currentMonth.ToString("yyyy-MM")}.json");
               
                List<ProjectInfo> cache;

                // (5.a) cache file exists
                if (File.Exists(cacheFilePath))
                {
                    cache = JsonSerializerHelper.Deserialize<List<ProjectInfo>>(cacheFilePath);
                    cache.ForEach(project => project.Initialize());

                    foreach (var projectId in projectIds)
                    {
                        var project = cache.FirstOrDefault(project => project.Id == projectId);

                        // project is in cache ...
                        if (project != null)
                        {
                            // ... but cache is outdated
                            if (this.IsCacheOutdated(projectId, currentDataFolderPath, versioning))
                            {
                                project = this.ScanFiles(projectId, currentDataFolderPath, versioning);
                                cacheChanged = true;
                            }
                        }
                        // project is not in cache
                        else
                        {
                            project = this.ScanFiles(projectId, currentDataFolderPath, versioning);
                            cache.Add(project);
                            cacheChanged = true;
                        }
                    }
                }
                // (5.b) cache file does not exist
                else
                {
                    cache = projectIds.Select(projectId =>
                    {
                        var project = this.ScanFiles(projectId, currentDataFolderPath, versioning);
                        cacheChanged = true;
                        return project;
                    }).ToList();
                }

                // (6) save cache and versioning files
                if (cacheChanged)
                {
                    JsonSerializerHelper.Serialize(cache, cacheFilePath);
                    JsonSerializerHelper.Serialize(versioning, versioningFilePath);
                }

                currentMonth = currentMonth.AddMonths(1);
            }

            // (7) update main cache
            List<ProjectInfo> mainCache;

            if (cacheChanged || !File.Exists(mainCacheFilePath))
            {
                var cacheFiles = Directory.EnumerateFiles(cacheFolderPath, "*-*.json");
                mainCache = new List<ProjectInfo>();

                var message = "Merging cache files into main cache ...";

                try
                {
                    this.Logger.LogInformation(message);

                    foreach (var cacheFile in cacheFiles)
                    {
                        var cache = JsonSerializerHelper.Deserialize<List<ProjectInfo>>(cacheFile);
                        cache.ForEach(project => project.Initialize());

                        foreach (var project in cache)
                        {
                            var reference = mainCache.FirstOrDefault(current => current.Id == project.Id);

                            if (reference != null)
                                reference.Merge(project, ChannelMergeMode.NewWins);
                            else
                                mainCache.Add(project);
                        }
                    }

                    this.Logger.LogInformation($"{message} Done.");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"{message} Error: {ex.GetFullMessage()}");
                    throw;
                }

                JsonSerializerHelper.Serialize(mainCache, mainCacheFilePath);
            }
            else
            {
                mainCache = JsonSerializerHelper.Deserialize<List<ProjectInfo>>(mainCacheFilePath);
                mainCache.ForEach(project => project.Initialize());
            }

            // update project start and end
            foreach (var project in mainCache)
            {
                var searchPattern = $"{this.ToUnderscoredId(project.Id)}*.h5";
                var files = Directory.EnumerateFiles(dataFolderPath, searchPattern, SearchOption.AllDirectories);
                var firstDateTime = this.GetFirstDateTime(files);

                project.ProjectStart = firstDateTime;
                project.ProjectEnd = versioning.ScannedUntilMap[project.Id].AddDays(1);
            }

            return mainCache;
        }

        protected override double GetAvailability(string projectId, DateTime day)
        {
            if (!this.Projects.Any(project => project.Id == projectId))
                throw new Exception($"The project '{projectId}' could not be found.");

            var fileNamePattern = $"{this.ToUnderscoredId(projectId)}*{day.ToISO8601()}.h5";
            var folderName = day.ToString("yyyy-MM");
            var folderPath = Path.Combine(this.RootPath, "DATA", folderName);

            int fileCount;

            if (Directory.Exists(folderPath))
                fileCount = Directory.EnumerateFiles(folderPath, fileNamePattern, SearchOption.AllDirectories).Count();
            else
                fileCount = 0;

            return fileCount > 0 ? 1 : 0;
        }

        public override void Dispose()
        {
            if (_copyToLocal)
                this.HandleCopyToLocal(new List<string>());

            base.Dispose();
        }

        private List<string> HandleCopyToLocal(List<string> filePaths)
        {
            if (_lastLocalFilePaths == null)
                _lastLocalFilePaths = new List<string>();

            var targetDirectory = Path.Combine(Path.GetTempPath(), "OneDAS Explorer", "CopyToLocal");
            Directory.CreateDirectory(targetDirectory);

            // build target file paths
            var targetFilePaths = filePaths.Select(current
                => Path.Combine(targetDirectory, Path.GetFileName(current)));

            // if non of the file paths is contained in the local file list
            if (!targetFilePaths.Any(current => _lastLocalFilePaths.Contains(current)))
            {
                // delete files in that list first
                foreach (var localFilePath in _lastLocalFilePaths)
                {
                    try
                    {
                        if (File.Exists(localFilePath))
                            File.Delete(localFilePath);
                    }
                    catch
                    {
                        throw;
                    }
                }

                _lastLocalFilePaths.Clear();
            }

            // no copy each file
            foreach (var sourceFilePath in filePaths)
            {
                var targetFilePath = Path.Combine(targetDirectory, Path.GetFileName(sourceFilePath));

                // if that path is not yet in the list
                if (!_lastLocalFilePaths.Contains(targetFilePath))
                {
                    // then copy file and add it to the list
                    try
                    {
                        if (!File.Exists(targetFilePath))
                            File.Copy(sourceFilePath, targetFilePath);

                        _lastLocalFilePaths.Add(targetFilePath);
                    }
                    catch
                    {
                        if (File.Exists(targetFilePath))
                            File.Delete(targetFilePath);

                        throw;
                    }
                }
            }

            return _lastLocalFilePaths;
        }

        private List<string> FindProjectIds(string dataFolderPath)
        {
            var distinctFiles = Directory
                .EnumerateFiles(dataFolderPath, "*.h5")
                .Select(filePath =>
                {
                    var fileName = Path.GetFileName(filePath);
                    return fileName.Substring(0, fileName.Length - 24);
                })
                .Distinct();

            return distinctFiles.Select(distinctFile =>
            {
                var filePath = Directory.EnumerateFiles(dataFolderPath, $"{distinctFile}*.h5").First();

                try
                {
                    this.FileAccessManager?.Register(filePath, CancellationToken.None);
                    var projectId = GeneralHelper.GetProjectIdFromFile(filePath);
                    return projectId;
                }
                finally
                {
                    this.FileAccessManager?.Unregister(filePath);
                }
            }).Distinct().ToList();
        }

        private bool IsCacheOutdated(string projectId, string dataFolderPath, HdfVersioning versioning)
        {
            var searchPattern = $"{this.ToUnderscoredId(projectId)}*.h5";
            var files = Directory.EnumerateFiles(dataFolderPath, searchPattern);
            var lastDateTime = this.GetLastDateTime(files);
            return lastDateTime > versioning.ScannedUntilMap[projectId];
        }

        private ProjectInfo ScanFiles(string projectId, string dataFolderPath, HdfVersioning versioning)
        {
            var message = $"Scanning files for {Path.GetFileName(dataFolderPath)} ({projectId}) ...";
            var searchPattern = $"{this.ToUnderscoredId(projectId)}*.h5";
            var filePaths = Directory.EnumerateFiles(dataFolderPath, searchPattern);
            var project = new ProjectInfo(projectId);

            this.Logger.LogInformation(message);

            try
            {
                foreach (var filePath in filePaths)
                {
                    long fileId = 0;

                    try
                    {
                        this.FileAccessManager?.Register(filePath, CancellationToken.None);
                        fileId = H5F.open(filePath, H5F.ACC_RDONLY);

                        if (H5I.is_valid(fileId) > 0)
                        {
                            var newProject = GeneralHelper.GetProject(fileId, projectId);
                            project.Merge(newProject, ChannelMergeMode.NewWins);
                        }
                    }
                    finally
                    {
                        if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
                        this.FileAccessManager?.Unregister(filePath);
                    }
                }

                // update scanned until
                var scannedUntil = this.GetLastDateTime(filePaths);

                if (versioning.ScannedUntilMap.TryGetValue(projectId, out var value))
                {
                    if (scannedUntil > value)
                        versioning.ScannedUntilMap[projectId] = scannedUntil;
                }
                else
                {
                    versioning.ScannedUntilMap[projectId] = scannedUntil;
                }

                this.Logger.LogInformation($"{message} Done.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"{message} Error: {ex.GetFullMessage()}");
            }

            return project;
        }

        private DateTime GetFirstDateTime(IEnumerable<string> files)
        {
            if (files.Any())
            {
                var firstFile = files.First();
                var dateString = firstFile.Substring(firstFile.Length - 23, 20);
                var dateTime = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                return dateTime.ToUniversalTime();
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        private DateTime GetLastDateTime(IEnumerable<string> files)
        {
            if (files.Any())
            {
                var lastFile = files.Last();
                var dateString = lastFile.Substring(lastFile.Length - 23, 20);
                var dateTime = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                return dateTime.ToUniversalTime();
            }
            else
            {
                return DateTime.MaxValue;
            }
        }

        private string ToUnderscoredId(string projectId)
        {
            return projectId.Replace('/', '_').TrimStart('_');
        }

        #endregion
    }
}
