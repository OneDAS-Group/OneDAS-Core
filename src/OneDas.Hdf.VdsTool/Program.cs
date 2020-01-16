using FluentFTP;
using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.Database;
using OneDas.DataStorage;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Hdf.VdsTool.Commands;
using OneDas.Hdf.VdsTool.Documentation;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.VdsTool
{
    class Program
    {
        #region Fields

        private static ILoggerFactory _loggerFactory;

        #endregion

        #region Methods

        private static async Task<int> Main(string[] args)
        {
            // try/catch is necessary to support command tab completion
            try
            {
                if (!OneDasUtilities.ValidateDatabaseFolderPath(Environment.CurrentDirectory, out var message))
                {
                    Console.WriteLine(message);
                    return 1;
                }

                Console.CursorVisible = false;
                Console.Title = "VdsTool";
            }
            catch (Exception)
            {
                //
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // configure logging
            var serviceProvider = new ServiceCollection().AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile(Path.Combine(Environment.CurrentDirectory, "SUPPORT", "LOGS", "VdsTool-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
            }).BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            // configure CLI
            var rootCommand = new RootCommand("Virtual dataset tool");

            rootCommand.AddCommand(Program.PreparePwshCommand());
            rootCommand.AddCommand(Program.PrepareUpdateCommand());
            rootCommand.AddCommand(Program.PrepareVdsCommand());
            rootCommand.AddCommand(Program.PrepareAggregateCommand());
            rootCommand.AddCommand(Program.PrepareDocCommand());

            return await rootCommand.InvokeAsync(args);
        }

        private static bool HandleEscape()
        {
            ConsoleKeyInfo consoleKeyInfo;

            Console.Clear();
            Console.WriteLine("The application will be closed. Proceed (y/n)?");

            consoleKeyInfo = Console.ReadKey(true);

            if (consoleKeyInfo.Key == ConsoleKey.Y)
            {
                return true;
            }

            return false;
        }

        private static void ClearCurrentLine()
        {
            Console.Write($"\r{ new string(' ', Console.WindowWidth - 1) }\r");
        }

        #endregion

        #region Commands

        private static Command PreparePwshCommand()
        {
            var command = new Command("pwsh", "Runs the provided Powershell script")
            {
                new Option("--script-path", "The location of the powershell script")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--transaction-id", "Log messages are tagged with the transaction identifier")
                {
                    Argument = new Argument<string>(),
                    Required = true
                }
            };

            command.Handler = CommandHandler.Create((string scriptPath, string transactionId) =>
            {
                var logger = _loggerFactory.CreateLogger($"PWSH ({transactionId})");

                try
                {
                    Program.ExecutePwsh(scriptPath, logger);
                    logger.LogInformation($"Execution of script '{scriptPath}' finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution 'pwsh' command failed (path: '{scriptPath}'). Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareUpdateCommand()
        {
            var command = new Command("update", "Updates the database index")
            {
                //
            };

            command.Handler = CommandHandler.Create(() =>
            {
                var logger = _loggerFactory.CreateLogger("UPDATE");

                try
                {
                    DateTime epochStart;
                    var date = DateTime.UtcNow.Date;

                    epochStart = new DateTime(date.Year, date.Month, 1).AddMonths(-1);
                    new VdsCommand(epochStart, logger).Run();

                    epochStart = new DateTime(date.Year, date.Month, 1);
                    new VdsCommand(epochStart, logger).Run();

                    epochStart = DateTime.MinValue;
                    new VdsCommand(epochStart, logger).Run();

                    logger.LogInformation($"Execution of the 'update' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'vds' command failed. Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareVdsCommand()
        {
            bool TryConvertArgument(SymbolResult result, out DateTime value)
            {
                return DateTime.TryParseExact(result.Token.Value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
            }

            var command = new Command("vds", "Updates the database index of files that are part of the specified epoch")
            {
                new Option("--epoch-start", "The start date of the epoch")
                {
                    Argument = new Argument<DateTime>(TryConvertArgument),
                    Required = false
                }
            };

            command.Handler = CommandHandler.Create((DateTime epochStart) =>
            {
                var logger = _loggerFactory.CreateLogger("VDS");

                try
                {
                    new VdsCommand(epochStart, logger).Run();
                    logger.LogInformation($"Execution of the 'vds' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'vds' command failed. Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareAggregateCommand()
        {
            bool TryConvertArgument(SymbolResult result, out DateTime value)
            {
                return DateTime.TryParseExact(result.Token.Value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
            }

            var command = new Command("aggregate", "Aggregates data of channels that match the filter conditions.")
            {
                new Option("--epoch-start", "The start date of the epoch")
                {
                    Argument = new Argument<DateTime>(TryConvertArgument),
                    Required = true
                },
                new Option("--method", "Possible arguments are 'mean', 'mean_polar', 'min', 'max', 'std', 'rms', 'min_bitwise' and 'max_bitwise'")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--argument", "Zero or more arguments for the selected aggregation method.")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--campaign-name", "The campaign name, e.g /A/B/C.")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--include-channel", "A regex based filter to include channels with certain names.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--include-group", "A regex based filter to include channels within certain groups.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--include-unit", "A regex based filter to include channels with certain unit.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--exclude-channel", "A regex based filter to exclude channels with certain names.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--exclude-group", "A regex based filter to exclude channels within certain groups.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--exclude-unit", "A regex based filter to exclude channels with certain unit.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                }
            };

            command.Handler = CommandHandler.Create((DateTime epochStart, string method, string argument, string campaignName, ParseResult parseResult) =>
            {
                var logger = _loggerFactory.CreateLogger("AGGREGATE");
                var filters = new Dictionary<string, string>();
                var skip = 7;
                var isOption = true;

                for (int i = skip; i < parseResult.Tokens.Count - 1; i++)
                {
                    if (isOption)
                    {
                        var filterOption = parseResult.Tokens[i].Value;
                        var filterArgument = parseResult.Tokens[i + 1].Value;

                        filters[filterOption] = filterArgument;
                    }

                    isOption = !isOption;
                }

                try
                {
                    Program.CreateAggregatedFiles(epochStart, method, argument, campaignName, filters, logger);
                    logger.LogInformation($"Execution of the 'aggregate' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'aggregate' command failed. Error message: '{ex.ToString()}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareDocCommand()
        {
            var command = new Command("doc", "Exports the current VDS content to the specified directory.")
            {
                new Option("--campaign-name", "The campaign name to export the documentation for.")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--output-dir", "The output directory, where the exported VDS file content will be written to")
                {
                    Argument = new Argument<string>(),
                    Required = true
                }
            };

            command.Handler = CommandHandler.Create((string campaignName, string outputDir) =>
            {
                var logger = _loggerFactory.CreateLogger("DOC");

                try
                {
                    logger.LogInformation($"Execution of the 'doc' command finished successfully.");
                    Program.WriteCampaignDocumentation(campaignName, outputDir, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'doc' command failed. Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        #endregion

        #region AGGREGATION

        public static void CreateAggregatedFiles(DateTime epochStart, string method, string argument, string campaignName, Dictionary<string, string> filters, ILogger logger)
        {
            var epochEnd = epochStart.AddMonths(1);
            var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_NATIVE", epochStart.ToString("yyyy-MM"));
            var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_AGGREGATION", epochStart.ToString("yyyy-MM"));

            logger.LogInformation($"Epoch start: {epochStart.ToString("yyyy-MM-dd")}{Environment.NewLine}Epoch   end: {epochEnd.ToString("yyyy-MM-dd")}");

            Program.InternalCreateAggregatedFiles(sourceDirectoryPath, targetDirectoryPath, method, argument, campaignName, filters, logger);
        }

        private static void InternalCreateAggregatedFiles(string sourceDirectoryPath, string targetDirectoryPath, string method, string argument, string campaignName, Dictionary<string, string> filters, ILogger logger)
        {
            long sourceFileId = -1;
            long targetFileId = -1;

            IList<string> filePathSet;

            if (Directory.Exists(sourceDirectoryPath))
                filePathSet = Directory.GetFiles(sourceDirectoryPath, "*.h5", SearchOption.TopDirectoryOnly);
            else
                filePathSet = new List<string>();

            try
            {
                foreach (string sourceFilePath in filePathSet)
                {
                    targetFileId = -1;

                    logger.LogInformation($"file: {sourceFilePath}");
                    Console.WriteLine();

                    // sourceFileId
                    sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);
                    var campaignInfo = GeneralHelper.GetCampaignInfo(sourceFileId, campaignName);

                    if (campaignInfo == null)
                        continue;

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
                            IOHelper.PrepareAttribute(targetFileId, "date_time", dateTime.ToArray(), new ulong[] { 1 }, true);
                        }

                        // campaignInfo
                        Program.AggregateCampaign(sourceFileId, targetFileId, campaignInfo, method, argument, filters, logger);

                        Console.CursorTop -= 1;
                    }
                    finally
                    {
                        if (H5I.is_valid(targetFileId) > 0) { H5F.close(targetFileId); }
                        if (H5I.is_valid(sourceFileId) > 0) { H5F.close(sourceFileId); }
                    }
                }
            }
            finally
            {
                //
            }
        }

        private static void AggregateCampaign(long sourceFileId, long targetFileId, CampaignInfo campaignInfo, string method, string argument, Dictionary<string, string> filters, ILogger logger)
        {
            var index = 0;
            var datasetPath = $"{campaignInfo.GetPath()}/is_chunk_completed_set";
            (var groupId, var isNew) = IOHelper.OpenOrCreateGroup(targetFileId, campaignInfo.GetPath());

            try
            {
                if (isNew || !IOHelper.CheckLinkExists(targetFileId, datasetPath))
                    H5O.copy(sourceFileId, datasetPath, targetFileId, datasetPath);

                var filteredVariableInfos = campaignInfo.VariableInfoSet.Where(variableInfo => Program.ApplyAggregationFilter(variableInfo, filters, logger)).ToList();

                foreach (var filteredVariableInfo in filteredVariableInfos)
                {
                    index++;

                    Console.WriteLine($"{filteredVariableInfo.VariableNameSet.Last()} ({index}/{filteredVariableInfos.Count()})");
                    Program.AggregateVariable(sourceFileId, targetFileId, filteredVariableInfo, method, argument, logger);
                    Console.CursorTop -= 1;
                    Program.ClearCurrentLine();
                }
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
            }
        }

        private static void AggregateVariable(long sourceFileId, long targetFileId, VariableInfo variableInfo, string method, string argument, ILogger logger)
        {
            long sourceDatasetId = -1;
            long sourceDatasetId_status = -1;
            long typeId = -1;

            var sampleRate = (SampleRate)0;
            var groupPath = variableInfo.GetPath();

            try
            {
                // find proper data source
                foreach (SampleRate testedSampleRate in Enum.GetValues(typeof(SampleRate)))
                {
                    var sourceDatasetName = new SampleRateContainer(testedSampleRate).ToUnitString();

                    if (variableInfo.DatasetInfoSet.Where(x => x.Name == sourceDatasetName).Any())
                    {
                        sampleRate = testedSampleRate;
                        sourceDatasetId = H5D.open(sourceFileId, $"{groupPath}/{sourceDatasetName}");
                        sourceDatasetId_status = H5D.open(sourceFileId, $"{groupPath}/{sourceDatasetName}_status");
                        typeId = H5D.get_type(sourceDatasetId);

                        break;
                    }
                }

                if (sampleRate == default)
                {
                    logger.LogInformation($"No appropriate data source was found. Skipping variable '{variableInfo.Name}'.");
                    return;
                }

                typeId = H5D.get_type(sourceDatasetId);

                // invoke Program.AggregateDataset
                GeneralHelper.InvokeGenericMethod(typeof(Program), null, nameof(Program.AggregateDataset),
                                              BindingFlags.NonPublic | BindingFlags.Static,
                                              TypeConversionHelper.GetTypeFromHdfTypeId(typeId),
                                              new object[] { groupPath, sourceDatasetId, sourceDatasetId_status, targetFileId, sampleRate, method, argument, logger });

                // clean up
                H5D.close(sourceDatasetId_status);
                H5D.close(sourceDatasetId);
                H5T.close(typeId);
            }
            finally
            {
                //
            }
        }

        private static void AggregateDataset<T>(string groupPath, long sourceDatasetId, long sourceDatasetId_status, long targetFileId, SampleRate sampleRate, string method, string argument, ILogger logger)
        {
            long targetDatasetId = -1;
            double[] targetValueSet;
            double[] sourceValueSet_double = null;
            byte[] sourceValueSet_status = null;
            T[] sourceValueSet = null;

            Console.WriteLine($"\t{method}");

            // for each period
            foreach (Period period in Enum.GetValues(typeof(Period)))
            {
                var targetDatasetPath = $"{groupPath}/{(int)period} s_{method}";

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

                    switch (method)
                    {
                        case "mean":
                        case "mean_polar":
                        case "min":
                        case "max":
                        case "std":

                            if (sourceValueSet_double == null)
                            {
                                sourceValueSet_double = ExtendedDataStorageBase.ApplyDatasetStatus(sourceValueSet, sourceValueSet_status);
                            }

                            targetValueSet = Program.ApplyAggregationFunction(method, argument, chunkCount, sourceValueSet_double, logger);

                            break;

                        case "min_bitwise":
                        case "max_bitwise":

                            targetValueSet = Program.ApplyAggregationFunction(method, argument, chunkCount, sourceValueSet, sourceValueSet_status, logger);

                            break;

                        default:

                            logger.LogWarning($"The aggregation method '{method}' is not known. Skipping period {period}.");

                            continue;
                    }

                    targetDatasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, (ulong)chunkCount, 1).DatasetId;

                    try
                    {
                        IOHelper.Write(targetDatasetId, targetValueSet.ToArray(), DataContainerType.Dataset);

                        Console.CursorTop -= 1;
                        Program.ClearCurrentLine();

                        H5F.flush(targetDatasetId, H5F.scope_t.LOCAL);
                    }
                    finally
                    {
                        if (H5I.is_valid(targetDatasetId) > 0) { H5D.close(targetDatasetId); }
                    }
                }
            }

            Console.CursorTop -= 1;
            Program.ClearCurrentLine();
        }

        private static double[] ApplyAggregationFunction(string method, string argument, int targetDatasetLength, double[] valueSet, ILogger logger)
        {
            var chunkSize = valueSet.Count() / targetDatasetLength;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case "mean":

                    Parallel.For(0, targetDatasetLength, x =>
                    {

                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Mean(chunkValueSet);
                    });

                    break;

                case "mean_polar":

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

                case "min":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Minimum(chunkValueSet);
                    });

                    break;

                case "max":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Maximum(chunkValueSet);
                    });

                    break;

                case "std":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        var baseIndex = x * chunkSize;
                        var chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.StandardDeviation(chunkValueSet);
                    });

                    break;

                case "rms":

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

        private static double[] ApplyAggregationFunction<T>(string method, string argument, int targetDatasetLength, T[] valueSet, byte[] valueSet_status, ILogger logger)
        {
            var chunkSize = valueSet.Count() / targetDatasetLength;
            var result = new double[targetDatasetLength];

            switch (method)
            {
                case "min_bitwise":

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

                case "max_bitwise":

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

        private static bool ApplyAggregationFilter(VariableInfo variableInfo, Dictionary<string, string> filters, ILogger logger)
        {
            bool result = true;

            // channel
            if (filters.ContainsKey("--include-channel"))
            {
                if (variableInfo.VariableNameSet.Any())
                    result &= Regex.IsMatch(variableInfo.VariableNameSet.Last(), filters["--include-channel"]);
                else
                    result &= false;
            }

            if (filters.ContainsKey("--exclude-channel"))
            {
                if (variableInfo.VariableNameSet.Any())
                    result &= !Regex.IsMatch(variableInfo.VariableNameSet.Last(), filters["--exclude-channel"]);
                else
                    result &= true;
            }

            // group
            if (filters.ContainsKey("--include-group"))
            {
                if (variableInfo.VariableGroupSet.Any())
                    result &= variableInfo.VariableGroupSet.Last().Split('\n').Any(groupName => Regex.IsMatch(groupName, filters["--include-group"]));
                else
                    result &= false;
            }

            if (filters.ContainsKey("--exclude-group"))
            {
                if (variableInfo.VariableGroupSet.Any())
                    result &= !variableInfo.VariableGroupSet.Last().Split('\n').Any(groupName => Regex.IsMatch(groupName, filters["--exclude-group"]));
                else
                    result &= true;
            }

            // unit
            if (filters.ContainsKey("--include-unit"))
            {
#warning Remove this special case check.
                if (variableInfo.UnitSet.Last() == null)
                {
                    logger.LogWarning("Unit 'null' value detected.");
                    result &= false;
                }
                else
                {
                    if (variableInfo.UnitSet.Any())
                        result &= Regex.IsMatch(variableInfo.UnitSet.Last(), filters["--include-unit"]);
                    else
                        result &= false;
                }
            }

            if (filters.ContainsKey("--exclude-unit"))
            {
#warning Remove this special case check.
                if (variableInfo.UnitSet.Last() == null)
                {
                    logger.LogWarning("Unit 'null' value detected.");
                    result &= true;

                }
                else
                {
                    if (variableInfo.UnitSet.Any())
                        result &= !Regex.IsMatch(variableInfo.UnitSet.Last(), filters["--exclude-unit"]);
                    else
                        result &= true;
                }
            }

            return result;
        }

        #endregion

        #region PWSH

        private static void ExecutePwsh(string scriptFilePath, ILogger logger)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                // ensure FluentFTP lib is loaded
                _ = FtpExists.NoCheck;

                var vdsToolLogger = new VdsToolLogger(logger);
                logger.LogInformation($"Executing script '{scriptFilePath}'.");

                ps.Runspace.SessionStateProxy.SetVariable("dbRoot", Environment.CurrentDirectory);
                ps.Runspace.SessionStateProxy.SetVariable("logger", vdsToolLogger);

                ps.AddScript(File.ReadAllText(scriptFilePath))
                  .Invoke();
            }
        }

        #endregion

        #region DOC

        public static void WriteCampaignDocumentation(string campaignName, string targetDirectoryPath, ILogger logger)
        {
            long vdsFileId = -1;
            long vdsMetaFileId = -1;

            vdsFileId = H5F.open(Path.Combine(Environment.CurrentDirectory, "VDS.h5"), H5F.ACC_RDONLY);
            vdsMetaFileId = H5F.open(Path.Combine(Environment.CurrentDirectory, "VDS_META.h5"), H5F.ACC_RDONLY);

            try
            {
                var campaign = GeneralHelper.GetCampaignInfo(vdsFileId, campaignName);

                if (campaign == null)
                    throw new Exception($"The campaign named '{campaignName}' could not be found.");

                Program.InternalWriteCampaignDocumentation(campaign, targetDirectoryPath, vdsMetaFileId, logger);
            }
            finally
            {
                if (H5I.is_valid(vdsMetaFileId) > 0) { H5F.close(vdsMetaFileId); }
                if (H5I.is_valid(vdsFileId) > 0) { H5F.close(vdsFileId); }
            }
        }

        public static void InternalWriteCampaignDocumentation(CampaignInfo campaign, string targetDirectoryPath, long vdsMetaFileId, ILogger logger)
        {
            long campaign_groupId = -1;

            var groupNameSet = campaign.VariableInfoSet.Select(variableInfo => variableInfo.VariableGroupSet.Last()).Distinct().ToList();
            var filePath = Path.Combine(targetDirectoryPath, $"{ campaign.Name.ToLower().Replace("/", "_").TrimStart('_') }.rst");          

            using (var streamWriter = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                var restructuredTextWriter = new RestructuredTextWriter(streamWriter);

                // campaign header
                restructuredTextWriter.WriteHeading(campaign.Name.TrimStart('/').Replace("/", " / "), SectionHeader.Section);

                // campaign description
                restructuredTextWriter.WriteLine();

                try
                {
                    string description;

                    if (IOHelper.CheckLinkExists(vdsMetaFileId, campaign.Name))
                    {
                        campaign_groupId = H5G.open(vdsMetaFileId, campaign.Name);

                        if (H5A.exists(campaign_groupId, "description") > 0)
                            description = IOHelper.ReadAttribute<string>(campaign_groupId, "description").First();
                        else
                            description = "no description available";
                    }
                    else
                    {
                        description = "no description available";
                    }

                    restructuredTextWriter.WriteNote(description);
                }
                finally
                {
                    if (H5I.is_valid(campaign_groupId) > 0) { H5G.close(campaign_groupId); }
                }

                // groups
                foreach (string groupName in groupNameSet)
                {
                    restructuredTextWriter.WriteLine();
                    restructuredTextWriter.WriteHeading(groupName, SectionHeader.SubSection);
                    restructuredTextWriter.WriteLine();

                    var restructuredTextTable = new RestructuredTextTable(new List<string>() { "Name", "Unit", "Guid" });
                    var groupedVariableInfoSet = campaign.VariableInfoSet.Where(variableInfo => variableInfo.VariableGroupSet.Last() == groupName).OrderBy(variableInfo => variableInfo.VariableNameSet.Last()).ToList();

                    // variables
                    groupedVariableInfoSet.ForEach(variableInfo =>
                    {
                        long variable_groupId = -1;

                        List<hdf_transfer_function_t> transferFunctionSet = null;

                        // name
                        var name = variableInfo.VariableNameSet.Last();

                        if (name.Count() > 43)
                            name = $"{ name.Substring(0, 40) }...";

                        // guid
                        var guid = $"{ variableInfo.Name.Substring(0, 8) }...";

                        // unit, transferFunctionSet
                        var unit = string.Empty;

                        try
                        {
                            var groupPath = variableInfo.GetPath();

                            if (IOHelper.CheckLinkExists(vdsMetaFileId, groupPath))
                            {
                                variable_groupId = H5G.open(vdsMetaFileId, groupPath);

                                if (H5A.exists(variable_groupId, "unit") > 0)
                                    unit = IOHelper.ReadAttribute<string>(variable_groupId, "unit").FirstOrDefault();

                                if (H5A.exists(variable_groupId, "transfer_function_set") > 0)
                                    transferFunctionSet = IOHelper.ReadAttribute<hdf_transfer_function_t>(variable_groupId, "transfer_function_set").ToList();
                            }
                        }
                        finally
                        {
                            if (H5I.is_valid(variable_groupId) > 0) { H5G.close(variable_groupId); }
                        }

                        // 
                        restructuredTextTable.AddRow(new List<string> { name, unit, guid });

                        //if (transferFunctionSet != null)
                        //    transferFunctionSet.ForEach(x => restructuredTextWriter.WriteLine($"{ x.date_time }, { x.type }, { x.option }, { x.argument } | "));
                    });

                    restructuredTextWriter.WriteTable(restructuredTextTable);
                    restructuredTextWriter.WriteLine();
                }
            }

            logger.LogInformation($"The file has been successfully written to '{filePath}'.");
        }

        #endregion
    }
}
