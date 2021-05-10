using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneDas.Buffers;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Extensibility;
using OneDas.Infrastructure;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static OneDas.DataManagement.Explorer.Services.DatabaseManager;

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

        public static List<AggregationInstruction> ComputeInstructions(AggregationSetup setup, DatabaseManagerState state, ILogger logger)
        {
            var projectIds = setup.Aggregations
                .Select(aggregation => aggregation.ProjectId)
                .Distinct().ToList();

            return projectIds.Select(projectId =>
            {
                var container = state.Database.ProjectContainers.FirstOrDefault(container => container.Id == projectId);

                if (container is null)
                    return null;

                var dataReaderRegistrations = container
                    .Project
                    .Channels
                    .SelectMany(channel => channel.Datasets.Select(dataset => dataset.Registration))
                    .Distinct()
                    .Where(registration => registration != state.AggregationRegistration)
                    .ToList();

                return new AggregationInstruction(container, dataReaderRegistrations.ToDictionary(registration => registration, registration =>
                {
                    // find aggregations for project ID
                    var potentialAggregations = setup.Aggregations
                        .Where(parameters => parameters.ProjectId == container.Project.Id)
                        .ToList();

                    // create channel to aggregations map
                    var aggregationChannels = container.Project.Channels
                        // find all channels for current reader registration
                        .Where(channel => channel.Datasets.Any(dataset => dataset.Registration == registration))
                        // find all aggregations for current channel
                        .Select(channel =>
                        {
                            var channelMeta = container.ProjectMeta.Channels
                                .First(current => current.Id == channel.Id);

                            return new AggregationChannel()
                            {
                                Channel = channel,
                                Aggregations = potentialAggregations.Where(current => AggregationService.ApplyAggregationFilter(channel, channelMeta, current.Filters, logger)).ToList()
                            };
                        })
                        // take all channels with aggregations
                        .Where(aggregationChannel => aggregationChannel.Aggregations.Any());

                    return aggregationChannels.ToList();
                }));
            }).Where(instruction => instruction != null).ToList();
        }

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
                    var instructions = AggregationService.ComputeInstructions(setup, _databaseManager.State, _logger);
                    var days = (setup.End - setup.Begin).TotalDays;
                    var totalDays = instructions.Count() * days;
                    var i = 0;

                    foreach (var instruction in instructions)
                    {
                        var projectId = instruction.Container.Id;

                        for (int j = 0; j < days; j++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var currentDay = setup.Begin.AddDays(j);
                            var progressMessage = $"Processing project '{projectId}': {currentDay.ToString("yyyy-MM-dd")}";
                            var progressValue = (i * days + j) / totalDays;
                            var eventArgs = new ProgressUpdatedEventArgs(progressValue, progressMessage);
                            progress.Report(eventArgs);

                            this.AggregateProject(userIdService.User, databaseFolderPath, projectId, currentDay, setup, instruction, cancellationToken);
                        }

                        i++;
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
                                      AggregationInstruction instruction,
                                      CancellationToken cancellationToken)
        {
            foreach (var (registration, aggregationChannels) in instruction.DataReaderToAggregationsMap)
            {
                using var dataReader = _databaseManager.GetDataReader(user, registration);

                // find reader configurations
                foreach (var configuration in setup.ReaderConfigurations
                    .Where(configuration => configuration.ProjectId == projectId))
                {
                    var tmpRegistration = new DataReaderRegistration()
                    {
                        RootPath = configuration.DataReaderRootPath,
                        DataReaderId = configuration.DataReaderId
                    };

                    if (dataReader.Registration.Equals(tmpRegistration))
                    {
                        dataReader.OptionalParameters = configuration.Parameters;
                        break;
                    }
                }                   

                // get files
                if (!dataReader.IsDataOfDayAvailable(projectId, date))
                    return;

                // project
                var container = _databaseManager.Database.ProjectContainers.FirstOrDefault(container => container.Id == projectId);

                if (container == null)
                    throw new Exception($"The requested project '{projectId}' could not be found.");

                var targetDirectoryPath = Path.Combine(databaseFolderPath, "DATA", WebUtility.UrlEncode(container.Id), date.ToString("yyyy-MM"), date.ToString("dd"));

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
                                                            new object[] 
                                                            {
                                                                targetDirectoryPath,
                                                                dataReader, 
                                                                dataset, 
                                                                aggregationChannel.Aggregations, 
                                                                date, 
                                                                setup.Force, 
                                                                cancellationToken
                                                            });
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
        }

        private void OrchestrateAggregation<T>(string targetDirectoryPath,
                                               DataReaderExtensionBase dataReader,
                                               DatasetInfo dataset,
                                               List<Aggregation> aggregations,
                                               DateTime date,
                                               bool force,
                                               CancellationToken cancellationToken) where T : unmanaged
        {
            // check source sample rate
            var sampleRate = new SampleRateContainer(dataset.Id, ensureNonZeroIntegerHz: true);

            // prepare variables
            var units = new List<AggregationUnit>();
            var channel = (ChannelInfo)dataset.Parent;

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

                        var targetFileName = $"{channel.Id}_{period}_s_{methodIdentifier}.nex";
                        var targetFilePath = Path.Combine(targetDirectoryPath, targetFileName);

                        if (force || !File.Exists(targetFileName))
                        {
                            var buffer = new double[86400 / period];

                            var unit = new AggregationUnit()
                            {
                                Aggregation = aggregation,
                                Period = period,
                                Method = method,
                                Argument = arguments,
                                Buffer = buffer,
                                TargetFilePath = targetFilePath
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
                    var unit = entry.Key;

                    Array.Copy(partialBuffer, 0, unit.Buffer, unit.BufferPosition, partialBuffer.Length);
                    unit.BufferPosition += partialBuffer.Length;
                }
            }

            // write data to file
            foreach (var unit in units)
            {
                try
                {
                    _fileAccessManager.Register(unit.TargetFilePath, cancellationToken);

                    if (File.Exists(unit.TargetFilePath))
                        File.Delete(unit.TargetFilePath);

                    // create data file
                    AggregationFile.Create<double>(unit.TargetFilePath, unit.Buffer);
                }
                finally
                {
                    _fileAccessManager.Unregister(unit.TargetFilePath);
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

        private static bool ApplyAggregationFilter(ChannelInfo channel, ChannelMeta channelMeta, Dictionary<AggregationFilter, string> filters, ILogger logger)
        {
            bool result = true;

            // channel
            if (filters.ContainsKey(AggregationFilter.IncludeChannel))
                result &= Regex.IsMatch(channel.Name, filters[AggregationFilter.IncludeChannel]);

            if (filters.ContainsKey(AggregationFilter.ExcludeChannel))
                result &= !Regex.IsMatch(channel.Name, filters[AggregationFilter.ExcludeChannel]);

            // group
            if (filters.ContainsKey(AggregationFilter.IncludeGroup))
                result &= channel.Group.Split('\n').Any(groupName => Regex.IsMatch(groupName, filters[AggregationFilter.IncludeGroup]));

            if (filters.ContainsKey(AggregationFilter.ExcludeGroup))
                result &= !channel.Group.Split('\n').Any(groupName => Regex.IsMatch(groupName, filters[AggregationFilter.ExcludeGroup]));

            // unit
            if (filters.ContainsKey(AggregationFilter.IncludeUnit))
            {
#warning Remove this special case check.
                if (channel.Unit == null)
                {
                    logger.LogWarning("Unit 'null' value detected.");
                    result &= false;
                }
                else
                {
                    var unit = !string.IsNullOrWhiteSpace(channelMeta.Unit)
                        ? channelMeta.Unit
                        : channel.Unit;

                    result &= Regex.IsMatch(unit, filters[AggregationFilter.IncludeUnit]);
                }
            }

            if (filters.ContainsKey(AggregationFilter.ExcludeUnit))
            {
#warning Remove this special case check.
                if (channel.Unit == null)
                {
                    logger.LogWarning("Unit 'null' value detected.");
                    result &= true;

                }
                else
                {
                    var unit = !string.IsNullOrWhiteSpace(channelMeta.Unit)
                        ? channelMeta.Unit
                        : channel.Unit;

                    result &= !Regex.IsMatch(unit, filters[AggregationFilter.ExcludeUnit]);
                }
            }

            return result;
        }

        #endregion
    }
}
