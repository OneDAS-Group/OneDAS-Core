using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Hdf.Core;
using OneDas.Hdf.Explorer.Web;
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

        public DownloadService(HdfExplorerStateManager stateManager, IHubContext<Broadcaster> context, ILoggerFactory loggerFactory, IOptions<HdfExplorerOptions> options, string connectionId)
        {
            _stateManager = stateManager;
            _context = context;
            _logger = loggerFactory.CreateLogger("HDF Explorer");
            _options = options.Value;
            _connectionId = connectionId;
        }

        public Task<DataAvailabilityStatistics> GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            return Task.Run(() =>
            {
                IDatabase database = null;

                _stateManager.CheckState(_connectionId);

                try
                {
                    database = new HdfDatabase(_options.DataBaseFolderPath);
                    return database.GetDataAvailabilityStatistics(campaignName, dateTimeBegin, dateTimeEnd);
                }
                finally
                {
                    database?.Dispose();
                }
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
            IDatabase database = null;

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
                    throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

                var start = (ulong)(Math.Floor((dateTimeBegin - epochStart).TotalSeconds * samplesPerSecond));
                var block = (ulong)(Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalSeconds * samplesPerSecond));

                try
                {
                    // open database
                    database = new HdfDatabase(_options.DataBaseFolderPath);

                    // byte count
                    var bytesPerRow = 0UL;

                    foreach (var campaignInfo in campaignInfoSet)
                    {
                        foreach (var variableInfo in campaignInfo.Value)
                        {
                            foreach (string datasetInfo in variableInfo.Value)
                            {
                                bytesPerRow = database.GetElementSize(campaignInfo.Key, variableInfo.Key, datasetInfo);
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

                            if (!hdfDataLoader.WriteZipFileCampaignEntry(zipArchive, fileGranularity, fileFormat, new ZipSettings(dateTimeBegin, campaignInfo, database, samplesPerSecond, start, block, segmentLength)))
                                return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
                finally
                {
                    _stateManager.SetState(_connectionId, HdfExplorerState.Idle);
                    database?.Dispose();
                }

                var message = $"{remoteIpAddress} requested data: {dateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss")} to {dateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss")}";
                _logger.LogInformation(message);

                return $"download/{Path.GetFileName(zipFilePath)}";
            }, _stateManager.GetToken(_connectionId));
        }

        public Task<string> GetCampaignDocumentation(string campaigInfoName)
        {
            return Task.Run(() =>
            {
                IDatabase database = null;

                var campaignInfo = Program.CampaignInfoSet.First(campaign => campaign.Name == campaigInfoName);

                _stateManager.CheckState(_connectionId);

                try
                {
                    database = new HdfDatabase(_options.DataBaseFolderPath);

                    var csvFileName = Path.Combine(_options.SupportDirectoryPath, "EXPORT", $"OneDAS_{ campaignInfo.Name.ToLower().Replace("/", "_").TrimStart('_') }_{ Guid.NewGuid().ToString() }.csv");
                    var groupNameSet = campaignInfo.VariableInfoSet.SelectMany(variableInfo => variableInfo.VariableGroupSet.Last().Split('\n')).Distinct().ToList();

                    using (var streamWriter = new StreamWriter(new FileStream(csvFileName, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
                    {
                        // campaign header
                        streamWriter.WriteLine($"# { campaignInfo.Name.TrimStart('/').Replace("/", " / ") }");

                        // campaign description
                        streamWriter.WriteLine($"# { Program.CampaignDescriptionSet[campaigInfoName] }");

                        // header
                        streamWriter.WriteLine("Group;Name;Unit;Transfer function;Guid");

                        // groups
                        foreach (string groupName in groupNameSet)
                        {
                            List<VariableInfo> groupedVariableInfoSet;

                            groupedVariableInfoSet = campaignInfo.VariableInfoSet.Where(variableInfo => variableInfo.VariableGroupSet.Last().Split('\n').Contains(groupName)).OrderBy(variableInfo => variableInfo.VariableNameSet.Last()).ToList();

                            // variables
                            groupedVariableInfoSet.ForEach(variableInfo =>
                            {
                                (var name, var guid, var unit, var transferFunctionSet) = database.GetDocumentation(campaignInfo, variableInfo);

                                // transfer function
                                var transferFunction = string.Empty;

                                transferFunctionSet.ForEach(tf =>
                                {
                                    if (!string.IsNullOrWhiteSpace(transferFunction))
                                        transferFunction += " | ";

                                    transferFunction += $"{tf.date_time}, {tf.type}, {tf.option}, {tf.argument}";
                                });

                                transferFunction = $"\"{transferFunction}\"";

                                // write row
                                streamWriter.WriteLine($"{groupName};{name};{unit};{transferFunction};{guid}");
                            });
                        }
                    }

                    return $"download/{Path.GetFileName(csvFileName)}";
                }
                finally
                {
                    database?.Dispose();
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
    }
}
