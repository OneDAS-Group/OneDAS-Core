using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneDas.Hdf.VdsTool.Commands
{
    public class AggregateCommand
    {
        #region Fields

        private AggregationMethod _method;
        private string _argument;
        private string _campaignName;
        private uint _days;
        private uint _aggregationChunkSizeMb;
        private Dictionary<string, string> _filters;
        private ILogger _logger;

        private OneDasDatabaseManager _databaseManager;

        #endregion

        #region Constructors

        public AggregateCommand(AggregationMethod method, string argument, string campaignName, uint days, uint aggregationChunkSizeMb, Dictionary<string, string> filters, ILogger logger)
        {
            _method = method;
            _argument = argument;
            _campaignName = campaignName;
            _days = days;
            _aggregationChunkSizeMb = aggregationChunkSizeMb;
            _logger = logger;
            _filters = filters;
        }

        #endregion

        #region Methods

        public void Run()
        {
            _databaseManager = new OneDasDatabaseManager();

            var epochEnd = DateTime.UtcNow.Date;
            var epochStart = epochEnd.AddDays(-_days);

            for (int i = 0; i <= _days; i++)
            {
                this.CreateAggregatedFiles(epochStart.AddDays(i), _campaignName);
            }
        }

        private void CreateAggregatedFiles(DateTime dateTimeBegin, string campaignName)
        {
            var subfolderName = dateTimeBegin.ToString("yyyy-MM");
            var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DATA", subfolderName);

            using var dataReader = _databaseManager.GetDataReader(campaignName);

            // get files
            if (!dataReader.IsDataOfDayAvailable(campaignName, dateTimeBegin))
                return;

            // process data
            try
            {
                var targetFileId = -1L;

                // campaign
                var campaign = _databaseManager.GetCampaigns().FirstOrDefault(campaign => campaign.Name == campaignName);

                if (campaign is null)
                    throw new Exception($"The requested campaign '{campaignName}' could not be found.");

                // targetFileId
                var campaignFileName = campaignName.TrimStart('/').Replace("/", "_");
                var dateTimeFileName = dateTimeBegin.ToString("yyyy-MM-ddTHH-mm-ssZ");
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
                        var dateTimeString = dateTimeBegin.ToString("yyyy-MM-ddTHH-mm-ssZ");
                        IOHelper.PrepareAttribute(targetFileId, "date_time", new string[] { dateTimeString }, new ulong[] { 1 }, true);
                    }

                    // campaignInfo
                    this.AggregateCampaign(dataReader, campaign, targetFileId);
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

        private void AggregateCampaign(DataReaderExtensionBase dataReader, CampaignInfo campaign, long targetFileId)
        {
            var filteredVariables = campaign.Variables.Where(variableInfo => this.ApplyAggregationFilter(variableInfo)).ToList();

            foreach (var filteredVariable in filteredVariables)
            {
                var dataset = filteredVariable.Datasets.First();

                OneDasUtilities.InvokeGenericMethod(typeof(AggregateCommand), this, nameof(this.OrchestrateAggregation),
                                                    BindingFlags.Instance | BindingFlags.NonPublic,
                                                    OneDasUtilities.GetTypeFromOneDasDataType(dataset.DataType),
                                                    new object[] { dataReader, dataset, targetFileId });
            }
        }

        private void OrchestrateAggregation<T>(DataReaderExtensionBase dataReader, Dataset dataset, long targetFileId) where T : unmanaged
        {
            // value size
            var valueSize = OneDasUtilities.SizeOf(dataset.DataType);

            // check source sample rate
            var sampleRateContainer = new SampleRateContainer(dataset.Name);

            if (!sampleRateContainer.IsPositiveNonZeroIntegerHz)
                throw new NotSupportedException($"Only positive non-zero integer frequencies are supported, but '{dataset.Name}' data were provided.");

            // prepare period data
            var groupPath = dataset.Parent.GetPath();
            var periodToDataMap = new Dictionary<Period, AggregationPeriodData>();
            var periodToBufferDataMap = new Dictionary<Period, AggregationBufferData>();
            var actualPeriods = new List<Period>();

            try
            {
                foreach (Period period in Enum.GetValues(typeof(Period)))
                {
                    var targetDatasetPath = $"{groupPath}/{(int)period}s_{_method}";

                    if (!IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
                    {
                        // buffer data
                        var bufferData = new AggregationBufferData(period);
                        var bufferSize = (ulong)bufferData.Buffer.Length;
                        var datasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, bufferSize, 1).DatasetId;

                        if (!(H5I.is_valid(datasetId) > 0))
                            throw new FormatException($"Could not open dataset '{targetDatasetPath}'.");

                        bufferData.DatasetId = datasetId;
                        periodToBufferDataMap[period] = bufferData;

                        // period data
                        var periodData = new AggregationPeriodData(period, sampleRateContainer, valueSize);
                        periodToDataMap[period] = periodData;

                        actualPeriods.Add(period);
                    }
                    else
                    {
                        // skip period
                    }
                }

                var fundamentalPeriod = TimeSpan.FromMinutes(10);
                var samplesPerFundamentalPeriod = sampleRateContainer.SamplesPerSecond * (ulong)fundamentalPeriod.TotalSeconds;
                var maxSamplesPerReadOperation = _aggregationChunkSizeMb * 1024 * 1024;

                // read data
                dataReader.ReadFullDay<T>(dataset, fundamentalPeriod, samplesPerFundamentalPeriod, maxSamplesPerReadOperation, (data, statusSet) =>
                {
                    // get aggregation data
                    var periodToPartialBufferMap = this.ApplyAggregationFunction(dataset, data, statusSet, periodToDataMap);

                    // copy aggregation data into buffer
                    foreach (var period in actualPeriods)
                    {
                        var partialBuffer = periodToPartialBufferMap[period];
                        var bufferData = periodToBufferDataMap[period];

                        Array.Copy(partialBuffer, 0, bufferData.Buffer, bufferData.BufferPosition, partialBuffer.Length);

                        bufferData.BufferPosition += partialBuffer.Length;
                    }
                });

                // write data to file
                foreach (Period period in actualPeriods)
                {
                    var bufferData = periodToBufferDataMap[period];

                    IOHelper.Write(bufferData.DatasetId, bufferData.Buffer, DataContainerType.Dataset);
                    H5F.flush(bufferData.DatasetId, H5F.scope_t.LOCAL);
                }
            }
            finally
            {
                foreach (Period period in actualPeriods)
                {
                    var datasetId = periodToBufferDataMap[period].DatasetId;
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }
            }
        }

        private Dictionary<Period, double[]> ApplyAggregationFunction<T>(Dataset dataset, T[] data, byte[] statusSet, Dictionary<Period, AggregationPeriodData> periodToDataMap)
        {
            var dataset_double = default(double[]);
            var periodToPartialBufferMap = new Dictionary<Period, double[]>();

            foreach (var item in periodToDataMap)
            {
                var period = item.Key;
                var periodData = item.Value;

                switch (_method)
                {
                    case AggregationMethod.Mean:
                    case AggregationMethod.MeanPolar:
                    case AggregationMethod.Min:
                    case AggregationMethod.Max:
                    case AggregationMethod.Std:
                    case AggregationMethod.Rms:

                        if (dataset_double == null)
                            dataset_double = ExtendedDataStorageBase.ApplyDatasetStatus(data, statusSet);

                        periodToPartialBufferMap[period] = this.ApplyAggregationFunction(_method, _argument, (int)periodData.SampleCount, dataset_double, _logger);

                        break;

                    case AggregationMethod.MinBitwise:
                    case AggregationMethod.MaxBitwise:

                        periodToPartialBufferMap[period] = this.ApplyAggregationFunction(_method, _argument, (int)periodData.SampleCount, data, statusSet, _logger);

                        break;

                    default:

                        _logger.LogWarning($"The aggregation method '{_method}' is not known. Skipping period {period}.");

                        continue;
                }
            }

            return periodToPartialBufferMap;
        }

        private double[] ApplyAggregationFunction(AggregationMethod method, string argument, int kernelSize, double[] data, ILogger logger)
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case AggregationMethod.Mean:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;
                        var chunkData = new double[kernelSize];

                        Array.Copy(data, baseIndex, chunkData, 0, kernelSize);

                        result[x] = ArrayStatistics.Mean(chunkData);
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
                        var baseIndex = x * kernelSize;

                        for (int i = 0; i < kernelSize; i++)
                        {
                            sin[x] += Math.Sin(data[baseIndex + i] * factor);
                            cos[x] += Math.Cos(data[baseIndex + i] * factor);
                        }

                        result[x] = Math.Atan2(sin[x], cos[x]) / factor;

                        if (result[x] < 0)
                            result[x] += limit;
                    });

                    break;

                case AggregationMethod.Min:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;
                        var chunkData = new double[kernelSize];

                        Array.Copy(data, baseIndex, chunkData, 0, kernelSize);

                        result[x] = ArrayStatistics.Minimum(chunkData);
                    });

                    break;

                case AggregationMethod.Max:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;
                        var chunkData = new double[kernelSize];

                        Array.Copy(data, baseIndex, chunkData, 0, kernelSize);

                        result[x] = ArrayStatistics.Maximum(chunkData);
                    });

                    break;

                case AggregationMethod.Std:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;
                        var chunkData = new double[kernelSize];

                        Array.Copy(data, baseIndex, chunkData, 0, kernelSize);

                        result[x] = ArrayStatistics.StandardDeviation(chunkData);
                    });

                    break;

                case AggregationMethod.Rms:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;
                        var chunkData = new double[kernelSize];

                        Array.Copy(data, baseIndex, chunkData, 0, kernelSize);

                        result[x] = ArrayStatistics.RootMeanSquare(chunkData);
                    });

                    break;

                default:

                    logger.LogWarning($"The aggregation method '{method}' is not known. Skipping period.");

                    break;

            }

            return result;
        }

        private double[] ApplyAggregationFunction<T>(AggregationMethod method, string argument, int kernelSize, T[] data, byte[] statusSet, ILogger logger)
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case AggregationMethod.MinBitwise:

                    T[] bitField_and = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;

                        for (int i = 0; i < kernelSize; i++)
                        {
                            if (statusSet[baseIndex + i] != 1)
                            {
                                result[x] = Double.NaN;
                                return;
                            }
                            else
                            {
                                if (i == 0)
                                    bitField_and[x] = GenericBitOr<T>.BitOr(bitField_and[x], data[baseIndex + i]);
                                else
                                    bitField_and[x] = GenericBitAnd<T>.BitAnd(bitField_and[x], data[baseIndex + i]);
                            }
                        }

                        // all OK
                        result[x] = Convert.ToDouble(bitField_and[x]);
                    });

                    break;

                case AggregationMethod.MaxBitwise:

                    T[] bitField_or = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * kernelSize;

                        for (int i = 0; i < kernelSize; i++)
                        {
                            if (statusSet[baseIndex + i] != 1)
                            {
                                result[x] = Double.NaN;
                                return;
                            }
                            else
                            {
                                bitField_or[x] = GenericBitOr<T>.BitOr(bitField_or[x], data[baseIndex + i]);
                            }
                        }

                        // all OK
                        result[x] = Convert.ToDouble(bitField_or[x]);
                    });

                    break;

                default:
                    logger.LogWarning($"The aggregation method '{method}' is not known. Skipping period.");
                    break;

            }

            return result;
        }

        private bool ApplyAggregationFilter(Variable variableInfo)
        {
            bool result = true;

            // channel
            if (_filters.ContainsKey("--include-channel"))
            {
                if (variableInfo.VariableNames.Any())
                    result &= Regex.IsMatch(variableInfo.VariableNames.Last(), _filters["--include-channel"]);
                else
                    result &= false;
            }

            if (_filters.ContainsKey("--exclude-channel"))
            {
                if (variableInfo.VariableNames.Any())
                    result &= !Regex.IsMatch(variableInfo.VariableNames.Last(), _filters["--exclude-channel"]);
                else
                    result &= true;
            }

            // group
            if (_filters.ContainsKey("--include-group"))
            {
                if (variableInfo.VariableGroups.Any())
                    result &= variableInfo.VariableGroups.Last().Split('\n').Any(groupName => Regex.IsMatch(groupName, _filters["--include-group"]));
                else
                    result &= false;
            }

            if (_filters.ContainsKey("--exclude-group"))
            {
                if (variableInfo.VariableGroups.Any())
                    result &= !variableInfo.VariableGroups.Last().Split('\n').Any(groupName => Regex.IsMatch(groupName, _filters["--exclude-group"]));
                else
                    result &= true;
            }

            // unit
            if (_filters.ContainsKey("--include-unit"))
            {
#warning Remove this special case check.
                if (variableInfo.Units.Last() == null)
                {
                    _logger.LogWarning("Unit 'null' value detected.");
                    result &= false;
                }
                else
                {
                    if (variableInfo.Units.Any())
                        result &= Regex.IsMatch(variableInfo.Units.Last(), _filters["--include-unit"]);
                    else
                        result &= false;
                }
            }

            if (_filters.ContainsKey("--exclude-unit"))
            {
#warning Remove this special case check.
                if (variableInfo.Units.Last() == null)
                {
                    _logger.LogWarning("Unit 'null' value detected.");
                    result &= true;

                }
                else
                {
                    if (variableInfo.Units.Any())
                        result &= !Regex.IsMatch(variableInfo.Units.Last(), _filters["--exclude-unit"]);
                    else
                        result &= true;
                }
            }

            return result;
        }

        #endregion
    }
}
