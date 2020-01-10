using FluentFTP;
using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Hdf.VdsTool.Documentation;
using OneDas.Hdf.VdsTool.Navigation;
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
using System.Runtime.InteropServices;
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
            Environment.CurrentDirectory = @"D:\DATABASE";
            OneDasUtilities.ValidateDatabaseFolderPath(Environment.CurrentDirectory);

            try
            {
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

            // if CLI mode
            if (args.Any())
            {
                // configure CLI
                var rootCommand = new RootCommand("Virtual dataset tool");

                rootCommand.AddCommand(Program.PreparePwshCommand());
                rootCommand.AddCommand(Program.PrepareUpdateCommand());
                rootCommand.AddCommand(Program.PrepareVdsCommand());
                rootCommand.AddCommand(Program.PrepareAggregateCommand());
                rootCommand.AddCommand(Program.PrepareDocCommand());

                return await rootCommand.InvokeAsync(args);
            }

            uint isLibraryThreadSafe = 0;
            H5.is_library_threadsafe(ref isLibraryThreadSafe);

            if (isLibraryThreadSafe <= 0)
                Console.WriteLine("Warning: libary is not thread safe!");

            while (true)
            {
                new MainMenuNavigator();

                if (Program.HandleEscape())
                    return 0;
            }
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
                try
                {
                    Program.ExecutePwsh(scriptPath, transactionId);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not execute pwsh command. Aborting action.");
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
                try
                {
                    DateTime epochStart;

                    var date = DateTime.UtcNow.Date;

                    if (date.Day == 1)
                    {
                        epochStart = new DateTime(date.Year, date.Month, 1);
                        epochStart = epochStart.AddMonths(-1);
                        Program.CreateVirtualDatasetFile(epochStart);
                    }

                    epochStart = new DateTime(date.Year, date.Month, 1);
                    Program.CreateVirtualDatasetFile(epochStart);

                    epochStart = DateTime.MinValue;
                    Program.CreateVirtualDatasetFile(epochStart);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not update the database index. Aborting action.");
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
                    Required = true
                }
            };

            command.Handler = CommandHandler.Create((DateTime epochStart) =>
            {
                try
                {
                    Program.CreateVirtualDatasetFile(epochStart);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not update the database index. Aborting action.");
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
                new Option("--method", "Possible arguments are 'mean', 'mean_polar', 'min', 'max', 'std', 'min_bitwise' and 'max_bitwise'")
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
                    Program.CreateAggregatedFiles(epochStart, method, argument, campaignName, filters);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not calculate aggregations. Aborting action.");
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
                try
                {
                    Program.WriteCampaignDocumentation(campaignName, outputDir);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not write campaign documentation. Aborting action.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        #endregion

        #region VDS

        public static void CreateVirtualDatasetFile(DateTime epochStart)
        {
            string vdsFilePath;
            DateTime epochEnd;

            var logger = _loggerFactory.CreateLogger("VDS");
            var sourceDirectoryPathSet = new List<string>();

            if (epochStart > DateTime.MinValue)
            {
                epochEnd = epochStart.AddMonths(1);
                sourceDirectoryPathSet.Add(Path.Combine(Environment.CurrentDirectory, "DB_AGGREGATION", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPathSet.Add(Path.Combine(Environment.CurrentDirectory, "DB_IMPORT", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPathSet.Add(Path.Combine(Environment.CurrentDirectory, "DB_NATIVE", epochStart.ToString("yyyy-MM")));
                vdsFilePath = Path.Combine(Environment.CurrentDirectory, "VDS", $"{ epochStart.ToString("yyyy-MM") }.h5");
            }
            else
            {
                epochStart = new DateTime(2000, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = new DateTime(2030, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                sourceDirectoryPathSet.Add(Path.Combine(Environment.CurrentDirectory, "VDS"));
                vdsFilePath = Path.Combine(Environment.CurrentDirectory, "VDS.h5");
            }

            if (Console.CursorTop > 0 || Console.CursorLeft > 0)
                Console.WriteLine();

            logger.LogInformation($"Epoch start: {epochStart.ToString("yyyy-MM-dd")}");
            logger.LogInformation($"Epoch end: {epochEnd.ToString("yyyy-MM-dd")}");
            Console.WriteLine();

            try
            {
                Program.InternalCreateVirtualDatasetFile(sourceDirectoryPathSet, vdsFilePath, epochStart, epochEnd, logger);
                logger.LogInformation($"Execution of the 'vds' command finished successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Execution of the 'vds' command failed. Error message: '{ex.Message}'.");
                throw;
            }
        }

        private static unsafe void InternalCreateVirtualDatasetFile(List<string> sourceDirectoryPathSet, string vdsFilePath, DateTime epochStart, DateTime epochEnd, ILogger logger)
        {
            long vdsFileId = -1;

            var actualDimenionSet = new ulong[1];
            var maximumDimensionSet = new ulong[1];

            var lastVariablePath = String.Empty;
            var tempFilePath = Path.GetTempFileName();

            var campaignInfoSet = new List<CampaignInfo>();
            var sourceFilePathSet = new List<string>();
            var isChunkCompletedMap = new Dictionary<string, List<byte>>();

            // fill sourceFilePathSet
            sourceDirectoryPathSet.ToList().ForEach(x =>
            {
                if (Directory.Exists(x))
                    sourceFilePathSet.AddRange(Directory.GetFiles(x, "*.h5", SearchOption.TopDirectoryOnly).ToList());
            });

            try
            {
                // open VDS file
                vdsFileId = H5F.create(tempFilePath, H5F.ACC_TRUNC);

                // write attribute
                IOHelper.PrepareAttribute(vdsFileId, "date_time", new string[] { $"{ epochStart.ToString("yyyy-MM-ddTHH-mm-ss") }Z" }, new ulong[] { 1 }, true);

                // create an index of all campaigns, variables and datasets
                foreach (string sourceFilePath in sourceFilePathSet)
                {
                    Program.VdsSourceFile(sourceFilePath, campaignInfoSet, isChunkCompletedMap, logger);
                }

                //foreach (var variableInfo in variableInfoSet)
                //{
                //    Console.WriteLine(variableInfo.Key);
                //    Console.WriteLine($"\tVariableNameSet:{ variableInfo.Value.VariableNameSet.Count }");
                //    Console.WriteLine($"\tDatasetInfoSet: { variableInfo.Value.DatasetInfoSet.Count }");

                //    foreach (var datasetInfo in variableInfo.Value.DatasetInfoSet)
                //    {
                //        Console.WriteLine($"\t\t{ datasetInfo.Key }");
                //        Console.WriteLine($"\t\t\tLength: { datasetInfo.Value.Length }");
                //        Console.WriteLine($"\t\t\tSourceFileInfoSet: { datasetInfo.Value.SourceFileInfoSet.Count }");
                //    }
                //}

                // write the result into the temporary vds file
                foreach (var campaignInfo in campaignInfoSet)
                {
                    Program.VdsCampaign(vdsFileId, campaignInfo, epochStart, epochEnd, isChunkCompletedMap);
                }
            }
            finally
            {
                if (H5I.is_valid(vdsFileId) > 0) { H5F.close(vdsFileId); }
            }

            // copy temporary file into target location 
            try
            {
                if (File.Exists(vdsFilePath))
                    File.Delete(vdsFilePath);

                File.Copy(tempFilePath, vdsFilePath);
            }
            finally
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                    //
                }
            }
        }

        private static void VdsSourceFile(string sourceFilePath, List<CampaignInfo> campaignInfoSet, Dictionary<string, List<byte>> isChunkCompletedMap, ILogger logger)
        {
            long sourceFileId = -1;

            var message = $"Processing file { Path.GetFileName(sourceFilePath) } ... ";
            logger.LogInformation(message);

            sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);

            try
            {
                if (sourceFileId < 0)
                    throw new Exception(ErrorMessage.Program_CouldNotOpenFile);

                GeneralHelper.UpdateCampaignInfoSet(sourceFileId, campaignInfoSet);

                // load is_chunk_completed_set
                campaignInfoSet.ForEach(campaignInfo =>
                {
                    string key;
                    byte[] isChunkCompletedSet;

                    if (campaignInfo.ChunkDatasetInfo.SourceFileInfoSet.Any(sourceFileInfo => sourceFileInfo.FilePath == sourceFilePath))
                    {
                        key = $"{ sourceFilePath }+{ campaignInfo.GetPath() }";

                        if (!isChunkCompletedMap.ContainsKey(key))
                        {
                            isChunkCompletedSet = IOHelper.ReadDataset<byte>(sourceFileId, $"{ campaignInfo.GetPath() }/is_chunk_completed_set");
                            isChunkCompletedMap[key] = isChunkCompletedSet.ToList();
                        }
                    }
                });

                logger.LogInformation($"{message}Done.");
            }
            finally
            {
                if (H5I.is_valid(sourceFileId) > 0) { H5F.close(sourceFileId); }
            }
        }

        private static void VdsCampaign(long vdsFileId, CampaignInfo campaignInfo, DateTime epochStart, DateTime epochEnd, Dictionary<string, List<byte>> isChunkCompletedMap)
        {
            long campaignGroupId = -1;

            Console.WriteLine($"\n{ campaignInfo.Name }");

            campaignGroupId = IOHelper.OpenOrCreateGroup(vdsFileId, campaignInfo.GetPath()).GroupId;

            try
            {
                // variable
                foreach (var variableInfo in campaignInfo.VariableInfoSet)
                {
                    Program.VdsVariable(vdsFileId, campaignGroupId, variableInfo, epochStart, epochEnd, isChunkCompletedMap, campaignInfo.GetPath());
                }

                // don't forget is_chunk_completed_set
                Program.VdsDataset(campaignGroupId, epochStart, epochEnd, campaignInfo.ChunkDatasetInfo, campaignInfo.GetPath(), isChunkCompletedMap, false);
            }
            finally
            {
                if (H5I.is_valid(campaignGroupId) > 0) { H5G.close(campaignGroupId); }
            }
        }

        private static void VdsVariable(long vdsFileId, long vdsCampaignGroupId, VariableInfo variableInfo, DateTime epochStart, DateTime epochEnd, Dictionary<string, List<byte>> isChunkCompletedMap, string campaignPath)
        {
            long variableGroupId = -1;

            Console.WriteLine($"\t{ variableInfo.Name }");

            variableGroupId = IOHelper.OpenOrCreateGroup(vdsCampaignGroupId, variableInfo.Name).GroupId;

            try
            {
                //// make hard links for each display name
                ////foreach (string variableName in variableInfo.Value.VariableNameSet)
                ////{
                ////    H5L.copy(vdsGroupIdSet[vdsGroupIdSet.Count() - 1], variableInfo.Key, vdsGroupIdSet[vdsGroupIdSet.Count() - 2], variableName);
                ////}

                IOHelper.PrepareAttribute(variableGroupId, "name_set", variableInfo.VariableNameSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(variableGroupId, "group_set", variableInfo.VariableGroupSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(variableGroupId, "unit_set", variableInfo.UnitSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(variableGroupId, "transfer_function_set", variableInfo.TransferFunctionSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);

                // dataset
                foreach (var datasetInfo in variableInfo.DatasetInfoSet)
                {
                    Console.WriteLine($"\t\t{ datasetInfo.Name }");
                    Program.VdsDataset(variableGroupId, epochStart, epochEnd, datasetInfo, campaignPath, isChunkCompletedMap, true);
                }

                // flush data - necessary to avoid AccessViolationException at H5F.close()
                H5F.flush(vdsFileId, H5F.scope_t.LOCAL);
            }
            finally
            {
                if (H5I.is_valid(variableGroupId) > 0) { H5G.close(variableGroupId); }
            }
        }

        private static void VdsDataset(long groupId, DateTime epochStart, DateTime epochEnd, DatasetInfo datasetInfo, string campaignPath, Dictionary<string, List<byte>> isChunkCompletedMap, bool closeType)
        {
            long datasetId = -1;
            long spaceId = -1;
            long propertyId = -1;

            var createDataset = false;

            try
            {
                var datasetName = datasetInfo.Name;
                var sampleRate = new SampleRateContainer(datasetName);
                var vdsLength = (ulong)(epochEnd - epochStart).Days * sampleRate.SamplesPerDay;

                spaceId = H5S.create_simple(1, new ulong[] { vdsLength }, new ulong[] { H5S.UNLIMITED });
                propertyId = H5P.create(H5P.DATASET_CREATE);

                foreach (SourceFileInfo sourceFileInfo in datasetInfo.SourceFileInfoSet)
                {
                    long sourceSpaceId = -1;

                    var relativeFilePath = $".{sourceFileInfo.FilePath.Remove(0, Environment.CurrentDirectory.TrimEnd('\\').TrimEnd('/').Length)}";

                    sourceSpaceId = H5S.create_simple(1, new ulong[] { sourceFileInfo.Length }, new ulong[] { sourceFileInfo.Length });

                    var key = $"{ sourceFileInfo.FilePath }+{ campaignPath }";
                    var chunkCount = isChunkCompletedMap[key].Count;
                    var firstIndex = isChunkCompletedMap[key].FindIndex(value => value > 0);
                    var lastIndex = isChunkCompletedMap[key].FindLastIndex(value => value > 0);

                    var offset = (ulong)((sourceFileInfo.StartDateTime - epochStart).TotalDays * sampleRate.SamplesPerDay);
                    var start = (ulong)((double)sourceFileInfo.Length * firstIndex / chunkCount);
                    var stride = 1UL;
                    var count = 1UL;
                    var block = (ulong)((double)sourceFileInfo.Length * (lastIndex - firstIndex + 1) / chunkCount);

                    if (firstIndex >= 0)
                    {
                        createDataset = true;

                        try
                        {
                            if (offset + start + block > vdsLength)
                                throw new Exception($"start + block = { offset + start + block } > { vdsLength }");

                            H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { offset + start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                            H5S.select_hyperslab(sourceSpaceId, H5S.seloper_t.SET, new ulong[] { start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                            H5P.set_virtual(propertyId, spaceId, relativeFilePath, datasetInfo.GetPath(), sourceSpaceId);
                        }
                        finally
                        {
                            if (H5I.is_valid(sourceSpaceId) > 0) { H5S.close(sourceSpaceId); }
                        }
                    }
                }

                H5S.select_all(spaceId);

                if (TypeConversionHelper.GetTypeFromHdfTypeId(datasetInfo.TypeId) == typeof(double))
                {
                    var gcHandle = GCHandle.Alloc(Double.NaN, GCHandleType.Pinned);
                    H5P.set_fill_value(propertyId, datasetInfo.TypeId, gcHandle.AddrOfPinnedObject());
                    gcHandle.Free();
                }

                if (createDataset) // otherwise there will be an error, if set_virtual has never been called.
                    datasetId = H5D.create(groupId, datasetName, datasetInfo.TypeId, spaceId, H5P.DEFAULT, propertyId);
            }
            finally
            {
                if (H5I.is_valid(propertyId) > 0) { H5P.close(propertyId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(spaceId) > 0) { H5S.close(spaceId); }

                if (closeType && H5I.is_valid(datasetInfo.TypeId) > 0)
                    H5T.close(datasetInfo.TypeId);
            }
        }

        #endregion

        #region VDS_META

        public static hdf_transfer_function_t PromptTransferFunctionData(hdf_transfer_function_t hdf_transfer_function)
        {
            DateTime dateTime;
            Console.CursorVisible = true;

            // date / time
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for date/time ({ hdf_transfer_function.date_time }): ");

                var optionSet = new HashSet<string>() { "0001-01-01" };

                if (!string.IsNullOrWhiteSpace(hdf_transfer_function.date_time))
                {
                    optionSet.Add(hdf_transfer_function.date_time);
                }

                (var dateTime_tmp, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    hdf_transfer_function.date_time = dateTime.ToString("yyyy-MM-ddTHH-mm-ssZ");
                    break;
                }
                else if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    hdf_transfer_function.date_time = dateTime.ToString("yyyy-MM-ddT00-00-00Z");
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_transfer_function.date_time))
                {
                    break;
                }
            }

            // type
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for type ({ hdf_transfer_function.type }): ");

                var optionSet = new HashSet<string>() { "polynomial", "function" };

                if (!string.IsNullOrWhiteSpace(hdf_transfer_function.type))
                {
                    optionSet.Add(hdf_transfer_function.type);
                }

                (var type, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(type))
                {
                    hdf_transfer_function.type = type;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_transfer_function.type))
                {
                    break;
                }
            }

            // option
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for option ({ hdf_transfer_function.option }): ");

                var optionSet = new HashSet<string>() { "permanent" };

                if (!string.IsNullOrWhiteSpace(hdf_transfer_function.option))
                {
                    optionSet.Add(hdf_transfer_function.option);
                }

                (var option, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(option))
                {
                    hdf_transfer_function.option = option;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_transfer_function.option))
                {
                    break;
                }
            }

            // argument
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for argument ({ hdf_transfer_function.argument }): ");

                var optionSet = new HashSet<string>() { };

                if (!string.IsNullOrWhiteSpace(hdf_transfer_function.argument))
                {
                    optionSet.Add(hdf_transfer_function.argument);
                }

                (var argument, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(argument))
                {
                    hdf_transfer_function.argument = argument;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_transfer_function.argument))
                {
                    break;
                }
            }

            //
            Console.CursorVisible = false;

            return hdf_transfer_function;
        }

        public static hdf_tag_t PromptTagData(hdf_tag_t hdf_tag)
        {
            DateTime dateTime;

            var tmp = string.Empty;

            Console.CursorVisible = true;

            // date / time
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for date/time ({ hdf_tag.date_time }): ");

                var optionSet = new HashSet<string>() { "0001-01-01" };

                if (!string.IsNullOrWhiteSpace(hdf_tag.date_time))
                    optionSet.Add(hdf_tag.date_time);

                (var dateTime_tmp, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    hdf_tag.date_time = dateTime.ToString("yyyy-MM-ddTHH-mm-ssZ");
                    break;
                }
                else if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    hdf_tag.date_time = dateTime.ToString("yyyy-MM-ddT00-00-00Z");
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_tag.date_time))
                {
                    break;
                }
            }

            // name
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for name ({ hdf_tag.name }): ");

                var optionSet = new HashSet<string>() { };

                if (!string.IsNullOrWhiteSpace(hdf_tag.name))
                    optionSet.Add(hdf_tag.name);

                (var name, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(name) && OneDasUtilities.CheckNamingConvention(name, out tmp))
                {
                    hdf_tag.name = name;
                    break;
                }
                else if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(hdf_tag.name))
                {
                    break;
                }
            }

            // mode
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for mode ({ hdf_tag.mode }): ");

                var optionSet = new HashSet<string>() { "none", "start", "end" };

                if (!string.IsNullOrWhiteSpace(hdf_tag.mode))
                    optionSet.Add(hdf_tag.mode);

                (var comment, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(comment))
                {
                    hdf_tag.mode = comment;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_tag.mode))
                {
                    break;
                }
            }

            // comment
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for comment ({ hdf_tag.comment }): ");

                var optionSet = new HashSet<string>() { "none" };

                if (!string.IsNullOrWhiteSpace(hdf_tag.comment))
                    optionSet.Add(hdf_tag.comment);

                (var mode, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (mode.Any())
                {
                    hdf_tag.comment = mode;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_tag.comment))
                {
                    break;
                }
            }

            //
            Console.CursorVisible = false;

            return hdf_tag;
        }

        public static string PromptDirectoryPath(string directoryPath)
        {
            Console.CursorVisible = true;

            // name
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for output directory ({ directoryPath }): ");

                var optionSet = new HashSet<string>() { };

                if (!string.IsNullOrWhiteSpace(directoryPath))
                {
                    optionSet.Add(directoryPath);
                }

                (var value, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (Directory.Exists(value))
                {
                    directoryPath = value;
                    break;
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    break;
                }
            }

            //
            Console.CursorVisible = false;

            return directoryPath;
        }

        #endregion

        #region AGGREGATION

        public static void CreateAggregatedFiles(DateTime epochStart, string method, string argument, string campaignName, Dictionary<string, string> filters)
        {
            var epochEnd = epochStart.AddMonths(1);
            var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_NATIVE", epochStart.ToString("yyyy-MM"));
            var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "DB_AGGREGATION", epochStart.ToString("yyyy-MM"));
            var logger = _loggerFactory.CreateLogger("AGGREGATE");

            logger.LogInformation($"Epoch start: {epochStart.ToString("yyyy-MM-dd")}{Environment.NewLine}Epoch   end: {epochEnd.ToString("yyyy-MM-dd")}");

            try
            {
                Program.InternalCreateAggregatedFiles(sourceDirectoryPath, targetDirectoryPath, method, argument, campaignName, filters, logger);
                logger.LogInformation($"Execution of the 'aggregate' command finished successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Execution of the 'aggregate' command failed. Error message: '{ex.ToString()}'.");
                throw;
            }
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
                    var campaignInfo = GeneralHelper.GetCampaignInfo(sourceFileId, isLazyLoading: false, campaignName);

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

        private static void ExecutePwsh(string scriptFilePath, string transactionId)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                // ensure FluentFTP lib is loaded
                var dummy = FtpExists.NoCheck;

                var logger = new VdsToolLogger(_loggerFactory.CreateLogger($"PWSH ({transactionId})"));
                logger.LogInformation($"Executing script '{scriptFilePath}'.");

                ps.Runspace.SessionStateProxy.SetVariable("dbRoot", Environment.CurrentDirectory);
                ps.Runspace.SessionStateProxy.SetVariable("logger", logger);

                try
                {
                    ps.AddScript(File.ReadAllText(scriptFilePath))
                      .Invoke();

                    logger.LogInformation($"Execution of script '{scriptFilePath}' finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution 'pwsh' command failed (path: '{scriptFilePath}'). Error message: '{ex.Message}'.");
                    throw;
                }
            }
        }

        #endregion

        #region DOC

        public static void WriteCampaignDocumentation(string campaignName, string targetDirectoryPath)
        {
            long vdsFileId = -1;
            long vdsMetaFileId = -1;

            var logger = _loggerFactory.CreateLogger("DOC");

            vdsFileId = H5F.open(Path.Combine(Environment.CurrentDirectory, "VDS.h5"), H5F.ACC_RDONLY);
            vdsMetaFileId = H5F.open(Path.Combine(Environment.CurrentDirectory, "VDS_META.h5"), H5F.ACC_RDONLY);

            try
            {
                var campaign = GeneralHelper.GetCampaignInfo(vdsFileId, isLazyLoading: false, campaignName);

                if (campaign == null)
                    throw new Exception($"The campaign named '{campaignName}' could not be found.");

                Program.InternalWriteCampaignDocumentation(campaign, targetDirectoryPath, vdsMetaFileId, logger);
                logger.LogInformation($"Execution of the 'doc' command finished successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Execution of the 'doc' command failed. Error message: '{ex.Message}'.");
                throw;
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
