using FluentFTP;
using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Extension.Hdf;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Hdf.VdsTool.Import;
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
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.VdsTool
{
    class Program
    {
        #region "Fields"

        private static uint _isLibraryThreadSafe;
        private static ILoggerFactory _loggerFactory;

        #endregion

        #region "Properties"

        public static string BaseDirectoryPath { get; set; }

        #endregion

        #region "Methods"

        private static async Task<int> Main(string[] args)
        {
            // IMPORTANT: ENSURE CURRENT PATH POINTS TO DATABASE

            // EXPERIMENTAL

            // Create a root command with some options
            var rootCommand = new RootCommand("Virtual dataset tool");

            rootCommand.AddCommand(Program.PreparePwshCommand());
            rootCommand.AddCommand(Program.PrepareUpdateCommand());
            rootCommand.AddCommand(Program.PrepareVdsCommand());

            // run
            return await rootCommand.InvokeAsync(args);

            Console.CursorVisible = false;
            Console.Title = "VdsTool";

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //// 
            //if (args.Any())
            //{
            //    if (!Program.TryGetParameterValue("database", "d", args.ToList(), ref _databaseDirectoryPath, value => Program.ValidateDatabaseDirectoryPath(value)))
            //    {
            //        return;
            //    }

            //    Environment.CurrentDirectory = Program.BaseDirectoryPath;

            //    // configure logging
            //    var serviceProvider = new ServiceCollection().AddLogging(builder =>
            //    {
            //        builder.AddConsole();
            //        builder.AddFile(Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "VdsTool-{Date}.txt"));
            //    }).BuildServiceProvider();

            //    _loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            //    if (Program.ParseCommandLineArguments(args))
            //        return;
            //}

            //H5.is_library_threadsafe(ref _isLibraryThreadSafe);

            //if (_isLibraryThreadSafe <= 0)
            //    Console.WriteLine("Warning: libary is not thread safe!");

            //while (true)
            //{
            //    Console.CursorVisible = true;

            //    while (true)
            //    {
            //        Console.Clear();
            //        Console.WriteLine("Please enter the base directory path of the HDF files:");

            //        if (string.IsNullOrWhiteSpace(Program.BaseDirectoryPath))
            //        {
            //            bool isEscaped;

            //            (Program.BaseDirectoryPath, isEscaped) = Utilities.ReadLine(new List<string>());

            //            if (isEscaped && Program.HandleEscape())
            //                return;
            //        }

            //        if (Program.ValidateDatabaseDirectoryPath(Program.BaseDirectoryPath))
            //        {
            //            Console.Title = $"VdsTool - { Program.BaseDirectoryPath }";
            //            break;
            //        }
            //        else
            //        {
            //            Program.BaseDirectoryPath = string.Empty;
            //        }
            //    }

            //    Environment.CurrentDirectory = Program.BaseDirectoryPath;

            //    Console.CursorVisible = false;

            //    new MainMenuNavigator();

            //    if (Program.HandleEscape())
            //        return;
            //}
        }

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

            command.Handler = CommandHandler.Create<string, string>((scriptPath, transactionId) =>
            {
                try
                {
                    Program.ExecutePwsh(scriptPath, transactionId);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not import custom data. Aborting action.");
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
            bool TryConvertArgument(SymbolResult a, out DateTime value)
            {
                
                //https://github.com/dotnet/command-line-api/blob/549c3abc5ca36821c5d02a33ab0b47acbc05d639/src/System.CommandLine.Tests/Binding/TypeConversionTests.cs
                value = DateTime.MinValue;
                return true;
            }


            var command = new Command("vds", "Updates the database index of files that are part of the specified epoch")
            {
                new Option("--epoch-start", "The start date of the epoch")
                {
                    Argument = new Argument<DateTime>(new TryConvertArgument),
                    Required = true
                }
            };

            command.Handler = CommandHandler.Create(() =>
            {
                try
                {
                    
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

        private static bool ValidateDatabaseDirectoryPath(string databaseDirectoryPath)
        {
            return Directory.Exists(databaseDirectoryPath) &&
                   Directory.Exists(Path.Combine(databaseDirectoryPath, "DB_AGGREGATION")) &&
                   Directory.Exists(Path.Combine(databaseDirectoryPath, "DB_IMPORT")) &&
                   Directory.Exists(Path.Combine(databaseDirectoryPath, "DB_NATIVE")) &&
                   Directory.Exists(Path.Combine(databaseDirectoryPath, "VDS"));
        }

        private static bool ParseCommandLineArguments(string[] args)
        {
            switch (args[0])
            {
                case "convert":

                    Program.HandleConvert(args.Skip(1).ToList());
                    break;

                case "aggregate":

                    Program.HandleAggregations(args.Skip(1).ToList());
                    break;

                case "--database":

                    if (args.Count() > 1 && Program.ValidateDatabaseDirectoryPath(args[1]))
                    {
                        Program.BaseDirectoryPath = args[1];
                        return false;
                    }

                    break;
            }

            return true;
        }

        private static void HandleConvert(List<string> args)
        {
            // campaignName
            string campaignName = default;

            if (!Program.TryGetParameterValue("campaign", "c", args, ref campaignName))
                return;

            // version
            string versionValue = default;
            int version = default;

            if (!Program.TryGetParameterValue("version", "v", args, ref versionValue, value => int.TryParse(value, out version)))
                return;

            // fileFormat
            string fileFormat = default;

            if (!Program.TryGetParameterValue("format", "f", args, ref fileFormat))
                return;

            // fileFormat
            string fileNameFormat = default;

            if (!Program.TryGetParameterValue("file-name-format", "n", args, ref fileNameFormat))
                return;

            // periodPerFile
            string periodPerFileValue = default;
            uint periodPerFile = default;

            if (!Program.TryGetParameterValue("period-per-file", "p", args, ref periodPerFileValue, value => uint.TryParse(value, out periodPerFile)))
                return;

            // days
            string daysValue = default;
            int days = default;

            if (!Program.TryGetParameterValue("days", "d", args, ref daysValue, value => int.TryParse(value, out days)))
                return;

            // systemName
            string systemName = default;

            if (!Program.TryGetParameterValue("system-name", "s", args, ref systemName))
                return;

            // converToDouble
            bool convertToDouble = false;

            if (args.IndexOf($"-x") >= 0 || args.IndexOf($"--convert-to-double") >= 0)
                convertToDouble = true;

            // applyCalibration
            bool applyCalibration = false;

            if (args.IndexOf($"-x") >= 0 || args.IndexOf($"--apply-calibration") >= 0)
                applyCalibration = true;

            //
            try
            {
                Program.ConvertFiles(campaignName, version, fileFormat, fileNameFormat, periodPerFile, days, systemName, convertToDouble, applyCalibration);
            }
            catch
            {
                Console.WriteLine("Could not import data. Aborting action.");
            }
        }

        private static void HandleVds(List<string> args)
        {
            // epoch start
            string dateTime = default;
            DateTime epochStart = default;

            if (!Program.TryGetParameterValue("epoch-start", "e", args, ref dateTime, value => DateTime.TryParseExact(value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out epochStart)))
                return;

            Program.CreateVirtualDatasetFile(epochStart);
        }

        private static void HandleAggregations(List<string> args)
        {
            // epoch start
            string dateTime = default;
            DateTime epochStart = default;

            if (!Program.TryGetParameterValue("epoch-start", "e", args, ref dateTime, value => DateTime.TryParseExact(value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out epochStart)))
                return;

            Program.CreateAggregatedFiles(epochStart);
        }

        #endregion

        #region "VDS"

        public static void CreateVirtualDatasetFile(DateTime epochStart)
        {
            string vdsFilePath;
            DateTime epochEnd;
            List<string> sourceDirectoryPathSet;

            sourceDirectoryPathSet = new List<string>();

            if (epochStart > DateTime.MinValue)
            {
                epochEnd = epochStart.AddMonths(1);
                sourceDirectoryPathSet.Add(Path.Combine(Program.BaseDirectoryPath, "DB_AGGREGATION", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPathSet.Add(Path.Combine(Program.BaseDirectoryPath, "DB_IMPORT", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPathSet.Add(Path.Combine(Program.BaseDirectoryPath, "DB_NATIVE", epochStart.ToString("yyyy-MM")));
                vdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS", $"{ epochStart.ToString("yyyy-MM") }.h5");
            }
            else
            {
                epochStart = new DateTime(2000, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = new DateTime(2030, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                sourceDirectoryPathSet.Add(Path.Combine(Program.BaseDirectoryPath, "VDS"));
                vdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS.h5");
            }

            if (Console.CursorTop > 0 || Console.CursorLeft > 0)
            {
                Console.WriteLine();
            }

            Console.WriteLine($"Epoch start: { epochStart.ToString("yyyy-MM-dd") }");
            Console.WriteLine($"Epoch end:   { epochEnd.ToString("yyyy-MM-dd") }");
            Console.WriteLine();

            Program.InternalCreateVirtualDatasetFile(sourceDirectoryPathSet, vdsFilePath, epochStart, epochEnd);
        }

        private static unsafe void InternalCreateVirtualDatasetFile(List<string> sourceDirectoryPathSet, string vdsFilePath, DateTime epochStart, DateTime epochEnd)
        {
            long vdsFileId = -1;

            ulong[] actualDimenionSet;
            ulong[] maximumDimensionSet;

            string lastVariablePath;
            string tempFilePath;

            List<CampaignInfo> campaignInfoSet;
            List<string> sourceFilePathSet;
            Dictionary<string, List<byte>> isChunkCompletedMap;

            //
            actualDimenionSet = new ulong[1];
            maximumDimensionSet = new ulong[1];

            lastVariablePath = String.Empty;
            tempFilePath = Path.GetTempFileName();

            campaignInfoSet = new List<CampaignInfo>();
            sourceFilePathSet = new List<string>();
            isChunkCompletedMap = new Dictionary<string, List<byte>>();

            // fill sourceFilePathSet
            sourceDirectoryPathSet.ToList().ForEach(x =>
            {
                if (Directory.Exists(x))
                {
                    sourceFilePathSet.AddRange(Directory.GetFiles(x, "*.h5", SearchOption.TopDirectoryOnly).ToList());
                }
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
                    Program.VdsSourceFile(sourceFilePath, campaignInfoSet, isChunkCompletedMap);
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
                {
                    File.Delete(vdsFilePath);
                }

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

        private static void VdsSourceFile(string sourceFilePath, List<CampaignInfo> campaignInfoSet, Dictionary<string, List<byte>> isChunkCompletedMap)
        {
            long sourceFileId = -1;

            Console.Write($"Processing file { Path.GetFileName(sourceFilePath) } ... ");

            sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);

            try
            {
                if (sourceFileId < 0)
                {
                    throw new Exception(ErrorMessage.Program_CouldNotOpenFile);
                }

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

                Console.WriteLine("Done.");
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

                    var relativeFilePath = $".{sourceFileInfo.FilePath.Remove(0, Program.BaseDirectoryPath.TrimEnd('\\').TrimEnd('/').Length)}";

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

        #region "VDS_META"

        public static hdf_aggregate_function_t PromptAggregateFunctionData(hdf_aggregate_function_t hdf_aggregate_function)
        {
            var tmp = string.Empty;

            Console.CursorVisible = true;

            // type
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for type ({ hdf_aggregate_function.type }): ");

                var optionSet = new HashSet<string>() { "mean", "min", "max", "std", "min_bitwise", "max_bitwise" };

                if (!string.IsNullOrWhiteSpace(hdf_aggregate_function.type))
                    optionSet.Add(hdf_aggregate_function.type);

                (var type, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(type) && OneDasUtilities.CheckNamingConvention(type, out tmp))
                {
                    hdf_aggregate_function.type = type;
                    break;
                }
                else if (string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(hdf_aggregate_function.type))
                {
                    break;
                }
            }

            // argument
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for argument ({ hdf_aggregate_function.argument }): ");

                var optionSet = new HashSet<string>() { "none" };

                if (!string.IsNullOrWhiteSpace(hdf_aggregate_function.argument))
                    optionSet.Add(hdf_aggregate_function.argument);

                (var argument, var isEscaped) = Utilities.ReadLine(optionSet.ToList());

                if (!string.IsNullOrWhiteSpace(argument))
                {
                    hdf_aggregate_function.argument = argument;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hdf_aggregate_function.argument))
                {
                    break;
                }
            }

            //
            Console.CursorVisible = false;

            return hdf_aggregate_function;
        }

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

        #region "AGGREGATION"

        public static void CreateAggregatedFiles(DateTime epochStart)
        {
            var vdsMetaFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS_META.h5");

            if (!File.Exists(vdsMetaFilePath))
                return;

            var epochEnd = epochStart.AddMonths(1);
            var sourceDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "DB_NATIVE", epochStart.ToString("yyyy-MM"));
            var targetDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "DB_AGGREGATION", epochStart.ToString("yyyy-MM"));
            var logDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "HDF VdsTool");

            Directory.CreateDirectory(logDirectoryPath);

            if (Console.CursorTop > 0 || Console.CursorLeft > 0)
                Console.WriteLine();

            Console.WriteLine($"Epoch start: { epochStart.ToString("yyyy-MM-dd") }");
            Console.WriteLine($"Epoch end:   { epochEnd.ToString("yyyy-MM-dd") }");
            Console.WriteLine();

            Program.InternalCreateAggregatedFiles(sourceDirectoryPath, targetDirectoryPath, logDirectoryPath, vdsMetaFilePath, epochStart, epochEnd);
        }

        private static void InternalCreateAggregatedFiles(string sourceDirectoryPath, string targetDirectoryPath, string logDirectoryPath, string vdsMetaFilePath, DateTime epochStart, DateTime epochEnd)
        {
            long vdsMetaFileId = -1;
            long sourceFileId = -1;
            long targetFileId = -1;

            IList<string> filePathSet;

            var campaignInfoSet = new List<CampaignInfo>();

            if (Directory.Exists(sourceDirectoryPath))
                filePathSet = Directory.GetFiles(sourceDirectoryPath);
            else
                filePathSet = new List<string>();

            vdsMetaFileId = H5F.open(vdsMetaFilePath, H5F.ACC_RDONLY);

            try
            {
                foreach (string sourceFilePath in filePathSet)
                {
                    targetFileId = -1;
                    var logFilePath = Path.Combine(logDirectoryPath, $"aggregations_{ Path.GetFileNameWithoutExtension(sourceFilePath) }.txt");

                    using (StreamWriter messageLog = new StreamWriter(new FileStream(logFilePath, FileMode.Append)))
                    {
                        Console.WriteLine($"file: { sourceFilePath }");
                        Console.WriteLine();

                        // sourceFileId
                        sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);
                        campaignInfoSet = GeneralHelper.GetCampaignInfoSet(sourceFileId, false);

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
                            foreach (var campaignInfo in campaignInfoSet)
                            {
                                Program.AggregateCampaign(sourceFileId, targetFileId, vdsMetaFileId, campaignInfo, messageLog);
                            }

                            Console.CursorTop -= 1;
                        }
                        finally
                        {
                            if (H5I.is_valid(targetFileId) > 0) { H5F.close(targetFileId); }
                            if (H5I.is_valid(sourceFileId) > 0) { H5F.close(sourceFileId); }
                        }
                    }
                }
            }
            finally
            {
                if (H5I.is_valid(vdsMetaFileId) > 0) { H5F.close(vdsMetaFileId); }
            }
        }

        private static void AggregateCampaign(long sourceFileId, long targetFileId, long vdsMetaFileId, CampaignInfo campaignInfo, StreamWriter messageLog)
        {
            var index = 0;
            var datasetPath = $"{ campaignInfo.GetPath() }/is_chunk_completed_set";
            (var groupId, var isNew) = IOHelper.OpenOrCreateGroup(targetFileId, campaignInfo.GetPath());

            try
            {
                if (isNew || !IOHelper.CheckLinkExists(targetFileId, datasetPath))
                    H5O.copy(sourceFileId, datasetPath, targetFileId, datasetPath);

                foreach (var variableInfo in campaignInfo.VariableInfoSet)
                {
                    index++;

                    Console.WriteLine($"{ variableInfo.VariableNameSet.Last() } ({ index }/{ campaignInfo.VariableInfoSet.Count() })");
                    Program.AggregateVariable(sourceFileId, targetFileId, vdsMetaFileId, variableInfo, messageLog);
                    Console.CursorTop -= 1;
                    Program.ClearCurrentLine();
                }
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
            }
        }

        private static void AggregateVariable(long sourceFileId, long targetFileId, long vdsMetaFileId, VariableInfo variableInfo, StreamWriter messageLog)
        {
            long vdsMetaGroupId = -1;
            long sourceDatasetId = -1;
            long sourceDatasetId_status = -1;
            long typeId = -1;

            var sampleRate = (SampleRate)0;
            var groupPath = variableInfo.GetPath();

            // if meta data entry exists
            if (IOHelper.CheckLinkExists(vdsMetaFileId, groupPath))
            {
                vdsMetaGroupId = H5G.open(vdsMetaFileId, groupPath);

                try
                {
                    if (H5A.exists(vdsMetaGroupId, "aggregate_function_set") > 0)
                    {
                        var aggregate_function_set = IOHelper.ReadAttribute<hdf_aggregate_function_t>(vdsMetaGroupId, "aggregate_function_set");

                        // find proper data source
                        foreach (SampleRate testedSampleRate in Enum.GetValues(typeof(SampleRate)))
                        {
#warning remove magic number
                            var sourceDatasetName = $"{ 100 / (int)testedSampleRate } Hz";

                            if (variableInfo.DatasetInfoSet.Where(x => x.Name == sourceDatasetName).Any())
                            {
                                sampleRate = testedSampleRate;
                                sourceDatasetId = H5D.open(sourceFileId, $"{ groupPath }/{ sourceDatasetName }");
                                sourceDatasetId_status = H5D.open(sourceFileId, $"{ groupPath }/{sourceDatasetName }_status");
                                typeId = H5D.get_type(sourceDatasetId);

                                break;
                            }
                        }

                        if (sampleRate == default)
                        {
                            messageLog.WriteLine($"No appropriate data source was found. Skipping variable.");
                            return;
                        }

                        typeId = H5D.get_type(sourceDatasetId);

                        // invoke Program.DoAggregationStuff
                        GeneralHelper.InvokeGenericMethod(typeof(Program), null, nameof(Program.AggregateDataset),
                                                      BindingFlags.NonPublic | BindingFlags.Static,
                                                      TypeConversionHelper.GetTypeFromHdfTypeId(typeId),
                                                      new object[] { groupPath, sourceDatasetId, sourceDatasetId_status, targetFileId, sampleRate, aggregate_function_set, messageLog });

                        // clean up
                        H5D.close(sourceDatasetId_status);
                        H5D.close(sourceDatasetId);
                        H5T.close(typeId);
                    }
                    else
                    {
                        messageLog.WriteLine($"The attribute 'aggregate_function_set' of variable { groupPath } does not exist in file VDS_META.h5. Skipping variable.");
                    }
                }
                finally
                {
                    if (H5I.is_valid(vdsMetaGroupId) > 0) { H5G.close(vdsMetaGroupId); }
                }
            }
            else
            {
                messageLog.WriteLine($"The variable { groupPath } does not exist in file VDS_META.h5. Skipping variable.");
            }
        }

        private static void AggregateDataset<T>(string groupPath, long sourceDatasetId, long sourceDatasetId_status, long targetFileId, SampleRate sampleRate, hdf_aggregate_function_t[] aggregate_function_set, StreamWriter messageLog)
        {
            long targetDatasetId = -1;
            double[] targetValueSet;
            double[] sourceValueSet_double = null;
            byte[] sourceValueSet_status = null;
            T[] sourceValueSet = null;

            // for each aggregation_function
            foreach (hdf_aggregate_function_t aggregation_function in aggregate_function_set)
            {
                Console.WriteLine($"\t{ aggregation_function.type }");

                // for each period
                foreach (Period period in Enum.GetValues(typeof(Period)))
                {
                    var targetDatasetPath = $"{ groupPath }/{ (int)period} s_{aggregation_function.type }";

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

                        switch (aggregation_function.type)
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

                                targetValueSet = Program.ApplyAggregationFunction(aggregation_function, chunkCount, sourceValueSet_double, messageLog);

                                break;

                            case "min_bitwise":
                            case "max_bitwise":

                                targetValueSet = Program.ApplyAggregationFunction(aggregation_function, chunkCount, sourceValueSet, sourceValueSet_status, messageLog);

                                break;

                            default:

                                messageLog.WriteLine($"The type { aggregation_function.type } of attribute 'aggregate_function_set' is unknown. Skipping period { period }.");

                                continue;
                        }

                        targetDatasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, (ulong)chunkCount, 1).DatasetId;

                        try
                        {
                            IOHelper.Write(targetDatasetId, targetValueSet.ToArray(), DataContainerType.Dataset);
                            IOHelper.PrepareAttribute(targetDatasetId, "aggregate_function_set", new hdf_aggregate_function_t[] { aggregation_function }, new ulong[] { 1 }, true);

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
        }

        private static double[] ApplyAggregationFunction(hdf_aggregate_function_t aggregation_function, int targetDatasetLength, double[] valueSet, StreamWriter messageLog)
        {
            var chunkSize = valueSet.Count() / targetDatasetLength;
            var result = new double[targetDatasetLength];

            switch (aggregation_function.type)
            {
                case "mean":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        double[] chunkValueSet;
                        int baseIndex;

                        baseIndex = (int)(x * chunkSize);
                        chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Mean(chunkValueSet);
                    });

                    break;

                case "mean_polar":

                    double[] sin = new double[targetDatasetLength];
                    double[] cos = new double[targetDatasetLength];
                    double factor;
                    double limit;

                    if (aggregation_function.argument.Contains("*PI"))
                    {
                        limit = Double.Parse(aggregation_function.argument.Replace("*PI", "")) * Math.PI;
                    }
                    else
                    {
                        limit = Double.Parse(aggregation_function.argument);
                    }

                    factor = 2 * Math.PI / limit;

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        int baseIndex = (int)(x * chunkSize);

                        for (int i = 0; i < chunkSize; i++)
                        {
                            sin[x] += Math.Sin(valueSet[baseIndex + i] * factor);
                            cos[x] += Math.Cos(valueSet[baseIndex + i] * factor);
                        }

                        result[x] = Math.Atan2(sin[x], cos[x]) / factor;

                        if (result[x] < 0)
                        {
                            result[x] += limit;
                        }
                    });

                    break;

                case "min":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        double[] chunkValueSet;
                        int baseIndex;

                        baseIndex = (int)(x * chunkSize);
                        chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Minimum(chunkValueSet);
                    });

                    break;

                case "max":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        double[] chunkValueSet;
                        int baseIndex;

                        baseIndex = (int)(x * chunkSize);
                        chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.Maximum(chunkValueSet);
                    });

                    break;

                case "std":

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        double[] chunkValueSet;
                        int baseIndex;

                        baseIndex = (int)(x * chunkSize);
                        chunkValueSet = new double[chunkSize];

                        Array.Copy(valueSet, baseIndex, chunkValueSet, 0, chunkSize);

                        result[x] = ArrayStatistics.StandardDeviation(chunkValueSet);
                    });

                    break;

                default:

                    messageLog.WriteLine($"The type {aggregation_function.type} of attribute 'aggregate_function_set' is unknown. Skipping period.");

                    break;

            }

            return result;
        }

        private static double[] ApplyAggregationFunction<T>(hdf_aggregate_function_t aggregation_function, int targetDatasetLength, T[] valueSet, byte[] valueSet_status, StreamWriter messageLog)
        {
            var chunkSize = valueSet.Count() / targetDatasetLength;
            var result = new double[targetDatasetLength];

            switch (aggregation_function.type)
            {
                case "min_bitwise":

                    T[] bitField_and = new T[targetDatasetLength];

                    Parallel.For(0, targetDatasetLength, x =>
                    {
                        int baseIndex = (int)(x * chunkSize);

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
                        int baseIndex = (int)(x * chunkSize);

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
                    messageLog.WriteLine($"The type {aggregation_function.type} of attribute 'aggregate_function_set' is unknown. Skipping period.");
                    break;

            }

            return result;
        }

        #endregion

        #region "PWSH"

        private static void ExecutePwsh(string scriptFilePath, string transactionId)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                // ensure FluentFTP lib is loaded
                var ftpClient = new FtpClient();

                var logger = new VdsToolLogger(_loggerFactory.CreateLogger(transactionId));
                logger.LogInformation($"Executing script '{scriptFilePath}'.");

                ps.Runspace.SessionStateProxy.SetVariable("dbRoot", Program.BaseDirectoryPath);
                ps.Runspace.SessionStateProxy.SetVariable("logger", logger);

                try
                {
                    ps.AddScript(File.ReadAllText(scriptFilePath))
                      .Invoke();

                    logger.LogInformation($"Execution of script '{scriptFilePath}' finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of script '{scriptFilePath}' failed. Error message: '{ex.Message}'.");
                    throw;
                }
            }
        }

        #endregion

        #region "CONVERT"

        private static void ConvertFiles(string campaignName, int version, string fileFormat, string fileNameFormat, uint periodPerFile, int days, string systemName, bool convertToDouble, bool applyCalibration)
        {
            var sourceDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "DB_ORIGIN", $"{campaignName.Replace('/', '_')}_V{version}");
            var dateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            var dateTimeBegin = dateTimeEnd.AddDays(-days);

            // logger
            var logDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "VdsTool");
            var logFilePath = Path.Combine(logDirectoryPath, $"{ sourceDirectoryPath.Split('\\').Last().Split('/').Last() }.txt");

            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            File.AppendAllText(logFilePath, $"BEGIN from { dateTimeBegin.ToString("yyyy-MM-dd") } to { dateTimeEnd.ToString("yyyy-MM-dd") }{ Environment.NewLine }");

            // convert
            for (int i = 0; i <= days; i++)
            {
                dateTimeBegin = dateTimeBegin.AddDays(1);

                var currentSourceDirectoryPath = Path.Combine(sourceDirectoryPath, dateTimeBegin.ToString("yyyy-MM"));
                var currentTargetDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "DB_IMPORT", dateTimeBegin.ToString("yyyy-MM"));
                var currentTargetFileName = $"{campaignName.Replace('/', '_')}_V{version}_{dateTimeBegin.ToString("yyyy-MM-ddTHH-mm-ssZ")}.h5";
                var currentTargetFilePath = Path.Combine(currentTargetDirectoryPath, currentTargetFileName);

                if (File.Exists(currentTargetFilePath))
                {
                    Console.WriteLine($"File '{currentTargetFilePath}' already exists.");
#warning Add proper logging here.
                }
                else if (!Directory.Exists(currentSourceDirectoryPath) || !Directory.EnumerateFileSystemEntries(currentSourceDirectoryPath).Any())
                {
                    // skip
                }
                else
                {
                    Directory.CreateDirectory(currentTargetDirectoryPath);

                    Program.InternalConvertFiles(currentSourceDirectoryPath, currentTargetDirectoryPath,
                                                dateTimeBegin, campaignName, version, fileFormat,
                                                fileNameFormat, TimeSpan.FromSeconds(periodPerFile), 
                                                systemName, convertToDouble, applyCalibration);
                }
            }

            File.AppendAllText(logFilePath, $"END{ Environment.NewLine }{ Environment.NewLine }");
        }

        private static void InternalConvertFiles(string sourceDirectoryPath, string targetDirectoryPath, DateTime dateTimeBegin, string campaignName, int version, string fileFormat, string fileNameFormat, TimeSpan periodPerFile, string systemName, bool convertToDouble, bool applyCalibration)
        {
            // data writer context
            IDataReader dataReader;
            List<VariableDescription> variableDescriptionSet;

            if (fileFormat == "FamosImc2")
            {
                var firstFilePath = Directory.EnumerateFiles(sourceDirectoryPath).FirstOrDefault();

                dataReader = new FamosImc2DataReader();
                variableDescriptionSet = dataReader.GetVariableDescriptions(firstFilePath);
            }
            else
            {
                return;
            }

            var importContext = ImportContext.OpenOrCreate(Path.Combine(sourceDirectoryPath, "..", ".."), campaignName, variableDescriptionSet);

            foreach (var variableDescription in variableDescriptionSet)
            {
                variableDescription.Guid = importContext.VariableToGuidMap[variableDescription.VariableName];

                if (convertToDouble)
                {
                    variableDescription.DataType = OneDasDataType.FLOAT64;
                    variableDescription.DataStorageType = DataStorageType.Simple;
                }
                else
                {
                    variableDescription.DataStorageType = DataStorageType.Extended;
                }
            }

            var campaignNameParts = campaignName.Split('/');
            var campaignDescription = new OneDasCampaignDescription(importContext.CampaignGuid, version, campaignNameParts[0], campaignNameParts[1], campaignNameParts[2]);
            var customMetadataEntrySet = new List<CustomMetadataEntry>();
            var dataWriterContext = new DataWriterContext(systemName, targetDirectoryPath, campaignDescription, customMetadataEntrySet);

            // configure data writer
            var settings = new HdfSettings() { FileGranularity = FileGranularity.Day };
            using var dataWriter = new HdfWriter(settings, NullLogger.Instance);

            dataWriter.Configure(dataWriterContext, variableDescriptionSet);

            // convert data
            var currentOffset = TimeSpan.Zero;

            while (currentOffset < TimeSpan.FromDays(1))
            {
                var currentDateTimeBegin = dateTimeBegin + currentOffset;
                var fileName = currentDateTimeBegin.ToString(fileNameFormat);
                var filePath = Path.Combine(sourceDirectoryPath, fileName);

                currentOffset += periodPerFile;

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File '{filePath}' does not exist.");
#warning Write Log entry.
                    continue;
                }

                try
                {
                    Console.Write($"Processing file '{filePath}' ... ");

                    var dataStorageSet = dataReader.GetData(filePath, variableDescriptionSet, convertToDouble, applyCalibration);

                    // check actual file size
                    for (int i = 0; i < variableDescriptionSet.Count; i++)
                    {
                        var elementCount = dataStorageSet[i].DataBuffer.Length / dataStorageSet[i].ElementSize;
                        var period = (double)elementCount / (variableDescriptionSet[i].SampleRate.SamplesPerSecond);

                        if (TimeSpan.FromSeconds(period) != periodPerFile)
                            throw new Exception("The file is not complete.");
                    }
                    
                    dataWriter.Write(currentDateTimeBegin, periodPerFile, dataStorageSet);

                    foreach (DataStorageBase dataStorage in dataStorageSet)
                    {
                        dataStorage.Dispose();
                    }

                    Console.WriteLine($"Done.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed.");
                    // skip this file
                }
            }
        }

        #endregion

        private static void ClearCurrentLine()
        {
            Console.Write($"\r{ new string(' ', Console.WindowWidth - 1) }\r");
        }
    }
}
