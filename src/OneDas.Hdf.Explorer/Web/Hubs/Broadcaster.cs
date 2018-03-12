using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HDF.PInvoke;
using Microsoft.AspNetCore.SignalR;
using OneDas.Hdf.Core;
using OneDas.Hdf.Explorer.Core;
using OneDas.Hdf.IO;
using OneDas.Infrastructure;
using OneDas.Plugin;

namespace OneDas.Hdf.Explorer.Web
{
    public class Broadcaster : Hub<IBroadcaster>
    {
        private static Dictionary<string, HdfExplorerState> _hdfExplorerStateSet;
        private static Dictionary<string, CancellationTokenSource> _ctsSet;

        static Broadcaster()
        {
            _hdfExplorerStateSet = new Dictionary<string, HdfExplorerState>();
            _ctsSet = new Dictionary<string, CancellationTokenSource>();
        }

        public override Task OnConnectedAsync()
        {
            _hdfExplorerStateSet[this.Context.ConnectionId] = HdfExplorerState.Idle;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_ctsSet.ContainsKey(this.Context.ConnectionId))
            {
                _ctsSet[this.Context.ConnectionId].Cancel();
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task<AppModel> GetAppModel()
        {
            return Task.Run(() =>
            {
                return new AppModel(
                    hdfExplorerState: HdfExplorerState.Idle,
                    campaignInfoSet: Program.CampaignInfoSet,
                    campaignDescriptionSet: Program.CampaignDescriptionSet
                );
            });
        }

        public Task<List<CampaignInfo>> UpdateCampaignInfoSet()
        {
            return Task.Run(() =>
            {
                _hdfExplorerStateSet[this.Context.ConnectionId] = HdfExplorerState.Updating;
                this.Clients.Client(this.Context.ConnectionId).SendState(_hdfExplorerStateSet[this.Context.ConnectionId]);

                Program.UpdateCampaignInfoSet();

                _hdfExplorerStateSet[this.Context.ConnectionId] = HdfExplorerState.Idle;
                this.Clients.Client(this.Context.ConnectionId).SendState(_hdfExplorerStateSet[this.Context.ConnectionId]);

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
            _ctsSet[this.Context.ConnectionId] = new CancellationTokenSource();

            return Task.Run(() =>
            {
                if (!campaignInfoSet.Any())
                {
                    return string.Empty;
                }

                // zip file
                zipFilePath = Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "EXPORT", $"OneDAS_{ dateTimeBegin.ToString("yyyy-MM-ddTHH-mm") }_{ sampleRateDescription }_{ Guid.NewGuid().ToString().Substring(0, 8) }.zip");

                // sampleRate
                sampleRate = sampleRateDescription.ToSampleRate();

                // epoch & hyperslab
                epochStart = new DateTime(2017, 01, 01);
                epochEnd = new DateTime(2030, 01, 01);

                if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                {
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");
                }

                start = (ulong)(Math.Floor((dateTimeBegin - epochStart).TotalSeconds * sampleRate));
                stride = 1;
                block = (ulong)(Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalSeconds * sampleRate));
                count = 1;

                // bugfix
                start = start & 0xFFFFFFFF;

                // state check
                if (_hdfExplorerStateSet[this.Context.ConnectionId] == HdfExplorerState.Loading)
                {
                    throw new Exception("Data request is already in progress.");
                }

                // open file
                fileId = H5F.open(Program.VdsFilePath, H5F.ACC_RDONLY);

                // byte count
                bytesPerRow = 0;

                foreach (var campaignInfo in campaignInfoSet)
                {
                    foreach (var variableInfo in campaignInfo.Value)
                    {
                        foreach (string datasetInfo in variableInfo.Value)
                        {
                            datasetId = H5D.open(fileId, $"{ campaignInfo.Key }/{ variableInfo.Key }/{ datasetInfo }");
                            bytesPerRow += (ulong)OneDasUtilities.SizeOf(TypeConversionHelper.GetTypeFromHdfTypeId(H5D.get_type(datasetId)));

                            // clean up
                            H5D.close(datasetId);
                        }
                    }
                }

                this.Clients.Client(this.Context.ConnectionId).SendByteCount(bytesPerRow * block);

                segmentSize = (50 * 1024 * 1024) / bytesPerRow * bytesPerRow;
                segmentLength = segmentSize / bytesPerRow;

                // ensure that dataset length is multiple of 1 minute
                if ((segmentLength / sampleRate) % 60 != 0)
                {
                    segmentLength = (ulong)((ulong)(segmentLength / sampleRate / 60) * 60 * sampleRate);
                }

                // start
                try
                {
                    _hdfExplorerStateSet[this.Context.ConnectionId] = HdfExplorerState.Loading;
                    this.Clients.Client(this.Context.ConnectionId).SendState(_hdfExplorerStateSet[this.Context.ConnectionId]);

                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var campaignInfo in campaignInfoSet)
                        {
                            HdfDataLoader hdfDataLoader;

                            hdfDataLoader = new HdfDataLoader(_ctsSet[this.Context.ConnectionId]);
                            hdfDataLoader.ProgressUpdated += this.HdfDataLoader_ProgressUpdated;

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
                    _hdfExplorerStateSet[this.Context.ConnectionId] = HdfExplorerState.Idle;
                    this.Clients.Client(this.Context.ConnectionId).SendState(_hdfExplorerStateSet[this.Context.ConnectionId]);

                    if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
                }

                this.WriteLogEntry($"User { "xxx" } requested data from { dateTimeBegin.ToString("yyyy-MM-dd hh:mm:ss") } to { dateTimeEnd.ToString("yyyy-mm-dd hh:MM:ss") } ({ Path.GetFileName(zipFilePath) })", false);

                return $"home/download/?file={ Path.GetFileName(zipFilePath) }";
            }, _ctsSet[this.Context.ConnectionId].Token);
        }

        public Task CancelGetData()
        {
            return Task.Run(() =>
            {
                if (_ctsSet.ContainsKey(this.Context.ConnectionId))
                {
                    _ctsSet[this.Context.ConnectionId].Cancel();
                }
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
                // open file
                fileId = H5F.open(Program.VdsFilePath, H5F.ACC_RDONLY);

                // epoch & hyperslab
                epochStart = new DateTime(2017, 01, 01);
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

        private void HdfDataLoader_ProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.Clients.Client(this.Context.ConnectionId).SendProgress(e.Progress, e.Message);
        }

        private void WriteLogEntry(string message, bool isException)
        {
            string logFilePath;
            string formatedMessage;

            if (isException)
            {
                logFilePath = Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "HDF Explorer", "errors.txt");
            }
            else
            {
                logFilePath = Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "HDF Explorer", "requests.txt");
            }

            formatedMessage = $"{ DateTime.UtcNow.ToString("yyyy-MM:dd hh:mm:ss") }: { message }\n";

            // console
            Console.WriteLine(formatedMessage);

            // file
            try
            {
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine(formatedMessage);
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
