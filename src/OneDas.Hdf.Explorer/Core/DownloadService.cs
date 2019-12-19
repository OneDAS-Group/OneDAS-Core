using HDF.PInvoke;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Hdf.Core;
using OneDas.Hdf.Explorer.Web;
using OneDas.Hdf.IO;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer.Core
{
    public class DownloadService
    {
        private ILogger _logger;
        private HdfExplorerStateManager _stateManager;
        private IHubContext<Broadcaster> _context;
        private HdfExplorerOptions _options;
        private string _connectionId;
        private object _lock;

        public DownloadService(HdfExplorerStateManager stateManager, IHubContext<Broadcaster> context, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options, string connectionId)
        {
            _stateManager = stateManager;
            _context = context;
            _logger = loggerFactory.CreateLogger("HDF Explorer");
            _options = options.Value;
            _connectionId = connectionId;
            _lock = new object();
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            long fileId = -1;

            ulong offset;
            int[] aggregatedData;
            DataAvailabilityGranularity granularity;

            return Task.Run(() =>
            {
                _stateManager.CheckState(_connectionId);

                // open file
                fileId = H5F.open(_options.VdsFilePath, H5F.ACC_RDONLY);

                // epoch & hyperslab
                var epochStart = new DateTime(2000, 01, 01);
                var epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

                var samplesPerDay = new SampleRateContainer("is_chunk_completed_set").SamplesPerDay;

                var start = (ulong)Math.Floor((dateTimeBegin - epochStart).TotalDays * samplesPerDay);
                var stride = 1UL;
                var block = (ulong)Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalDays * samplesPerDay);
                var count = 1UL;

                // get data
                var totalDays = (dateTimeEnd - dateTimeBegin).TotalDays;
                var data = IOHelper.ReadDataset<byte>(fileId, $"{ campaignName }/is_chunk_completed_set", start, stride, block, count).Select(value => (int)value).ToArray();

                if (totalDays <= 7)
                {
                    granularity = DataAvailabilityGranularity.ChunkLevel;
                    aggregatedData = data;
                }
                else if (totalDays <= 365)
                {
                    granularity = DataAvailabilityGranularity.DayLevel;
                    offset = (ulong)dateTimeBegin.TimeOfDay.TotalMinutes;
                    aggregatedData = new int[(int)Math.Ceiling(totalDays)];

                    Parallel.For(0, (int)Math.Ceiling(totalDays), day =>
                    {
                        var startIndex = (ulong)day * samplesPerDay; // inclusive
                        var endIndex = startIndex + samplesPerDay; // exclusive

                        if ((int)startIndex - (int)offset < 0)
                            startIndex = 0;
                        else
                            startIndex = startIndex - offset;

                        if (endIndex - offset >= (ulong)data.Length)
                            endIndex = (ulong)data.Length;
                        else
                            endIndex = endIndex - offset;

                        aggregatedData[day] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                    });
                }
                else
                {
                    var totalMonths = (dateTimeEnd.Month - dateTimeBegin.Month) + 1 + 12 * (dateTimeEnd.Year - dateTimeBegin.Year);
                    var totalDateTimeBegin = new DateTime(dateTimeBegin.Year, dateTimeBegin.Month, 1);

                    granularity = DataAvailabilityGranularity.MonthLevel;
                    offset = (ulong)(dateTimeBegin - totalDateTimeBegin).TotalMinutes;
                    aggregatedData = new int[totalMonths];

                    Parallel.For(0, totalMonths, month =>
                    {
                        ulong startIndex; // inclusive
                        ulong endIndex; // exclusive

                        DateTime currentDateTimeBegin;
                        DateTime currentDateTimeEnd;

                        currentDateTimeBegin = totalDateTimeBegin.AddMonths(month);
                        currentDateTimeEnd = currentDateTimeBegin.AddMonths(1);

                        if ((currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset < 0)
                            startIndex = 0;
                        else
                            startIndex = (ulong)(currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset;

                        if ((currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset >= data.Length)
                            endIndex = (ulong)data.Length;
                        else
                            endIndex = (ulong)(currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset;

                        aggregatedData[month] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                    });
                }

                // clean up
                H5F.close(fileId);

                return new DataAvailabilityStatistics(granularity, aggregatedData);
            });
        }

        public async Task Download(ChannelWriter<string> writer, IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, FileFormat fileFormat, FileGranularity fileGranularity, string sampleRateWithUnit, string campaignPath, List<string> variableNameSet)
        {
            try
            {
                var campaignInfo = Program.CampaignInfoSet.FirstOrDefault(current => current.Name == campaignPath);

                if (campaignInfo == null)
                    throw new Exception($"Could not find campaign with path '{campaignPath}'.");

                var campaignInfoSet = new Dictionary<string, Dictionary<string, List<string>>>();
                campaignInfoSet[campaignInfo.Name] = new Dictionary<string, List<string>>();

                foreach (var variableName in variableNameSet)
                {
                    VariableInfo variableInfo;

                    variableInfo = campaignInfo.VariableInfoSet.FirstOrDefault(current => current.VariableNameSet.Contains(variableName));

                    if (campaignInfo == null)
                        throw new Exception($"Could not find variable with name '{variableName}' in campaign '{campaignInfo.Name}'.");

                    if (!variableInfo.DatasetInfoSet.Any(current => current.Name == sampleRateWithUnit))
                        throw new Exception($"Could not find dataset in variable with ID '{variableInfo.Name}' ({variableInfo.VariableNameSet.First()}) in campaign '{campaignInfo.Name}'.");

                    campaignInfoSet[campaignInfo.Name][variableInfo.Name] = new List<string>() { sampleRateWithUnit };
                }

                var url = await this.GetData(remoteIpAddress, dateTimeBegin, dateTimeEnd, new SampleRateContainer(sampleRateWithUnit), fileFormat, fileGranularity, campaignInfoSet);
                await writer.WriteAsync(url);
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
                throw;
            }

            writer.TryComplete();
        }

        public Task<string> GetData(IPAddress remoteIpAddress, DateTime dateTimeBegin, DateTime dateTimeEnd, SampleRateContainer sampleRate, FileFormat fileFormat, FileGranularity fileGranularity, Dictionary<string, Dictionary<string, List<string>>> campaignInfoSet)
        {
            long fileId = -1;
            long datasetId = -1;

            // task 
            return Task.Run(() =>
            {
                _stateManager.CheckState(_connectionId);

                if (!campaignInfoSet.Any() || dateTimeBegin == dateTimeEnd)
                    return string.Empty;

                // zip file
                var zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{dateTimeBegin.ToString("yyyy-MM-ddTHH-mm")}_{sampleRate.ToUnitString(underscore: true)}_{Guid.NewGuid().ToString()}.zip");

                // sampleRate
                var samplesPerSecond = sampleRate.SamplesPerSecond;

                // epoch & hyperslab
                var epochStart = new DateTime(2000, 01, 01);
                var epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                {
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");
                }

                var start = (ulong)(Math.Floor((dateTimeBegin - epochStart).TotalSeconds * samplesPerSecond));
                var stride = 1UL;
                var block = (ulong)(Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalSeconds * samplesPerSecond));
                var count = 1UL;

                try
                {
                    // open file
                    fileId = H5F.open(_options.VdsFilePath, H5F.ACC_RDONLY);

                    // byte count
                    var bytesPerRow = 0UL;

                    foreach (var campaignInfo in campaignInfoSet)
                    {
                        foreach (var variableInfo in campaignInfo.Value)
                        {
                            foreach (string datasetInfo in variableInfo.Value)
                            {
                                try
                                {
                                    datasetId = H5D.open(fileId, $"{campaignInfo.Key}/{variableInfo.Key}/{datasetInfo}");
                                    bytesPerRow += (ulong)OneDasUtilities.SizeOf(TypeConversionHelper.GetTypeFromHdfTypeId(H5D.get_type(datasetId)));
                                }
                                finally
                                {
                                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                                }
                            }
                        }
                    }

                    this.GetClient().SendAsync("SendByteCount", bytesPerRow * block);

                    var segmentSize = (50 * 1024 * 1024) / bytesPerRow * bytesPerRow;
                    var segmentLength = segmentSize / bytesPerRow;

                    // ensure that dataset length is multiple of 1 minute
                    if ((segmentLength / samplesPerSecond) % 60 != 0)
                        segmentLength = (ulong)((ulong)(segmentLength / samplesPerSecond / 60) * 60 * samplesPerSecond);

                    // start
                    _stateManager.SetState(_connectionId, HdfExplorerState.Loading);

                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var campaignInfo in campaignInfoSet)
                        {
                            HdfDataLoader hdfDataLoader;

                            hdfDataLoader = new HdfDataLoader(_stateManager.GetToken(_connectionId));
                            hdfDataLoader.ProgressUpdated += this.OnProgressUpdated;

                            if (!hdfDataLoader.WriteZipFileCampaignEntry(zipArchive, fileGranularity, fileFormat, new ZipSettings(dateTimeBegin, campaignInfo, fileId, samplesPerSecond, start, stride, block, count, segmentLength)))
                                return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteLogEntry(ex.Message, true);
                    throw;
                }
                finally
                {
                    _stateManager.SetState(_connectionId, HdfExplorerState.Idle);

                    if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
                }

                this.WriteLogEntry($"{ remoteIpAddress } requested data: { dateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss") } to { dateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss") }", false);
                return $"download/{ Path.GetFileName(zipFilePath) }";
            }, _stateManager.GetToken(_connectionId));
        }

        public Task<string> GetCampaignDocumentation(string campaigInfoName)
        {
            long vdsMetaFileId = -1;

            var campaignInfo = Program.CampaignInfoSet.First(campaign => campaign.Name == campaigInfoName);

            return Task.Run(() =>
            {
                _stateManager.CheckState(_connectionId);

                try
                {
                    if (File.Exists(_options.VdsMetaFilePath))
                    {
                        vdsMetaFileId = H5F.open(_options.VdsMetaFilePath, H5F.ACC_RDONLY);
                    }

                    var csvFileName = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{ campaignInfo.Name.ToLower().Replace("/", "_").TrimStart('_') }_{ Guid.NewGuid().ToString() }.csv");
                    var groupNameSet = campaignInfo.VariableInfoSet.SelectMany(variableInfo => variableInfo.VariableGroupSet.Last().Split('\n')).Distinct().ToList();

                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(csvFileName, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
                    {
                        // campaign header
                        streamWriter.WriteLine($"# { campaignInfo.Name.TrimStart('/').Replace("/", " / ") }");

                        // campaign description
                        streamWriter.WriteLine($"# { Program.CampaignDescriptionSet[campaigInfoName] }");

                        // header
                        streamWriter.WriteLine("Group;Name;Unit;Transfer function;Aggregate function;Guid");

                        // groups
                        foreach (string groupName in groupNameSet)
                        {
                            List<VariableInfo> groupedVariableInfoSet;

                            groupedVariableInfoSet = campaignInfo.VariableInfoSet.Where(variableInfo => variableInfo.VariableGroupSet.Last().Split('\n').Contains(groupName)).OrderBy(variableInfo => variableInfo.VariableNameSet.Last()).ToList();

                            // variables
                            groupedVariableInfoSet.ForEach(variableInfo =>
                            {
                                long variable_groupId = -1;

                                var name = variableInfo.VariableNameSet.Last();
                                var guid = variableInfo.Name;
                                var unit = string.Empty;
                                var transferFunctionSet = new List<hdf_transfer_function_t>();
                                var aggregateFunctionSet = new List<hdf_aggregate_function_t>();

                                try
                                {
                                    var groupPath = GeneralHelper.CombinePath(campaignInfo.Name, variableInfo.Name);

                                    if (H5I.is_valid(vdsMetaFileId) > 0 && IOHelper.CheckLinkExists(vdsMetaFileId, groupPath))
                                    {
                                        variable_groupId = H5G.open(vdsMetaFileId, groupPath);

                                        if (H5A.exists(variable_groupId, "unit") > 0)
                                            unit = IOHelper.ReadAttribute<string>(variable_groupId, "unit").FirstOrDefault();

                                        if (H5A.exists(variable_groupId, "transfer_function_set") > 0)
                                            transferFunctionSet = IOHelper.ReadAttribute<hdf_transfer_function_t>(variable_groupId, "transfer_function_set").ToList();

                                        if (H5A.exists(variable_groupId, "aggregate_function_set") > 0)
                                            aggregateFunctionSet = IOHelper.ReadAttribute<hdf_aggregate_function_t>(variable_groupId, "aggregate_function_set").ToList();
                                    }
                                }
                                finally
                                {
                                    if (H5I.is_valid(variable_groupId) > 0) { H5G.close(variable_groupId); }
                                }

                                // transfer function
                                var transferFunction = string.Empty;

                                transferFunctionSet.ForEach(tf =>
                                {
                                    if (!string.IsNullOrWhiteSpace(transferFunction))
                                        transferFunction += " | ";

                                    transferFunction += $"{tf.date_time}, {tf.type}, {tf.option}, {tf.argument}";
                                });

                                transferFunction = $"\"{transferFunction}\"";

                                // aggregate function
                                var aggregateFunction = string.Empty;

                                aggregateFunctionSet.ForEach(af =>
                                {
                                    if (!string.IsNullOrWhiteSpace(aggregateFunction))
                                    {
                                        aggregateFunction += " | ";
                                    }

                                    aggregateFunction += $"{af.type}, {af.argument}";
                                });

                                aggregateFunction = $"\"{aggregateFunction}\"";

                                // write row
                                streamWriter.WriteLine($"{groupName};{name};{unit};{transferFunction};{aggregateFunction};{guid}");
                            });
                        }
                    }

                    return $"download/{Path.GetFileName(csvFileName)}";
                }
                finally
                {
                    if (H5I.is_valid(vdsMetaFileId) > 0) { H5F.close(vdsMetaFileId); }
                }
            });
        }

        private void OnProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.GetClient().SendAsync("SendProgress", e.Progress, e.Message);
        }

        private IClientProxy GetClient()
        {
            return _context.Clients.Client(_connectionId);
        }

        private void WriteLogEntry(string message, bool isException)
        {
            string logFilePath;

            if (isException)
            {
                _logger.LogError(message);
                logFilePath = Path.Combine(_options.SupportDirectoryPath, "LOGS", "HDF Explorer", "errors.txt");
            }
            else
            {
                _logger.LogInformation(message);
                logFilePath = Path.Combine(_options.SupportDirectoryPath, "LOGS", "HDF Explorer", "requests.txt");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            // file
            lock (_lock)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.WriteLine($"{ DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }: { message }\n");
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
        }
    }
}
