using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using OneDas.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.DataStorage;
using OneDas.Hdf.Explorer.Core;
using OneDas.Hdf.Explorer.DataLake;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer.Commands
{
    public class AggregateCommand
    {
        #region Fields

        private AggregationMethod _method;
        private string _argument;
        private string _campaignName;
        private uint _days;
        private Dictionary<string, string> _filters;
        private ILogger _logger;

        #endregion

        #region Constructors

        public AggregateCommand(AggregationMethod method, string argument, string campaignName, uint days, Dictionary<string, string> filters, ILogger logger)
        {
            _method = method;
            _argument = argument;
            _campaignName = campaignName;
            _days = days;
            _logger = logger;
            _filters = filters;
        }

        #endregion

        #region Methods

        public void Run()
        {
            var epochEnd = DateTime.UtcNow.Date;
            var epochStart = epochEnd.AddDays(-_days);

            var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_NATIVE", epochStart.ToString("yyyy-MM"));
            var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_AGGREGATION", epochStart.ToString("yyyy-MM"));

            for (int i = 0; i <= _days; i++)
            {
                this.CreateAggregatedFiles(epochStart.AddDays(i), _campaignName);
            }
        }

        private void CreateAggregatedFiles(DateTime dateTimeBegin, string campaignName)
        {
            long targetFileId = -1;

            var folderName = dateTimeBegin.ToString("yyyy-MM");
            using var dataProvider = new HdfDataLake(path).GetDataProvider(campaignName, dateTimeBegin);
            var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_AGGREGATION", folderName);

            // get files
            if (!Directory.Exists(sourceDirectoryPath))
                return;

            var campaignFileName = campaignName.TrimStart('/').Replace("/", "_");
            var dateTimeFileName = dateTimeBegin.ToString("yyyy-MM-ddTHH-mm-ssZ");
            var fileName = $"{campaignFileName}_V*_{dateTimeFileName}.h5";
            var filePathSet = Directory.GetFiles(sourceDirectoryPath, fileName, SearchOption.TopDirectoryOnly);

            // process files
            try
            {
                foreach (string sourceFilePath in filePathSet) #error
                {
                    targetFileId = -1;

                    _logger.LogInformation($"file: {sourceFilePath}");
                    Console.WriteLine();

                    // campaignInfo
                    var campaignInfo = dataProvider.GetCampaignInfo();

                    if (campaignInfo == null)
                        throw new Exception($"The campaign '{campaignName}' was not found in file '{sourceFilePath}'.");

                    // targetFileId
                    var targetFilePath = Path.Combine(targetDirectoryPath, Path.GetFileName(sourceFilePath));

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
                            var dateTime = IOHelper.ReadAttribute<string>(sourceFileId, "date_time");
                            IOHelper.PrepareAttribute(targetFileId, "date_time", dateTimeBegin, new ulong[] { 1 }, true);
                        }

                        // campaignInfo
                        this.AggregateCampaign(dataProvider, targetFileId, campaignInfo);

                        Console.CursorTop -= 1;
                    }
                    finally
                    {
                        if (H5I.is_valid(targetFileId) > 0) { H5F.close(targetFileId); }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void AggregateCampaign(IDataProvider dataProvider, long targetFileId, CampaignInfo campaignInfo)
        {
            var index = 0;
            var datasetPath = $"{campaignInfo.GetPath()}/is_chunk_completed_set";
            (var groupId, var isNew) = IOHelper.OpenOrCreateGroup(targetFileId, campaignInfo.GetPath());

            try
            {
                if (isNew || !IOHelper.CheckLinkExists(targetFileId, datasetPath))
                    H5O.copy(sourceFileId, datasetPath, targetFileId, datasetPath);

                var filteredVariableInfos = campaignInfo.VariableInfos.Where(variableInfo => this.ApplyAggregationFilter(variableInfo)).ToList();

                foreach (var filteredVariableInfo in filteredVariableInfos)
                {
                    index++;

                    Console.WriteLine($"{filteredVariableInfo.VariableNames.Last()} ({index}/{filteredVariableInfos.Count()})");
                    this.AggregateVariable(dataProvider, targetFileId, filteredVariableInfo);
                    Console.CursorTop -= 1;
                    this.ClearCurrentLine();
                }
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
            }
        }

        private void AggregateVariable(IDataProvider dataProvider, long targetFileId, VariableInfo variableInfo)
        {
            long sourceDatasetId = -1;
            long sourceDatasetId_status = -1;

            var sampleRate = (SampleRate)0;
            var groupPath = variableInfo.GetPath();

            try
            {
                // find proper data source
                foreach (SampleRate testedSampleRate in Enum.GetValues(typeof(SampleRate)))
                {
                    var sourceDatasetName = new SampleRateContainer(testedSampleRate).ToUnitString();

                    if (variableInfo.DatasetInfos.Where(x => x.Name == sourceDatasetName).Any())
                    {
                        sampleRate = testedSampleRate;
                        sourceDatasetId = H5D.open(sourceFileId, $"{groupPath}/{sourceDatasetName}");
                        sourceDatasetId_status = H5D.open(sourceFileId, $"{groupPath}/{sourceDatasetName}_status");

                        break;
                    }
                }

                if (sampleRate == default)
                {
                    _logger.LogInformation($"No appropriate data source was found. Skipping variable '{variableInfo.Name}'.");
                    return;
                }

                // invoke this.AggregateDataset
                GeneralHelper.InvokeGenericMethod(typeof(AggregateCommand), this, nameof(this.AggregateDataset),
                                              BindingFlags.Instance | BindingFlags.NonPublic,
                                              TypeConversionHelper.GetTypeFromHdfTypeId(typeId),
                                              new object[] { groupPath, sourceDatasetId, sourceDatasetId_status, targetFileId, sampleRate });

                // clean up
                H5D.close(sourceDatasetId_status);
                H5D.close(sourceDatasetId);
            }
            finally
            {
                //
            }
        }

        private void AggregateDataset<T>(string groupPath, long sourceDatasetId, long sourceDatasetId_status, long targetFileId, SampleRate sampleRate)
        {
            long targetDatasetId = -1;
            double[] targetValueSet;
            double[] sourceValueSet_double = null;
            byte[] sourceValueSet_status = null;
            T[] sourceValueSet = null;

            Console.WriteLine($"\t{_method}");

            // for each period
            foreach (Period period in Enum.GetValues(typeof(Period)))
            {
                var targetDatasetPath = $"{groupPath}/{(int)period} s_{_method}";

                // if target dataset does not yet exist
                if (!IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
                {
                    Console.WriteLine($"\t\t{ (int)period } s");

                    if (sourceValueSet == null)
                    {
                        sourceValueSet = IOHelper.Read<T>(sourceDatasetId, DataContainerType.Dataset);
                        sourceValueSet_status = IOHelper.Read<byte>(sourceDatasetId_status, DataContainerType.Dataset);
                    }

                    var chunkCount = sourceValueSet.Count() / (int)period / 100 * (int)sampleRate; // Improve: remove magic number

                    switch (_method)
                    {
                        case AggregationMethod.Mean:
                        case AggregationMethod.MeanPolar:
                        case AggregationMethod.Min:
                        case AggregationMethod.Max:
                        case AggregationMethod.Std:
                        case AggregationMethod.Rms:

                            if (sourceValueSet_double == null)
                                sourceValueSet_double = ExtendedDataStorageBase.ApplyDatasetStatus(sourceValueSet, sourceValueSet_status);

                            targetValueSet = this.ApplyAggregationFunction(_method, _argument, chunkCount, sourceValueSet_double, _logger);

                            break;

                        case AggregationMethod.MinBitwise:
                        case AggregationMethod.MaxBitwise:

                            targetValueSet = this.ApplyAggregationFunction(_method, _argument, chunkCount, sourceValueSet, sourceValueSet_status, _logger);

                            break;

                        default:

                            _logger.LogWarning($"The aggregation method '{_method}' is not known. Skipping period {period}.");

                            continue;
                    }

                    targetDatasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, (ulong)chunkCount, 1).DatasetId;

                    try
                    {
                        IOHelper.Write(targetDatasetId, targetValueSet.ToArray(), DataContainerType.Dataset);

                        Console.CursorTop -= 1;
                        this.ClearCurrentLine();

                        H5F.flush(targetDatasetId, H5F.scope_t.LOCAL);
                    }
                    finally
                    {
                        if (H5I.is_valid(targetDatasetId) > 0) { H5D.close(targetDatasetId); }
                    }
                }
            }

            Console.CursorTop -= 1;
            this.ClearCurrentLine();
        }

        private double[] ApplyAggregationFunction(AggregationMethod method, string argument, int targetDatasetLength, double[] valueSet, ILogger logger)
        {
            var chunkSize = valueSet.Count() / targetDatasetLength;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case AggregationMethod.Mean:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Mean(chunkValueSet);
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
                        var baseIndex = x * chunkSize;

                        for (int i = 0; i < chunkSize; i++)
                        {
                            sin[x] += Math.Sin(valueSet[baseIndex + i] * factor);
                            cos[x] += Math.Cos(valueSet[baseIndex + i] * factor);
                        }

                        result[x] = Math.Atan2(sin[x], cos[x]) / factor;

                        if (result[x] < 0)
                            result[x] += limit;
                    });

                    break;

                case AggregationMethod.Min:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Minimum(chunkValueSet);
                    });

                    break;

                case AggregationMethod.Max:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Maximum(chunkValueSet);
                    });

                    break;

                case AggregationMethod.Std:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.StandardDeviation(chunkValueSet);
                    });

                    break;

                case AggregationMethod.Rms:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.RootMeanSquare(chunkValueSet);
                    });

                    break;

                default:

                    logger.LogWarning($"The aggregation method '{method}' is not known. Skipping period.");

                    break;

            }

            return result;
        }

        private double[] ApplyAggregationFunction<T>(AggregationMethod method, string argument, int targetDatasetLength, T[] valueSet, byte[] valueSet_status, ILogger logger)
        {
            var chunkSize = valueSet.Count() / targetDatasetLength;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case AggregationMethod.MinBitwise:

                    T[] bitField_and = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;

                        for (int i = 0; i < chunkSize; i++)
                        {
                            if (valueSet_status[baseIndex + i] != 1)
                            {
                                result[x] = Double.NaN;
                                return;
                            }
                            else
                            {
                                if (i == 0)
                                    bitField_and[x] = GenericBitOr<T>.BitOr(bitField_and[x], valueSet[baseIndex + i]);
                                else
                                    bitField_and[x] = GenericBitAnd<T>.BitAnd(bitField_and[x], valueSet[baseIndex + i]);
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
                        var baseIndex = x * chunkSize;

                        for (int i = 0; i < chunkSize; i++)
                        {
                            if (valueSet_status[baseIndex + i] != 1)
                            {
                                result[x] = Double.NaN;
                                return;
                            }
                            else
                            {
                                bitField_or[x] = GenericBitOr<T>.BitOr(bitField_or[x], valueSet[baseIndex + i]);
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

        private bool ApplyAggregationFilter(VariableInfo variableInfo)
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

        private void ClearCurrentLine()
        {
            Console.Write($"\r{ new string(' ', Console.WindowWidth - 1) }\r");
        }

        #endregion
    }
}
