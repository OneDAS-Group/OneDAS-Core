using HDF.PInvoke;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Hdf.Core;
using OneDas.Hdf.Explorer.Core;
using OneDas.Hdf.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneDas.Infrastructure;

namespace OneDas.Hdf.Explorer.Web
{
    public class Broadcaster : Hub<IBroadcaster>
    {
        #region "Fields"

        private ILogger _logger;
        private HdfExplorerOptions _options;
        private HdfExplorerStateManager _stateManager;

        #endregion

        #region "Constructors"

        public Broadcaster(HdfExplorerStateManager stateManager, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options)
        {
            _stateManager = stateManager;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger("HDF Explorer");
        }

        #endregion

        #region "Methods"

        public override Task OnConnectedAsync()
        {
            _stateManager.Register(this.Context.ConnectionId);
            _logger.LogInformation($"{ this.Context.GetHttpContext().Connection.RemoteIpAddress } ({ this.Context.ConnectionId.Substring(0, 8) }) connected. { _stateManager.UserCount } client(s) are connected now.");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _stateManager.Unregister(this.Context.ConnectionId);
            _logger.LogInformation($"{ this.Context.GetHttpContext().Connection.RemoteIpAddress } ({ this.Context.ConnectionId.Substring(0, 8) }) disconnected. { _stateManager.UserCount } client(s) are remaining.");

            return base.OnDisconnectedAsync(exception);
        }

        public Task<AppModel> GetAppModel()
        {
            return Task.Run(() =>
            {
                return new AppModel(
                    hdfExplorerState: _stateManager.GetState(this.Context.ConnectionId),
                    campaignInfoSet: Program.CampaignInfoSet,
                    campaignDescriptionSet: Program.CampaignDescriptionSet
                );
            });
        }

        public Task<HdfExplorerState> GetHdfExplorerState()
        {
            return Task.Run(() =>
            {
                return _stateManager.GetState(this.Context.ConnectionId);
            });
        }

        public Task<List<CampaignInfo>> UpdateCampaignInfoSet()
        {
            return Task.Run(() =>
            {
                this.CheckState();

                _stateManager.SetState(this.Context.ConnectionId, HdfExplorerState.Updating);

                Program.UpdateCampaignInfoSet();

                _stateManager.SetState(this.Context.ConnectionId, HdfExplorerState.Idle);

                return Program.CampaignInfoSet;
            });
        }

        public Task<Dictionary<string, string>> GetCampaignDescriptionSet()
        {
            return Task.Run(() =>
            {
                return Program.CampaignDescriptionSet;
            });
        }

