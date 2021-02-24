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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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

        private FileAccessManager _fileAccessManager;
        private SignInManager<IdentityUser> _signInManager;
        private DatabaseManager _databaseManager;

        #endregion

        #region Constructors

        public AggregationService(
            FileAccessManager fileAccessManager,
            SignInManager<IdentityUser> signInManager,
            DatabaseManager databaseManager, 
            OneDasExplorerOptions options, 
            ILoggerFactory loggerFactory)
        {
            _fileAccessManager = fileAccessManager;
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

        public Task<string> AggregateDataAsync(UserIdService userIdService,
                                               string databaseFolderPath,
                                               AggregationSetup setup,
                                               CancellationToken cancellationToken)
        {
            if (setup.Begin != setup.Begin.Date)
                throw new ValidationException("The begin parameter must have no time component.");

            if (setup.End != setup.End.Date)
                throw new ValidationException("The end parameter must have no time component.");

            return Task.Run(() =>
            {
                // log
                var message = $"User '{userIdService.GetUserId()}' aggregates data: {setup.Begin.ToISO8601()} to {setup.End.ToISO8601()} ... ";
                _logger.LogInformation(message);

                try
                {
                    var progress = (IProgress<ProgressUpdatedEventArgs>)this.Progress;
                    progress.Report(new ProgressUpdatedEventArgs(0, "Updating database ..."));

                    _databaseManager.Update(CancellationToken.None);

                    var projectIds = setup.Aggregations
                        .Select(aggregation => aggregation.ProjectId)
                        .Distinct().ToList();
                    var days = (setup.End - setup.Begin).TotalDays;
                    var totalDays = projectIds.Count * days;

                    for (int i = 0; i < projectIds.Count; i++)
                    {
                        var projectId = projectIds[i];

                        for (int j = 0; j < days; j++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var currentDay = setup.Begin.AddDays(j);
                            var progressMessage = $"Processing project '{projectId}': {currentDay.ToString("yyyy-MM-dd")}";
                            var progressValue = (i * days + j) / totalDays;
                            var eventArgs = new ProgressUpdatedEventArgs(progressValue, progressMessage);
                            progress.Report(eventArgs);

                            this.AggregateProject(userIdService.User, databaseFolderPath, projectId, currentDay, setup, cancellationToken);
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

        private void AggregateProject(ClaimsPrincipal user,
                                      string databaseFolderPath,
                                      string projectId,
                                      DateTime date,
                                      AggregationSetup setup,
                                      CancellationToken cancellationToken)
        {
            var subfolderName = date.ToString("yyyy-MM");
            var targetDirectoryPath = Path.Combine(databaseFolderPath, "DATA", subfolderName);
            var dataReaders = _databaseManager.GetDataReaders(user, projectId, excludeAggregation: true);

            foreach (var dataReaderForUsing in dataReaders)
            {
                using var dataReader = dataReaderForUsing;

                // find reader configurations
                foreach (var configuration in setup.ReaderConfigurations
                    .Where(configuration => configuration.ProjectId == projectId))
                {
                    var registration = new DataReaderRegistration()
                    {
                        RootPath = configuration.DataReaderRootPath,
                        DataReaderId = configuration.DataReaderId
                    };

                    if (dataReader.Registration.Equals(registration))
                    {
                        dataReader.OptionalParameters = configuration.Parameters;
                        break;
                    }
                }                   

                // get files
                if (!dataReader.IsDataOfDayAvailable(projectId, date))
                    return;

                // process data
                try
                {
                    var targetFileId = -1L;

                    // project
                    if (!dataReader.TryGetProject(projectId, out var project))
                        throw new Exception($"The requested project '{projectId}' could not be found.");

                    // targetFileId
                    var projectFileName = projectId.TrimStart('/').Replace("/", "_");
                    var dateTimeFileName = date.ToISO8601();
                    var targetFileName = $"{projectFileName}_{dateTimeFileName}.h5";
                    var targetFilePath = Path.Combine(targetDirectoryPath, targetFileName);

                    if (!Directory.Exists(targetDirectoryPath))
                        Directory.CreateDirectory(targetDirectoryPath);

                    try
                    {
                        _fileAccessManager.Register(targetFilePath, cancellationToken);

                        var fapl = H5P.create(H5P.FILE_ACCESS);
                        var res = H5P.set_libver_bounds(fapl, H5F.libver_t.V110, H5F.libver_t.V110);

                        if (File.Exists(targetFilePath))
                            targetFileId = H5F.open(targetFilePath, H5F.ACC_RDWR, fapl);

                        if (targetFileId == -1)
                            targetFileId = H5F.create(targetFilePath, H5F.ACC_TRUNC, access_plist: fapl);

                        if (H5I.is_valid(fapl) > 0) { H5P.close(fapl); }

                        // create attribute if necessary
                        if (H5A.exists(targetFileId, "date_time") == 0)
                        {
                            var dateTimeString = date.ToISO8601();
                            IOHelper.PrepareAttribute(targetFileId, "date_time", new string[] { dateTimeString }, new ulong[] { 1 }, true);
                        }

                        // find aggregations for project ID
                        var potentialAggregations = setup.Aggregations
                            .Where(parameters => parameters.ProjectId == project.Id)
                            .ToList();

                        // create channel to aggregations map
                        var aggregationChannels = project.Channels
                            // find all aggregations for current channel
                            .Select(channel =>
                            {
                                return new AggregationChannel()
                                {
                                    Channel = channel,
                                    Aggregations = potentialAggregations.Where(current => this.ApplyAggregationFilter(channel, current.Filters)).ToList()
                                };
                            })
                            // keep all channels with aggregations
                            .Where(aggregationChannel => aggregationChannel.Aggregations.Any());

                        // for each channel
                        foreach (var aggregationChannel in aggregationChannels)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            try
                            {
                                var dataset = aggregationChannel.Channel.Datasets.First();

                                OneDasUtilities.InvokeGenericMethod(this, nameof(this.OrchestrateAggregation),
                                                                    BindingFlags.Instance | BindingFlags.NonPublic,
                                                                    OneDasUtilities.GetTypeFromOneDasDataType(dataset.DataType),
                                                                    new object[] { dataReader, dataset, aggregationChannel.Aggregations, date, targetFileId, setup.Force, cancellationToken });
                            }
                            catch (TaskCanceledException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.GetFullMessage());
                            }
                        }
                    }
                    finally
                    {
                        if (H5I.is_valid(targetFileId) > 0) { H5F.close(targetFileId); }
                        _fileAccessManager.Unregister(targetFilePath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.GetFullMessage());
                }
            }
        }

        private void OrchestrateAggregation<T>(DataReaderExtensionBase dataReader,
                                               DatasetInfo dataset,
                                               List<Aggregation> aggregations,
                                               DateTime date,
                                               long targetFileId,
                                               bool force,
                                               CancellationToken cancellationToken) where T : unmanaged
        {
            // check source sample rate
            var sampleRate = new SampleRateContainer(dataset.Id, ensureNonZeroIntegerHz: true);

            // prepare period data
            var groupPath = dataset.Parent.GetPath();
            var units = new List<AggregationUnit>();

            try
            {
                // prepare buffers
                foreach (var aggregation in aggregations)
                {
                    var periodsToSkip = new List<int>();

                    foreach (var period in aggregation.Periods)
                    {
#warning Ensure that period is a sensible value

                        foreach (var entry in aggregation.Methods)
                        {
                            var method = entry.Key;
                            var arguments = entry.Value;

                            // translate method name
                            var methodIdentifier = method switch
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
                                _ => throw new Exception($"The aggregation method '{method}' is unknown.")
                            };

                            var targetDatasetPath = $"{groupPath}/{period} s_{methodIdentifier}";

                            if (force || !IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
                            {
                                // buffer data
                                var targetBufferInfo = new AggregationTargetBufferInfo(period);
                                var bufferSize = (ulong)targetBufferInfo.Buffer.Length;
                                var datasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, bufferSize, 1).DatasetId;

                                if (!(H5I.is_valid(datasetId) > 0))
                                    throw new FormatException($"Could not open dataset '{targetDatasetPath}'.");

                                targetBufferInfo.DatasetId = datasetId;

                                var unit = new AggregationUnit()
                                {
                                    Aggregation = aggregation,
                                    Period = period,
                                    Method = method,
                                    Argument = arguments,
                                    TargetBufferInfo = targetBufferInfo
                                };

                                units.Add(unit);
                            }
                            else
                            {
                                // skip period / method combination
                            }
                        }
                    }
                }

                if (!units.Any())
                    return;

                // process data
                var fundamentalPeriod = TimeSpan.FromMinutes(10); // required to ensure that the aggregation functions get data with a multiple length of 10 minutes
                var endDate = date.AddDays(1);
                var blockSizeLimit = _aggregationChunkSizeMb * 1000 * 1000;

                // read raw data
                foreach (var progressRecord in dataReader.Read(dataset, date, endDate, blockSizeLimit, fundamentalPeriod, cancellationToken))
                {
                    var dataRecord = progressRecord.DatasetToRecordMap.First().Value;

                    // aggregate data
                    var partialBuffersMap = this.ApplyAggregationFunction(dataset, (T[])dataRecord.Dataset, dataRecord.Status, units);

                    foreach (var entry in partialBuffersMap)
                    {
                        // copy aggregated data to target buffer
                        var partialBuffer = entry.Value;
                        var targetBufferInfo = entry.Key.TargetBufferInfo;

                        Array.Copy(partialBuffer, 0, targetBufferInfo.Buffer, targetBufferInfo.BufferPosition, partialBuffer.Length);
                        targetBufferInfo.BufferPosition += partialBuffer.Length;
                    }
                }

                // write data to file
                foreach (var unit in units)
                {
                    var targetBufferInfo = unit.TargetBufferInfo;

                    IOHelper.Write(targetBufferInfo.DatasetId, targetBufferInfo.Buffer, DataContainerType.Dataset);
                    H5F.flush(targetBufferInfo.DatasetId, H5F.scope_t.LOCAL);
                }
            }
            finally
            {
                foreach (var unit in units)
                {
                    var datasetId = unit.TargetBufferInfo.DatasetId;

                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }
            }
        }

        private Dictionary<AggregationUnit, double[]> ApplyAggregationFunction<T>(DatasetInfo dataset,
                                                                              T[] data,
                                                                              byte[] status,
                                                                              List<AggregationUnit> aggregationUnits) where T : unmanaged
        {
            var nanLimit = 0.99;
            var dataset_double = default(double[]);
            var partialBuffersMap = new Dictionary<AggregationUnit, double[]>();

            foreach (var unit in aggregationUnits)
            {
                var aggregation = unit.Aggregation;
                var period = unit.Period;
                var method = unit.Method;
                var argument = unit.Argument;
                var sampleCount = dataset.GetSampleRate(ensureNonZeroIntegerHz: true).SamplesPerSecondAsUInt64 * (ulong)period;

                switch (unit.Method)
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
                            dataset_double = BufferUtilities.ApplyDatasetStatus<T>(data, status);

                        partialBuffersMap[unit] = this.ApplyAggregationFunction(method, argument, (int)sampleCount, dataset_double, nanLimit, _logger);

                        break;

                    case AggregationMethod.MinBitwise:
                    case AggregationMethod.MaxBitwise:

                        partialBuffersMap[unit] = this.ApplyAggregationFunction(method, argument, (int)sampleCount, data, status, nanLimit, _logger);

                        break;

                    default:

                        _logger.LogWarning($"The aggregation method '{unit.Method}' is not known. Skipping period {period}.");

                        continue;
                }
            }

            return partialBuffersMap;
        }

        private double[] ApplyAggregationFunction(AggregationMethod method,
                                                  string argument,
                                                  int kernelSize,
                                                  double[] data,
                                                  double nanLimit,
                                                  ILogger logger)
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];

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

        private double[] ApplyAggregationFunction<T>(AggregationMethod method,
                                                     string argument,
                                                     int kernelSize,
                                                     T[] data,
                                                     byte[] status,
                                                     double nanLimit,
                                                     ILogger logger) where T : unmanaged
        {
            var targetDatasetLength = data.Length / kernelSize;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case AggregationMethod.MinBitwise:

                    T[] bitField_and = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var chunkData = this.GetNaNFreeData(data, status, x, kernelSize);
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
                        var chunkData = this.GetNaNFreeData(data, status, x, kernelSize);
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

        private unsafe T[] GetNaNFreeData<T>(T[] data, byte[] status, int index, int kernelSize) where T : unmanaged
        {
            var sourceIndex = index * kernelSize;
            var length = data.Length;
            var chunkData = new List<T>();

            for (int i = 0; i < length; i++)
            {
                if (status[sourceIndex + i] == 1)
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
