using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.Buffers;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.VdsTool.Commands
{
    public class AggregateCommand
    {
        #region Fields

        private uint _days;
        private uint _aggregationChunkSizeMb;
        private bool _force;
        private ILogger _logger;

        private OneDasDatabaseManager _databaseManager;

        #endregion

        #region Constructors

        public AggregateCommand(uint days, uint aggregationChunkSizeMb, bool force, ILogger logger)
        {
            _days = days;
            _aggregationChunkSizeMb = aggregationChunkSizeMb;
            _force = force;
            _logger = logger;
        }

        #endregion

        #region Methods

        public void Run()
        {
            _databaseManager = new OneDasDatabaseManager();
            _databaseManager.Update();

            var campaignNames = _databaseManager.Config.AggregationConfigs.Select(config => config.CampaignName).Distinct().ToList();
            var epochEnd = DateTime.UtcNow.Date;
            var epochStart = epochEnd.AddDays(-_days);

            foreach (var campaignName in campaignNames)
            {
                for (int i = 0; i <= _days; i++)
                {
                    this.CreateAggregatedFiles(campaignName, epochStart.AddDays(i));
                }
            }
        }

        private void CreateAggregatedFiles(string campaignName, DateTime date)
        {
            var subfolderName = date.ToString("yyyy-MM");
            var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DATA", subfolderName);

            using var dataReader = _databaseManager.GetNativeDataReader(campaignName);

            // get files
            if (!dataReader.IsDataOfDayAvailable(campaignName, date))
                return;

            // process data
            try
            {
                var targetFileId = -1L;

                // campaign
                var campaign = _databaseManager.Database.GetCampaigns().FirstOrDefault(campaign => campaign.Id == campaignName);

                if (campaign is null)
                    throw new Exception($"The requested campaign '{campaignName}' could not be found.");

                // targetFileId
                var campaignFileName = campaignName.TrimStart('/').Replace("/", "_");
                var dateTimeFileName = date.ToString("yyyy-MM-ddTHH-mm-ssZ");
                var targetFileName = $"{campaignFileName}_{dateTimeFileName}.h5";
                var targetFilePath = Path.Combine(targetDirectoryPath, targetFileName);

                if (!Directory.Exists(targetDirectoryPath))
                    Directory.CreateDirectory(targetDirectoryPath);

                if (File.Exists(targetFilePath))
                    targetFileId = H5F.open(targetFilePath, H5F.ACC_RDWR);

                if (targetFileId == -1)
                    targetFileId = H5F.create(targetFilePath, H5F.ACC_TRUNC);

                try
                {
                    // create attribute if necessary
                    if (H5A.exists(targetFileId, "date_time") == 0)
                    {
                        var dateTimeString = date.ToString("yyyy-MM-ddTHH-mm-ssZ");
                        IOHelper.PrepareAttribute(targetFileId, "date_time", new string[] { dateTimeString }, new ulong[] { 1 }, true);
                    }

                    // campaignInfo
                    this.AggregateCampaign(dataReader, campaign, date, targetFileId);
                }
                finally
                {
                    if (H5I.is_valid(targetFileId) > 0) { H5F.close(targetFileId); }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void AggregateCampaign(DataReaderExtensionBase dataReader, CampaignInfo campaign, DateTime date, long targetFileId)
        {
            _logger.LogInformation($"Processing day {date.ToString("yyyy-MM-dd")} ... ");

            var potentialAggregationConfigs = _databaseManager.Config.AggregationConfigs.Where(config => config.CampaignName == campaign.Id).ToList();

            var variableToAggregationConfigMap = campaign.Variables
                .ToDictionary(variable => variable, variable => potentialAggregationConfigs.Where(config => this.ApplyAggregationFilter(variable, config.Filters)).ToList())
                .Where(entry => entry.Value.Any())
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var entry in variableToAggregationConfigMap)
            {
                var variable = entry.Key;
                var dataset = variable.Datasets.First();
                var aggregationConfigs = entry.Value;

                OneDasUtilities.InvokeGenericMethod(this, nameof(this.OrchestrateAggregation),
                                                    BindingFlags.Instance | BindingFlags.NonPublic,
                                                    OneDasUtilities.GetTypeFromOneDasDataType(dataset.DataType),
                                                    new object[] { dataReader, dataset, aggregationConfigs, date, targetFileId });
            }

            _logger.LogInformation($"Processing day {date.ToString("yyyy-MM-dd")} ... Done.");
        }

        private void OrchestrateAggregation<T>(DataReaderExtensionBase dataReader, DatasetInfo dataset, List<AggregationConfig> aggregationConfigs, DateTime date, long targetFileId) where T : unmanaged
        {
            _logger.LogInformation($"Processing dataset '{dataset.GetPath()}' ... ");

            // value size
            var valueSize = OneDasUtilities.SizeOf(dataset.DataType);

            // check source sample rate
            var sampleRate = new SampleRateContainer(dataset.Id, ensureNonZeroIntegerHz: true);

            // prepare period data
            var groupPath = dataset.Parent.GetPath();
            var setupToDataMap = new Dictionary<AggregationSetup, AggregationPeriodData>();
            var setupToBufferDataMap = new Dictionary<AggregationSetup, AggregationBufferData>();
            var actualPeriods = new HashSet<AggregationPeriod>();

            var setups = aggregationConfigs.SelectMany(config =>
            {
                return Enum.GetValues(typeof(AggregationPeriod))
                    .Cast<AggregationPeriod>()
                    .Select(period => new AggregationSetup(config, period));
            }).ToList();

            try
            {
                // prepare buffers
                foreach (var setup in setups)
                {
                    // translate method name
                    var methodIdentifier = setup.Config.Method switch
                    {
                        AggregationMethod.Mean => "mean",
                        AggregationMethod.MeanPolar => "mean_polar",
                        AggregationMethod.Min => "min",
                        AggregationMethod.Max => "max",
                        AggregationMethod.Std => "std",
                        AggregationMethod.Rms => "rms",
                        AggregationMethod.MinBitwise => "min_bitwise",
                        AggregationMethod.MaxBitwise => "max_bitwise",
                        AggregationMethod.SampleAndHold => "sample_and_hold",
                        _ => throw new Exception($"The aggregation method '{setup.Config.Method}' is unknown.")
                    };

                    var targetDatasetPath = $"{groupPath}/{(int)setup.Period} s_{methodIdentifier}";

                    if (_force || !IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
                    {
                        // buffer data
                        var bufferData = new AggregationBufferData(setup.Period);
                        var bufferSize = (ulong)bufferData.Buffer.Length;
                        var datasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, bufferSize, 1).DatasetId;

                        if (!(H5I.is_valid(datasetId) > 0))
                            throw new FormatException($"Could not open dataset '{targetDatasetPath}'.");

                        bufferData.DatasetId = datasetId;
                        setupToBufferDataMap[setup] = bufferData;

                        // period data
                        var periodData = new AggregationPeriodData(setup.Period, sampleRate, valueSize);
                        setupToDataMap[setup] = periodData;

                        actualPeriods.Add(setup.Period);
                    }
                    else
                    {
                        // skip period
                    }
                }

                if (!setupToDataMap.Any())
                {
                    _logger.LogInformation($"Processing dataset '{dataset.GetPath()}' ... Skipped.");
                    return;
                }

                // process data
                var fundamentalPeriod = TimeSpan.FromMinutes(10); // required to ensure that the aggregation functions gets data with a multiple length of 10 minutes
                var endDate = date.AddDays(1);
                var blockSizeLimit = _aggregationChunkSizeMb * 1000 * 1000;

                // read data
                dataReader.Read(dataset, date, endDate, blockSizeLimit, fundamentalPeriod, progressRecord =>
                {
                    var dataRecord = progressRecord.DatasetToRecordMap.First().Value;

                    // aggregate data
                    var setupToPartialBufferMap = this.ApplyAggregationFunction((T[])dataRecord.Dataset, dataRecord.Status, setupToDataMap);

                    foreach (var entry in setupToDataMap)
                    {
                        var setup = entry.Key;

                        // copy aggregated data into buffer
                        var partialBuffer = setupToPartialBufferMap[setup];
                        var bufferData = setupToBufferDataMap[setup];

                        Array.Copy(partialBuffer, 0, bufferData.Buffer, bufferData.BufferPosition, partialBuffer.Length);
                        bufferData.BufferPosition += partialBuffer.Length;
                    }
                }, CancellationToken.None);

                // write data to file
                foreach (var entry in setupToDataMap)
                {
                    var setup = entry.Key;
                    var bufferData = setupToBufferDataMap[setup];

                    IOHelper.Write(bufferData.DatasetId, bufferData.Buffer, DataContainerType.Dataset);
                    H5F.flush(bufferData.DatasetId, H5F.scope_t.LOCAL);
                }

                _logger.LogInformation($"Processing dataset '{dataset.GetPath()}' ... Done.");
            }
            finally
            {
                foreach (var setup in setups)
                {
                    if (setupToBufferDataMap.ContainsKey(setup))
                    {
                        var datasetId = setupToBufferDataMap[setup].DatasetId;

                        if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                    }
                }
            }
        }

        private Dictionary<AggregationSetup, double[]> ApplyAggregationFunction<T>(T[] data,
                                                                                   byte[] statusSet,
                                                                                   Dictionary<AggregationSetup, AggregationPeriodData> setupToDataMap) where T : unmanaged
        {
            var nanLimit = 0.99;
            var dataset_double = default(double[]);
            var setupToPartialBufferMap = new Dictionary<AggregationSetup, double[]>();

            foreach (var item in setupToDataMap)
            {
                var setup = item.Key;
                var aggregationConfig = setup.Config;
                var period = setup.Period;
                var periodData = item.Value;

                switch (aggregationConfig.Method)
                {
                    case AggregationMethod.Mean:
                    case AggregationMethod.MeanPolar:
                    case AggregationMethod.Min:
                    case AggregationMethod.Max:
                    case AggregationMethod.Std:
                    case AggregationMethod.Rms:

                        if (dataset_double == null)
                            dataset_double = BufferUtilities.ApplyDatasetStatus<T>(data, statusSet);

                        setupToPartialBufferMap[setup] = this.ApplyAggregationFunction(aggregationConfig, (int)periodData.SampleCount, dataset_double, nanLimit, _logger);

                        break;

                    case AggregationMethod.MinBitwise:
                    case AggregationMethod.MaxBitwise:

                        setupToPartialBufferMap[setup] = this.ApplyAggregationFunction(aggregationConfig, (int)periodData.SampleCount, data, statusSet, nanLimit, _logger);

                        break;

                    default:

                        _logger.LogWarning($"The aggregation method '{aggregationConfig.Method}' is not known. Skipping period {period}.");

                        continue;
                }
            }

            return setupToPartialBufferMap;
        }

        internal double[] ApplyAggregationFunction(AggregationConfig aggregationConfig, int kernelSize, double[] data, double nanLimit, ILogger logger)
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];
            var method = aggregationConfig.Method;
            var argument = aggregationConfig.Argument;

            switch (method)
            {
                case AggregationMethod.Mean:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = ArrayStatistics.Mean(chunkData);
                        else
                            result[x] = double.NaN;
                    });

                    break;

                case AggregationMethod.MeanPolar:

                    double[] sin = new double[targetDatasetLength];
                    double[] cos = new double[targetDatasetLength];
                    double limit;

                    if (argument.Contains("*PI"))
                        limit = Double.Parse(argument.Replace("*PI", "")) * Math.PI;
                    else
                        limit = Double.Parse(argument);

                    var factor = 2 * Math.PI / limit;

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var length = chunkData.Length;
                        var isHighQuality = (length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                        {
                            for (int i = 0; i < chunkData.Length; i++)
                            {
                                sin[x] += Math.Sin(chunkData[i] * factor);
                                cos[x] += Math.Cos(chunkData[i] * factor);
                            }

                            result[x] = Math.Atan2(sin[x], cos[x]) / factor;

                            if (result[x] < 0)
                                result[x] += limit;
                        }
                        else
                        {
                            result[x] = double.NaN;
                        }
                    });

                    break;

                case AggregationMethod.Min:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = ArrayStatistics.Minimum(chunkData);
                        else
                            result[x] = double.NaN;
                    });

                    break;

                case AggregationMethod.Max:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = ArrayStatistics.Maximum(chunkData);
                        else
                            result[x] = double.NaN;
                    });

                    break;

                case AggregationMethod.Std:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = ArrayStatistics.StandardDeviation(chunkData);
                        else
                            result[x] = double.NaN;
                    });

                    break;

                case AggregationMethod.Rms:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = ArrayStatistics.RootMeanSquare(chunkData);
                        else
                            result[x] = double.NaN;
                    });

                    break;

                case AggregationMethod.SampleAndHold:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = chunkData.First();
                        else
                            result[x] = double.NaN;
                    });

                    break;

                default:

                    logger.LogWarning($"The aggregation method '{method}' is not known. Skipping period.");

                    break;

            }

            return result;
        }

        internal double[] ApplyAggregationFunction<T>(AggregationConfig aggregationConfig, int kernelSize, T[] data, byte[] statusSet, double nanLimit, ILogger logger) where T : unmanaged
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];
            var method = aggregationConfig.Method;
            var argument = aggregationConfig.Argument;

            switch (method)
            {
                case AggregationMethod.MinBitwise:

                    T[] bitField_and = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, statusSet, x, kernelSize);
                        var length = chunkData.Length;
                        var isHighQuality = (length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                if (i == 0)
                                    bitField_and[x] = GenericBitOr<T>.BitOr(bitField_and[x], chunkData[i]);
                                else
                                    bitField_and[x] = GenericBitAnd<T>.BitAnd(bitField_and[x], chunkData[i]);
                            }

                            result[x] = Convert.ToDouble(bitField_and[x]);
                        }
                        else
                        {
                            result[x] = double.NaN;
                        }
                    });

                    break;

                case AggregationMethod.MaxBitwise:

                    T[] bitField_or = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, statusSet, x, kernelSize);
                        var length = chunkData.Length;
                        var isHighQuality = (length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                bitField_or[x] = GenericBitOr<T>.BitOr(bitField_or[x], chunkData[i]);
                            }

                            result[x] = Convert.ToDouble(bitField_or[x]);
                        }
                        else
                        {
                            result[x] = double.NaN;
                        }
                    });

                    break;

                default:
                    logger.LogWarning($"The aggregation method '{method}' is not known. Skipping period.");
                    break;

            }

            return result;
        }

        private unsafe T[] GetNaNFreeData<T>(T[] data, byte[] statusSet, int index, int kernelSize) where T : unmanaged
        {
            var sourceIndex = index * kernelSize;
            var length = data.Length;
            var chunkData = new List<T>();

            for (int i = 0; i < length; i++)
            {
                if (statusSet[sourceIndex + i] == 1)
                    chunkData.Add(data[sourceIndex + i]);
            }

            return chunkData.ToArray();
        }

        private double[] GetNaNFreeData(IEnumerable<double> data, int index, int kernelSize)
        {
            var sourceIndex = index * kernelSize;

            return data
                .Skip(sourceIndex)
                .Take(kernelSize)
                .Where(value => !double.IsNaN(value))
                .ToArray();
        }

        private bool ApplyAggregationFilter(VariableInfo variable, Dictionary<string, string> filters)
        {
            bool result = true;

            // channel
            if (filters.ContainsKey("--include-channel"))
                result &= Regex.IsMatch(variable.Name, filters["--include-channel"]);

            if (filters.ContainsKey("--exclude-channel"))
                result &= !Regex.IsMatch(variable.Name, filters["--exclude-channel"]);

            // group
            if (filters.ContainsKey("--include-group"))
                result &= variable.Group.Split('\n').Any(groupName => Regex.IsMatch(groupName, filters["--include-group"]));

            if (filters.ContainsKey("--exclude-group"))
                result &= !variable.Group.Split('\n').Any(groupName => Regex.IsMatch(groupName, filters["--exclude-group"]));

            // unit
            if (filters.ContainsKey("--include-unit"))
            {
#warning Remove this special case check.
                if (variable.Unit == null)
                {
                    _logger.LogWarning("Unit 'null' value detected.");
                    result &= false;
                }
                else
                {
                    result &= Regex.IsMatch(variable.Unit, filters["--include-unit"]);
                }
            }

            if (filters.ContainsKey("--exclude-unit"))
            {
#warning Remove this special case check.
                if (variable.Unit == null)
                {
                    _logger.LogWarning("Unit 'null' value detected.");
                    result &= true;

                }
                else
                {
                    result &= !Regex.IsMatch(variable.Unit, filters["--exclude-unit"]);
                }
            }

            return result;
        }

        #endregion
    }
}