        public Task<string> GetData(DateTime dateTimeBegin, DateTime dateTimeEnd, string sampleRateDescription, FileFormat fileFormat, FileGranularity fileGranularity, Dictionary<string, Dictionary<string, List<string>>> campaignInfoSet)
        {
            long fileId = -1;
            long datasetId = -1;

            ulong start;
            ulong stride;
            ulong block;
            ulong count;

            ulong segmentLength;
            ulong segmentSize;
            ulong bytesPerRow;

            double sampleRate;

            DateTime epochStart;
            DateTime epochEnd;

            string zipFilePath;

            // task 
            return Task.Run(() =>
            {
                this.CheckState();

                if (!campaignInfoSet.Any())
                {
                    return string.Empty;
                }

                // zip file
                zipFilePath = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{ dateTimeBegin.ToString("yyyy-MM-ddTHH-mm") }_{ sampleRateDescription }_{ Guid.NewGuid().ToString() }.zip");

                // sampleRate
                sampleRate = sampleRateDescription.ToSampleRate();

                // epoch & hyperslab
                epochStart = new DateTime(2000, 01, 01);
                epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                {
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");
                }

                start = (ulong)(Math.Floor((dateTimeBegin - epochStart).TotalSeconds * sampleRate));
                stride = 1;
                block = (ulong)(Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalSeconds * sampleRate));
                count = 1;

                try
                {
                    // open file
                    fileId = H5F.open(_options.VdsFilePath, H5F.ACC_RDONLY);

                    // byte count
                    bytesPerRow = 0;

                    foreach (var campaignInfo in campaignInfoSet)
                    {
                        foreach (var variableInfo in campaignInfo.Value)
                        {
                            foreach (string datasetInfo in variableInfo.Value)
                            {
                                try
                                {
                                    datasetId = H5D.open(fileId, $"{ campaignInfo.Key }/{ variableInfo.Key }/{ datasetInfo }");
                                    bytesPerRow += (ulong)OneDasUtilities.SizeOf(TypeConversionHelper.GetTypeFromHdfTypeId(H5D.get_type(datasetId)));
                                }
                                finally
                                {
                                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                                }
                            }
                        }
                    }

                    this.GetClient().SendByteCount(bytesPerRow * block);

                    segmentSize = (50 * 1024 * 1024) / bytesPerRow * bytesPerRow;
                    segmentLength = segmentSize / bytesPerRow;

                    // ensure that dataset length is multiple of 1 minute
                    if ((segmentLength / sampleRate) % 60 != 0)
                    {
                        segmentLength = (ulong)((ulong)(segmentLength / sampleRate / 60) * 60 * sampleRate);
                    }

                    // start
                    _stateManager.SetState(this.Context.ConnectionId, HdfExplorerState.Loading);

                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var campaignInfo in campaignInfoSet)
                        {
                            HdfDataLoader hdfDataLoader;

                            hdfDataLoader = new HdfDataLoader(_stateManager.GetToken(this.Context.ConnectionId));
                            hdfDataLoader.ProgressUpdated += this.OnProgressUpdated;

                            if (!hdfDataLoader.WriteZipFileCampaignEntry(zipArchive, fileGranularity, fileFormat, new ZipSettings(dateTimeBegin, campaignInfo, fileId, sampleRate, start, stride, block, count, segmentLength)))
                            {
                                return string.Empty;
                            }
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
                    _stateManager.SetState(this.Context.ConnectionId, HdfExplorerState.Idle);

                    if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
                }

                this.WriteLogEntry($"{ this.Context.GetHttpContext().Connection.RemoteIpAddress } requested data: { dateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss") } to { dateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss") }", false);

                return $"download/{ Path.GetFileName(zipFilePath) }";
            }, _stateManager.GetToken(this.Context.ConnectionId));
        }

        public Task<string> GetCampaignDocumentation(string campaigInfoName)
        {
            long vdsMetaFileId = -1;

            string csvFileName;

            CampaignInfo campaignInfo;
            List<string> groupNameSet;

            campaignInfo = Program.CampaignInfoSet.First(campaign => campaign.Name == campaigInfoName);

            return Task.Run(() =>
            {
                this.CheckState();

                try
                {
                    if (File.Exists(_options.VdsMetaFilePath))
                    {
                        vdsMetaFileId = H5F.open(_options.VdsMetaFilePath, H5F.ACC_RDONLY);
                    }

                    csvFileName = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{ campaignInfo.Name.ToLower().Replace("/", "_").TrimStart('_') }_{ Guid.NewGuid().ToString() }.csv");
                    groupNameSet = campaignInfo.VariableInfoSet.SelectMany(variableInfo => variableInfo.VariableGroupSet.Last().Split('\n')).Distinct().ToList();

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

                                string transferFunction;
                                string aggregateFunction;
                                string groupPath;
                                string name;
                                string guid;
                                string unit;

                                List<hdf_transfer_function_t> transferFunctionSet;
                                List<hdf_aggregate_function_t> aggregateFunctionSet;

                                name = variableInfo.VariableNameSet.Last();
                                guid = variableInfo.Name;
                                unit = string.Empty;
                                transferFunctionSet = new List<hdf_transfer_function_t>();
                                aggregateFunctionSet = new List<hdf_aggregate_function_t>();

                                try
                                {
                                    groupPath = GeneralHelper.CombinePath(campaignInfo.Name, variableInfo.Name);

                                    if (H5I.is_valid(vdsMetaFileId) > 0 && IOHelper.CheckLinkExists(vdsMetaFileId, groupPath))
                                    {
                                        variable_groupId = H5G.open(vdsMetaFileId, groupPath);

                                        if (H5A.exists(variable_groupId, "unit") > 0)
                                        {
                                            unit = IOHelper.ReadAttribute<string>(variable_groupId, "unit").FirstOrDefault();
                                        }

                                        if (H5A.exists(variable_groupId, "transfer_function_set") > 0)
                                        {
                                            transferFunctionSet = IOHelper.ReadAttribute<hdf_transfer_function_t>(variable_groupId, "transfer_function_set").ToList();
                                        }

                                        if (H5A.exists(variable_groupId, "aggregate_function_set") > 0)
                                        {
                                            aggregateFunctionSet = IOHelper.ReadAttribute<hdf_aggregate_function_t>(variable_groupId, "aggregate_function_set").ToList();
                                        }
                                    }
                                }
                                finally
                                {
                                    if (H5I.is_valid(variable_groupId) > 0) { H5G.close(variable_groupId); }
                                }

                                // transfer function
                                transferFunction = string.Empty;

                                transferFunctionSet.ForEach(tf =>
                                {
                                    if (!string.IsNullOrWhiteSpace(transferFunction))
                                    {
                                        transferFunction += " | ";
                                    }

                                    transferFunction += $"{ tf.date_time }, { tf.type }, { tf.option }, { tf.argument }";
                                });

                                transferFunction = $"\"{ transferFunction }\"";

                                // aggregate function
                                aggregateFunction = string.Empty;

                                aggregateFunctionSet.ForEach(af =>
                                {
                                    if (!string.IsNullOrWhiteSpace(aggregateFunction))
                                    {
                                        aggregateFunction += " | ";
                                    }

                                    aggregateFunction += $"{ af.type }, { af.argument }";
                                });

                                aggregateFunction = $"\"{ aggregateFunction  }\"";

                                // write row
                                streamWriter.WriteLine($"{ groupName };{ name };{ unit };{ transferFunction };{ aggregateFunction };{ guid }");
                            });
                        }
                    }

                    return $"download/{ Path.GetFileName(csvFileName) }";
                }
                finally
                {
                    if (H5I.is_valid(vdsMetaFileId) > 0) { H5F.close(vdsMetaFileId); }
                }
            });
        }

        public Task<string> GetInactivityMessage()
        {
            return Task.Run(() =>
            {
                DateTime startTime;
                DateTime stopTime;

                startTime = DateTime.UtcNow.Date.Add(_options.InactiveOn);
                stopTime = startTime.Add(_options.InactivityPeriod);

                return $"The database is offline for scheduled maintenance from { startTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture) } to { stopTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture) } UTC.";
            });
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            long fileId = -1;

            ulong lengthPerDay;
            ulong offset;

            ulong start;
            ulong stride;
            ulong block;
            ulong count;

            double totalDays;

            int[] data;
            int[] aggregatedData;

            DateTime epochStart;
            DateTime epochEnd;

            DataAvailabilityGranularity granularity;

            return Task.Run(() =>
            {
                this.CheckState();

                // open file
                fileId = H5F.open(_options.VdsFilePath, H5F.ACC_RDONLY);

                // epoch & hyperslab
                epochStart = new DateTime(2000, 01, 01);
                epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                {
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");
                }

                lengthPerDay = OneDasUtilities.GetSamplesPerDayFromString("is_chunk_completed_set");

                start = (ulong)(Math.Floor((dateTimeBegin - epochStart).TotalDays * lengthPerDay));
                stride = 1;
                block = (ulong)(Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalDays * lengthPerDay));
                count = 1;

                // get data
                totalDays = (dateTimeEnd - dateTimeBegin).TotalDays;
                data = IOHelper.ReadDataset<byte>(fileId, $"{ campaignName }/is_chunk_completed_set", start, stride, block, count).Select(value => (int)value).ToArray();

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
                        ulong startIndex; // inclusive
                        ulong endIndex; // exclusive

                        startIndex = (ulong)day * lengthPerDay;
                        endIndex = startIndex + lengthPerDay;

                        if ((int)startIndex - (int)offset < 0)
                        {
                            startIndex = 0;
                        }
                        else
                        {
                            startIndex = startIndex - offset;
                        }

                        if (endIndex - offset >= (ulong)data.Length)
                        {
                            endIndex = (ulong)data.Length;
                        }
                        else
                        {
                            endIndex = endIndex - offset;
                        }

                        aggregatedData[day] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                    });
                }
                else
                {
                    int totalMonths;
                    DateTime totalDateTimeBegin;

                    totalMonths = (dateTimeEnd.Month - dateTimeBegin.Month) + 1 + 12 * (dateTimeEnd.Year - dateTimeBegin.Year);
                    totalDateTimeBegin = new DateTime(dateTimeBegin.Year, dateTimeBegin.Month, 1);
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
                        {
                            startIndex = 0;
                        }
                        else
                        {
                            startIndex = (ulong)(currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset;
                        }

                        if ((currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset >= data.Length)
                        {
                            endIndex = (ulong)data.Length;
                        }
                        else
                        {
                            endIndex = (ulong)(currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset;
                        }

                        aggregatedData[month] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                    });
                }

                // clean up
                H5F.close(fileId);

                return new DataAvailabilityStatistics(granularity, aggregatedData);
            });
        }

        public Task CancelGetData()
        {
            return Task.Run(() =>
            {
                _stateManager.Cancel(this.Context.ConnectionId);
            });
        }

        private void CheckState()
        {
            switch (_stateManager.GetState(this.Context.ConnectionId))
            {
                case HdfExplorerState.Inactive:
                    throw new Exception("HDF Explorer is in scheduled inactivity mode.");

                case HdfExplorerState.Updating:
                    throw new Exception("The database is currently being updated.");

                case HdfExplorerState.Loading:
                    throw new Exception("Data request is already in progress.");

                default:
                    break;
            }
        }

        private IBroadcaster GetClient()
        {
            return this.Clients.Client(this.Context.ConnectionId);
        }

        private void OnProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.GetClient().SendProgress(e.Progress, e.Message);
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

            // file
            try
            {
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine($"{ DateTime.UtcNow.ToString("yyyy-MM:dd hh:mm:ss") }: { message }\n");
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        #endregion
    }
}
