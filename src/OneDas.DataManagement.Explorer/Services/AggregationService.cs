using HDF.PInvoke;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneDas.Buffers;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.Infrastructure;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Services
{
    public class AggregationService
    {
        #region Fields

        private uint _aggregationChunkSizeMb;

        private ILogger _logger;

        private SignInManager<IdentityUser> _signInManager;
        private OneDasDatabaseManager _databaseManager;

        #endregion

        #region Constructors

        public AggregationService(
            SignInManager<IdentityUser> signInManager,
            OneDasDatabaseManager databaseManager, 
            OneDasExplorerOptions options, 
            ILoggerFactory loggerFactory)
        {
            _signInManager = signInManager;
            _databaseManager = databaseManager;
            _aggregationChunkSizeMb = options.AggregationChunkSizeMB;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");

            this.Progress = new Progress<ProgressUpdatedEventArgs>();
        }

        #endregion

        #region Properties

        public Progress<ProgressUpdatedEventArgs> Progress { get; }

        #endregion

        #region Methods

        public Task<string> AggregateDataAsync(IPAddress remoteIpAddress, string databaseFolderPath, AggregationParameters aggregationParameters, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                // log
                string userName;

                if (_signInManager.Context.User.Identity.IsAuthenticated)
                    userName = _signInManager.Context.User.Identity.Name;
                else
                    userName = "anonymous";

                var message = $"User '{userName}' ({remoteIpAddress}) aggregates data: {aggregationParameters.Days} days ... ";
                _logger.LogInformation(message);

                try
                {
                    _databaseManager.Update();

                    var projectIds = aggregationParameters.Aggregations
                        .Select(config => config.ProjectId)
                        .Distinct().ToList();
                    var epochEnd = DateTime.UtcNow.Date;
                    var epochStart = epochEnd.AddDays(-aggregationParameters.Days);
                    var totalDays = projectIds.Count * aggregationParameters.Days;

                    for (int i = 0; i < projectIds.Count; i++)
                    {
                        var projectId = projectIds[i];
                        var currentDay = epochStart.AddDays(i);

                        if (cancellationToken.IsCancellationRequested)
                            break;

                        for (int j = 0; j <= aggregationParameters.Days; j++)
                        {
                            var progress = (IProgress<ProgressUpdatedEventArgs>)this.Progress;
                            var progressMessage = $"Processing project '{projectId}': {currentDay.ToString("yyy-MM-dd")}";
                            var eventArgs = new ProgressUpdatedEventArgs(i * j / totalDays, progressMessage);
                            progress.Report(eventArgs);

                            if (cancellationToken.IsCancellationRequested)
                                break;

                            this.CreateAggregatedFiles(databaseFolderPath, projectId, currentDay, aggregationParameters, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.GetFullMessage());
                    throw;
                }

                _logger.LogInformation($"{message} Done.");

                return string.Empty;
            }, cancellationToken);
        }

        private void CreateAggregatedFiles(string databaseFolderPath, string projectId, DateTime date, AggregationParameters aggregationParameters, CancellationToken cancellationToken)
        {
            var subfolderName = date.ToString("yyyy-MM");
            var targetDirectoryPath = Path.Combine(databaseFolderPath, "DATA", subfolderName);

            using var dataReader = _databaseManager.GetNativeDataReader(projectId);

            // get files
            if (!dataReader.IsDataOfDayAvailable(projectId, date))
                return;

            // process data
            try
            {
                var targetFileId = -1L;

                // project
                var project = _databaseManager.Database
                    .GetProjects()
                    .FirstOrDefault(project => project.Id == projectId);

                if (project is null)
                    throw new Exception($"The requested project '{projectId}' could not be found.");

                // targetFileId
                var projectFileName = projectId.TrimStart('/').Replace("/", "_");
                var dateTimeFileName = date.ToString("yyyy-MM-ddTHH-mm-ssZ");
                var targetFileName = $"{projectFileName}_{dateTimeFileName}.h5";
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

                    // projectInfo
                    this.AggregateProject(dataReader, project, date, targetFileId, aggregationParameters, cancellationToken);
                }
                finally
                {
                    if (H5I.is_valid(targetFileId) > 0) { H5F.close(targetFileId); }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetFullMessage());
            }
        }

        private void AggregateProject(DataReaderExtensionBase dataReader, ProjectInfo project, DateTime date, long targetFileId, AggregationParameters aggregationParameters, CancellationToken cancellationToken)
        {
            var potentialAggregationParamerters = aggregationParameters.Aggregations
                .Where(paramerters => paramerters.ProjectId == project.Id)
                .ToList();

            var channelToAggregationConfigMap = project.Channels
                .ToDictionary(channel => channel, channel => potentialAggregationParamerters.Where(paramerters => this.ApplyAggregationFilter(channel, paramerters.Filters)).ToList())
                .Where(entry => entry.Value.Any())
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var entry in channelToAggregationConfigMap)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var channel = entry.Key;
                var dataset = channel.Datasets.First();
                var aggregationParameterss = entry.Value;

                OneDasUtilities.InvokeGenericMethod(this, nameof(this.OrchestrateAggregation),
                                                    BindingFlags.Instance | BindingFlags.NonPublic,
                                                    OneDasUtilities.GetTypeFromOneDasDataType(dataset.DataType),
                                                    new object[] { dataReader, dataset, aggregationParameterss, date, targetFileId, aggregationParameters.Force });
            }
        }

        private void OrchestrateAggregation<T>(DataReaderExtensionBase dataReader,
                                               DatasetInfo dataset,
                                               AggregationParameters aggregationParameters,
                                               DateTime date,
                                               long targetFileId,
                                               bool force) where T : unmanaged
        {
            // value size
            var valueSize = OneDasUtilities.SizeOf(dataset.DataType);

            // check source sample rate
            var sampleRate = new SampleRateContainer(dataset.Id, ensureNonZeroIntegerHz: true);

            // prepare period data
            var groupPath = dataset.Parent.GetPath();
            var setupToDataMap = new Dictionary<AggregationSetup, AggregationPeriodData>();
            var setupToBufferDataMap = new Dictionary<AggregationSetup, AggregationBufferData>();
            var actualPeriods = new HashSet<AggregationPeriod>();

            var setups = aggregationParameters.Aggregations.SelectMany(parameters =>
            {
                return Enum.GetValues(typeof(AggregationPeriod))
                    .Cast<AggregationPeriod>()
                    .Select(period => new AggregationSetup(parameters, period));
            }).ToList();

            try
            {
                // prepare buffers
                foreach (var setup in setups)
                {
                    // translate method name
                    var methodIdentifier = setup.Aggregation.Method switch
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
                        AggregationMethod.Sum => "sum",
                        _ => throw new Exception($"The aggregation method '{setup.Aggregation.Method}' is unknown.")
                    };

                    var targetDatasetPath = $"{groupPath}/{(int)setup.Period} s_{methodIdentifier}";

                    if (force || !IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
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
                    return;

                // process data
                var fundamentalPeriod = TimeSpan.FromMinutes(10); // required to ensure that the aggregation functions gets data with a multiple length of 10 minutes
                var endDate = date.AddDays(1);
                var blockSizeLimit = _aggregationChunkSizeMb * 1000 * 1000;

                // read data
                foreach (var progressRecord in dataReader.Read(dataset, date, endDate, blockSizeLimit, fundamentalPeriod, CancellationToken.None))
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
                }

                // write data to file
                foreach (var entry in setupToDataMap)
                {
                    var setup = entry.Key;
                    var bufferData = setupToBufferDataMap[setup];

                    IOHelper.Write(bufferData.DatasetId, bufferData.Buffer, DataContainerType.Dataset);
                    H5F.flush(bufferData.DatasetId, H5F.scope_t.LOCAL);
                }
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
                var aggregation = setup.Aggregation;
                var period = setup.Period;
                var periodData = item.Value;

                switch (aggregation.Method)
                {
                    case AggregationMethod.Mean:
                    case AggregationMethod.MeanPolar:
                    case AggregationMethod.Min:
                    case AggregationMethod.Max:
                    case AggregationMethod.Std:
                    case AggregationMethod.Rms:
                    case AggregationMethod.SampleAndHold:
                    case AggregationMethod.Sum:

                        if (dataset_double == null)
                            dataset_double = BufferUtilities.ApplyDatasetStatus<T>(data, statusSet);

                        setupToPartialBufferMap[setup] = this.ApplyAggregationFunction(aggregation, (int)periodData.SampleCount, dataset_double, nanLimit, _logger);

                        break;

                    case AggregationMethod.MinBitwise:
                    case AggregationMethod.MaxBitwise:

                        setupToPartialBufferMap[setup] = this.ApplyAggregationFunction(aggregation, (int)periodData.SampleCount, data, statusSet, nanLimit, _logger);

                        break;

                    default:

                        _logger.LogWarning($"The aggregation method '{aggregation.Method}' is not known. Skipping period {period}.");

                        continue;
                }
            }

            return setupToPartialBufferMap;
        }

        internal double[] ApplyAggregationFunction(Aggregation aggregation, int kernelSize, double[] data, double nanLimit, ILogger logger)
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];
            var method = aggregation.Method;
            var argument = aggregation.Argument;

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

                case AggregationMethod.Sum:

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, x, kernelSize);
                        var isHighQuality = (chunkData.Length / (double)kernelSize) >= nanLimit;

                        if (isHighQuality)
                            result[x] = Vector<double>.Build.Dense(chunkData).Sum();
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

        internal double[] ApplyAggregationFunction<T>(Aggregation aggregation, int kernelSize, T[] data, byte[] statusSet, double nanLimit, ILogger logger) where T : unmanaged
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];
            var method = aggregation.Method;
            var argument = aggregation.Argument;

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

        private bool ApplyAggregationFilter(ChannelInfo channel, Dictionary<string, string> filters)
        {
            bool result = true;

            // channel
            if (filters.ContainsKey("--include-channel"))
                result &= Regex.IsMatch(channel.Name, filters["--include-channel"]);

            if (filters.ContainsKey("--exclude-channel"))
                result &= !Regex.IsMatch(channel.Name, filters["--exclude-channel"]);

            // group
            if (filters.ContainsKey("--include-group"))
                result &= channel.Group.Split('\n').Any(groupName => Regex.IsMatch(groupName, filters["--include-group"]));

            if (filters.ContainsKey("--exclude-group"))
                result &= !channel.Group.Split('\n').Any(groupName => Regex.IsMatch(groupName, filters["--exclude-group"]));

            // unit
            if (filters.ContainsKey("--include-unit"))
            {
#warning Remove this special case check.
                if (channel.Unit == null)
                {
                    _logger.LogWarning("Unit 'null' value detected.");
                    result &= false;
                }
                else
                {
                    result &= Regex.IsMatch(channel.Unit, filters["--include-unit"]);
                }
            }

            if (filters.ContainsKey("--exclude-unit"))
            {
#warning Remove this special case check.
                if (channel.Unit == null)
                {
                    _logger.LogWarning("Unit 'null' value detected.");
                    result &= true;

                }
                else
                {
                    result &= !Regex.IsMatch(channel.Unit, filters["--exclude-unit"]);
                }
            }

            return result;
        }

        #endregion
    }
}
