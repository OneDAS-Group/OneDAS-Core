using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Hdf.VdsTool.Navigation;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        #endregion

        #region "Properties"

        public static string BaseDirectoryPath { get; private set; }

        #endregion

        #region "Methods"

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            if (args.Any())
            {
                if (Program.ParseCommandLineArguments(args))
                {
                    return;
                };
            }

            H5.is_library_threadsafe(ref _isLibraryThreadSafe);

            if (_isLibraryThreadSafe <= 0)
            {
                Console.WriteLine("Warning: libary is not thread safe!");
            }

            while (true)
            {
                Console.CursorVisible = true;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Please enter the base directory path of the HDF files:");

                    if (string.IsNullOrWhiteSpace(Program.BaseDirectoryPath))
                    {
                        Program.BaseDirectoryPath = Console.ReadLine();
                    }

                    if (Program.ValidateDatabaseDirectoryPath(Program.BaseDirectoryPath))
                    {
                        Program.BaseDirectoryPath += @"\";

                        break;
                    }
                    else
                    {
                        Program.BaseDirectoryPath = string.Empty;
                    }
                }

                Console.CursorVisible = false;

                new MainMenuNavigator();
            }
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
                case "import":

                    Program.HandleImport(args.Skip(1).ToList());
                    break;

                case "update":

                    Program.HandleUpdate(args.Skip(1).ToList());
                    break;

                case "vds":

                    Program.HandleVds(args.Skip(1).ToList());
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

        private static void HandleImport(List<string> args)
        {
            int index;

            // databaseDirectoryPath
            string databaseDirectoryPath;

            index = -1;
            databaseDirectoryPath = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-d"); }
            if (index < 0) { index = args.IndexOf("--database"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1 && Program.ValidateDatabaseDirectoryPath(args[1]))
                {
                    databaseDirectoryPath = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            // sourceDirectoryPath
            string sourceDirectoryPath;

            index = -1;
            sourceDirectoryPath = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-s"); }
            if (index < 0) { index = args.IndexOf("--source"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1 && Directory.Exists(sourceDirectoryPath))
                {
                    sourceDirectoryPath = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            // campaignName
            string campaignName;

            index = -1;
            campaignName = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-c"); }
            if (index < 0) { index = args.IndexOf("--campaign"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1)
                {
                    campaignName = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            // dataWriterId
            string dataWriterId;

            index = -1;
            dataWriterId = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-w"); }
            if (index < 0) { index = args.IndexOf("--data-writer"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1)
                {
                    dataWriterId = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            Program.ImportFiles(databaseDirectoryPath, sourceDirectoryPath, campaignName, dataWriterId);
        }

        private static void HandleUpdate(List<string> args)
        {
            int index;

            // databaseDirectoryPath
            string databaseDirectoryPath;

            index = -1;
            databaseDirectoryPath = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-d"); }
            if (index < 0) { index = args.IndexOf("--database"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1 && Program.ValidateDatabaseDirectoryPath(args[1]))
                {
                    databaseDirectoryPath = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            // epoch start
            DateTime date;
            DateTime epochStart;

            date = DateTime.UtcNow.Date;

            if (date.Day == 1)
            {
                epochStart = new DateTime(date.Year, date.Month, 1);
                epochStart = epochStart.AddMonths(-1);
                Program.CreateVirtualDatasetFile(databaseDirectoryPath, epochStart);
                Program.CreateAggregatedFiles(databaseDirectoryPath, epochStart);
            }

            epochStart = new DateTime(date.Year, date.Month, 1);
            Program.CreateVirtualDatasetFile(databaseDirectoryPath, epochStart);
            Program.CreateAggregatedFiles(databaseDirectoryPath, epochStart);

            epochStart = DateTime.MinValue;
            Program.CreateVirtualDatasetFile(databaseDirectoryPath, epochStart);
        }

        private static void HandleVds(List<string> args)
        {
            int index;

            // databaseDirectoryPath
            string databaseDirectoryPath;

            index = -1;
            databaseDirectoryPath = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-d"); }
            if (index < 0) { index = args.IndexOf("--database"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1 && Program.ValidateDatabaseDirectoryPath(args[1]))
                {
                    databaseDirectoryPath = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            // epoch start
            string dateTime;
            DateTime epochStart;

            index = -1;
            epochStart = default;
            dateTime = string.Empty;

            if (index < 0) { index = args.IndexOf("-e"); }
            if (index < 0) { index = args.IndexOf("--epoch-start"); }

            if (index >= 0)
            {
                if (index + 1 < args.Count)
                {
                    dateTime = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(dateTime) || DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out epochStart))
            {
                Program.CreateVirtualDatasetFile(databaseDirectoryPath, epochStart);
            }
        }

        private static void HandleAggregations(List<string> args)
        {
            int index;

            // databaseDirectoryPath
            string databaseDirectoryPath;

            index = -1;
            databaseDirectoryPath = Directory.GetCurrentDirectory();

            if (index < 0) { index = args.IndexOf("-d"); }
            if (index < 0) { index = args.IndexOf("--database"); }

            if (index >= 0)
            {
                if (args.Count() > index + 1 && Program.ValidateDatabaseDirectoryPath(args[1]))
                {
                    databaseDirectoryPath = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            // epoch start
            string dateTime;
            DateTime epochStart;

            index = -1;
            epochStart = default;
            dateTime = string.Empty;

            if (index < 0) { index = args.IndexOf("-e"); }
            if (index < 0) { index = args.IndexOf("--epoch-start"); }

            if (index >= 0)
            {
                if (index + 1 < args.Count)
                {
                    dateTime = args[index + 1];
                }
                else
                {
                    return;
                }
            }

            if (DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out epochStart))
            {
                Program.CreateAggregatedFiles(databaseDirectoryPath, epochStart);
            }
        }

        #endregion

        #region "VDS"

        public static void CreateVirtualDatasetFile(string databaseDirectoryPath, DateTime epochStart)
        {
            string vdsFilePath;
            DateTime epochEnd;
            List<string> sourceDirectoryPathSet;

            sourceDirectoryPathSet = new List<string>();

            if (epochStart > DateTime.MinValue)
            {
                epochEnd = epochStart.AddMonths(1);
                sourceDirectoryPathSet.Add(Path.Combine(databaseDirectoryPath, "DB_AGGREGATION", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPathSet.Add(Path.Combine(databaseDirectoryPath, "DB_IMPORT", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPathSet.Add(Path.Combine(databaseDirectoryPath, "DB_NATIVE", epochStart.ToString("yyyy-MM")));
                vdsFilePath = Path.Combine(databaseDirectoryPath, "VDS", $"{epochStart.ToString("yyyy-MM")}.h5");
            }
            else
            {
                epochStart = new DateTime(2000, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = new DateTime(2030, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                sourceDirectoryPathSet.Add(Path.Combine(databaseDirectoryPath, "VDS"));
                vdsFilePath = Path.Combine(databaseDirectoryPath, "VDS.h5");
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
            long sourceFileId = -1;
            long vdsFileId = -1;
            long vdsCampaignGroupId = -1;
            long vdsVariableGroupId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            string lastVariablePath = String.Empty;

            List<CampaignInfo> campaignInfoSet = new List<CampaignInfo>();
            List<string> sourceFilePathSet = new List<string>();
            Dictionary<string, List<byte>> isChunkCompletedMap;

            isChunkCompletedMap = new Dictionary<string, List<byte>>();

            // fill sourceFilePathSet
            sourceDirectoryPathSet.ToList().ForEach(x =>
            {
                if (Directory.Exists(x))
                {
                    sourceFilePathSet.AddRange(Directory.GetFiles(x, "*.h5", SearchOption.TopDirectoryOnly).ToList());
                }
            });

            // open VDS file
            vdsFileId = H5F.create(vdsFilePath, H5F.ACC_TRUNC);

            try
            {
                // write attribute
                IOHelper.PrepareAttribute(vdsFileId, "date_time", new string[] { $"{ epochStart.ToString("yyyy-MM-ddTHH-mm-ss") }Z" }, new ulong[] { 1 }, true);

                // for each source file
                foreach (string sourceFilePath in sourceFilePathSet)
                {
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

                // campaign
                foreach (var campaignInfo in campaignInfoSet)
                {
                    Console.WriteLine($"\n{ campaignInfo.Name }");

                    vdsCampaignGroupId = IOHelper.OpenOrCreateGroup(vdsFileId, campaignInfo.GetPath()).GroupId;

                    try
                    {
                        // variable
                        foreach (var variableInfo in campaignInfo.VariableInfoSet)
                        {
                            Console.WriteLine($"\t{ variableInfo.Name }");

                            vdsVariableGroupId = IOHelper.OpenOrCreateGroup(vdsCampaignGroupId, variableInfo.Name).GroupId;

                            try
                            {
                                //// make hard links for each display name
                                ////foreach (string variableName in variableInfo.Value.VariableNameSet)
                                ////{
                                ////    H5L.copy(vdsGroupIdSet[vdsGroupIdSet.Count() - 1], variableInfo.Key, vdsGroupIdSet[vdsGroupIdSet.Count() - 2], variableName);
                                ////}

                                IOHelper.PrepareAttribute(vdsVariableGroupId, "name_set", variableInfo.VariableNameSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                                IOHelper.PrepareAttribute(vdsVariableGroupId, "group_set", variableInfo.VariableGroupSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                                IOHelper.PrepareAttribute(vdsVariableGroupId, "unit_set", variableInfo.UnitSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                                IOHelper.PrepareAttribute(vdsVariableGroupId, "transfer_function_set", variableInfo.TransferFunctionSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);

                                // dataset
                                foreach (var datasetInfo in variableInfo.DatasetInfoSet)
                                {
                                    Console.WriteLine($"\t\t{ datasetInfo.Name }");
                                    Program.MergeDatasets(vdsVariableGroupId, epochStart, epochEnd, datasetInfo, campaignInfo.GetPath(), isChunkCompletedMap, true);
                                }

                                // flush data - necessary to avoid AccessViolationException at H5F.close()
                                H5F.flush(vdsFileId, H5F.scope_t.LOCAL);
                            }
                            finally
                            {
                                if (H5I.is_valid(vdsVariableGroupId) > 0) { H5G.close(vdsVariableGroupId); }
                            }
                        }

                        // don't forget is_chunk_completed_set
                        Program.MergeDatasets(vdsCampaignGroupId, epochStart, epochEnd, campaignInfo.ChunkDatasetInfo, campaignInfo.GetPath(), isChunkCompletedMap, false);
                    }
                    finally
                    {
                        if (H5I.is_valid(vdsCampaignGroupId) > 0) { H5G.close(vdsCampaignGroupId); }
                    }
                }
            }
            finally
            {
                if (H5I.is_valid(vdsFileId) > 0) { H5F.close(vdsFileId); }
            }
        }

        private static void MergeDatasets(long groupId, DateTime epochStart, DateTime epochEnd, DatasetInfo datasetInfo, string campaignPath, Dictionary<string, List<byte>> isChunkCompletedMap, bool closeType)
        {
            string datasetName;

            ulong samplesPerDay;
            ulong vdsLength;

            long datasetId = -1;
            long spaceId = -1;
            long propertyId = -1;

            GCHandle gcHandle;

            try
            {
                datasetName = datasetInfo.Name;
                samplesPerDay = OneDasUtilities.GetSamplesPerDayFromString(datasetName);
                vdsLength = (ulong)(epochEnd - epochStart).Days * samplesPerDay;
                spaceId = H5S.create_simple(1, new ulong[] { vdsLength }, new ulong[] { H5S.UNLIMITED });
                propertyId = H5P.create(H5P.DATASET_CREATE);

                foreach (SourceFileInfo sourceFileInfo in datasetInfo.SourceFileInfoSet)
                {
                    ulong offset;
                    ulong start;
                    ulong stride;
                    ulong count;
                    ulong block;

                    int chunkCount;
                    int firstIndex;
                    int lastIndex;

                    long sourceSpaceId = -1;

                    string key;

                    sourceSpaceId = H5S.create_simple(1, new ulong[] { sourceFileInfo.Length }, new ulong[] { sourceFileInfo.Length });

                    key = $"{ sourceFileInfo.FilePath }+{ campaignPath }";
                    chunkCount = isChunkCompletedMap[key].Count;
                    firstIndex = isChunkCompletedMap[key].FindIndex(value => value > 0);
                    lastIndex = isChunkCompletedMap[key].FindLastIndex(value => value > 0);

                    offset = (ulong)((sourceFileInfo.StartDateTime - epochStart).TotalDays * samplesPerDay);
                    start = (ulong)((double)sourceFileInfo.Length * firstIndex / chunkCount);
                    stride = 1;
                    count = 1;
                    block = (ulong)((double)sourceFileInfo.Length * (lastIndex - firstIndex + 1) / chunkCount);

                    if (firstIndex >= 0)
                    {
                        try
                        {
                            if (offset + start + block > vdsLength)
                            {
                                throw new Exception($"start + block = { offset + start + block } > { vdsLength }");
                            }

                            H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { offset + start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                            H5S.select_hyperslab(sourceSpaceId, H5S.seloper_t.SET, new ulong[] { start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                            H5P.set_virtual(propertyId, spaceId, sourceFileInfo.FilePath, datasetInfo.GetPath(), sourceSpaceId);
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
                    gcHandle = GCHandle.Alloc(Double.NaN, GCHandleType.Pinned);
                    H5P.set_fill_value(propertyId, datasetInfo.TypeId, gcHandle.AddrOfPinnedObject());
                    gcHandle.Free();
                }

                datasetId = H5D.create(groupId, datasetName, datasetInfo.TypeId, spaceId, H5P.DEFAULT, propertyId);
            }
            finally
            {
                if (H5I.is_valid(propertyId) > 0) { H5P.close(propertyId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(spaceId) > 0) { H5S.close(spaceId); }

                if (closeType && H5I.is_valid(datasetInfo.TypeId) > 0)
                {
                    H5T.close(datasetInfo.TypeId);
                }
            }
        }

        #endregion

        #region "VDS_META"

        public static hdf_aggregate_function_t PromptAggregateFunctionData(hdf_aggregate_function_t hdf_aggregate_function)
        {
            string type;
            string argument;
            string tmp;
            bool isEscaped;
            List<string> optionSet;

            // 
            tmp = string.Empty;

            Console.CursorVisible = true;

            // type
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for type ({ hdf_aggregate_function.type }): ");

                optionSet = new List<string>() { "mean", "min", "max", "std", "min_bitwise", "max_bitwise" };
                (type, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { "none" };
                (argument, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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
            string dateTime_tmp;
            string type;
            string argument;
            string option;
            bool isEscaped;
            List<string> optionSet;

            //
            Console.CursorVisible = true;

            // date / time
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for date/time ({ hdf_transfer_function.date_time }): ");

                optionSet = new List<string>() { "0001-01-01" };
                (dateTime_tmp, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { "polynomial", "function" };
                (type, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { "permanent" };
                (option, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { };
                (argument, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

            string dateTime_tmp;
            string name;
            string mode;
            string comment;
            string tmp = string.Empty;
            bool isEscaped;
            List<string> optionSet;

            //
            Console.CursorVisible = true;

            // date / time
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for date/time ({ hdf_tag.date_time }): ");

                optionSet = new List<string>() { "0001-01-01" };
                (dateTime_tmp, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { };
                (name, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { "none", "start", "end" };
                (comment, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

                optionSet = new List<string>() { "none" };
                (mode, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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
            string value;
            bool isEscaped;
            List<string> optionSet;

            //
            Console.CursorVisible = true;

            // name
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for output directory ({ directoryPath }): ");

                optionSet = new List<string>() { };
                (value, isEscaped) = VdsToolUtilities.ReadLine(optionSet);

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

        public static void CreateAggregatedFiles(string databaseDirectoryPath, DateTime epochStart)
        {
            string sourceDirectoryPath;
            string targetDirectoryPath;
            string logDirectoryPath;
            string vdsMetaFilePath;

            DateTime epochEnd;

            vdsMetaFilePath = Path.Combine(databaseDirectoryPath, "VDS_META.h5");

            if (!File.Exists(vdsMetaFilePath))
            {
                return;
            }

            epochEnd = epochStart.AddMonths(1);
            sourceDirectoryPath = Path.Combine(databaseDirectoryPath, "DB_NATIVE", epochStart.ToString("yyyy-MM"));
            targetDirectoryPath = Path.Combine(databaseDirectoryPath, "DB_AGGREGATION", epochStart.ToString("yyyy-MM"));
            logDirectoryPath = Path.Combine(databaseDirectoryPath, "SUPPORT", "LOGS", "HDF VdsTool");

            Directory.CreateDirectory(logDirectoryPath);

            if (Console.CursorTop > 0 || Console.CursorLeft > 0)
            {
                Console.WriteLine();
            }

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

            string targetFilePath;
            string logFilePath;

            IList<string> dateTime;
            IList<string> filePathSet;

            List<CampaignInfo> campaignInfoSet;

            campaignInfoSet = new List<CampaignInfo>();

            if (Directory.Exists(sourceDirectoryPath))
            {
                filePathSet = Directory.GetFiles(sourceDirectoryPath);
            }
            else
            {
                filePathSet = new List<string>();
            }

            vdsMetaFileId = H5F.open(vdsMetaFilePath, H5F.ACC_RDONLY);

            try
            {
                foreach (string sourceFilePath in filePathSet)
                {
                    targetFileId = -1;
                    logFilePath = Path.Combine(logDirectoryPath, $"aggregations_{ Path.GetFileNameWithoutExtension(sourceFilePath) }.txt");

                    using (StreamWriter messageLog = new StreamWriter(new FileStream(logFilePath, FileMode.Append)))
                    {
                        Console.WriteLine($"file: { sourceFilePath }");
                        Console.WriteLine();

                        // sourceFileId
                        sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);
                        campaignInfoSet = GeneralHelper.GetCampaignInfoSet(sourceFileId, false);

                        // targetFileId
                        targetFilePath = Path.Combine(targetDirectoryPath, Path.GetFileName(sourceFilePath));

                        if (!Directory.Exists(targetDirectoryPath))
                        {
                            Directory.CreateDirectory(targetDirectoryPath);
                        }

                        if (File.Exists(targetFilePath))
                        {
                            targetFileId = H5F.open(targetFilePath, H5F.ACC_RDWR);
                        }

                        if (targetFileId == -1)
                        {
                            targetFileId = H5F.create(targetFilePath, H5F.ACC_TRUNC);
                        }

                        try
                        {
                            // create attribute if necessary
                            if (H5A.exists(targetFileId, "date_time") == 0)
                            {
                                dateTime = IOHelper.ReadAttribute<string>(sourceFileId, "date_time");
                                IOHelper.PrepareAttribute(targetFileId, "date_time", dateTime.ToArray(), new ulong[] { 1 }, true);
                            }

                            // campaignInfo
                            foreach (var campaignInfo in campaignInfoSet)
                            {
                                Program.AggregateCampaignInfo(sourceFileId, targetFileId, vdsMetaFileId, campaignInfo, messageLog);
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

        private static void AggregateCampaignInfo(long sourceFileId, long targetFileId, long vdsMetaFileId, CampaignInfo campaignInfo, StreamWriter messageLog)
        {
            int index;
            string datasetPath;
            long groupId;
            bool isNew;

            index = 0;
            datasetPath = $"{ campaignInfo.GetPath() }/is_chunk_completed_set";
            (groupId, isNew) = IOHelper.OpenOrCreateGroup(targetFileId, campaignInfo.GetPath());

            try
            {
                if (isNew || !IOHelper.CheckLinkExists(targetFileId, datasetPath))
                {
                    H5O.copy(sourceFileId, datasetPath, targetFileId, datasetPath);
                }

                foreach (var variableInfo in campaignInfo.VariableInfoSet)
                {
                    index++;

                    Console.WriteLine($"{ variableInfo.VariableNameSet.Last() } ({ index }/{ campaignInfo.VariableInfoSet.Count() })");
                    Program.AggregateVariableInfo(sourceFileId, targetFileId, vdsMetaFileId, variableInfo, messageLog);
                    Console.CursorTop -= 1;
                    Program.ClearCurrentLine();
                }
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
            }
        }

        private static void AggregateVariableInfo(long sourceFileId, long targetFileId, long vdsMetaFileId, VariableInfo variableInfo, StreamWriter messageLog)
        {
            long vdsMetaGroupId = -1;
            long sourceDatasetId = -1;
            long sourceDatasetId_status = -1;
            long typeId = -1;

            string groupPath;
            string sourceDatasetName;

            SampleRate sampleRate;
            hdf_aggregate_function_t[] aggregate_function_set;

            sampleRate = default;
            groupPath = variableInfo.GetPath();

            // if meta data entry exists
            if (IOHelper.CheckLinkExists(vdsMetaFileId, groupPath))
            {
                vdsMetaGroupId = H5G.open(vdsMetaFileId, groupPath);

                try
                {
                    if (H5A.exists(vdsMetaGroupId, "aggregate_function_set") > 0)
                    {
                        aggregate_function_set = IOHelper.ReadAttribute<hdf_aggregate_function_t>(vdsMetaGroupId, "aggregate_function_set");

                        // find proper data source
                        foreach (SampleRate testedSampleRate in Enum.GetValues(typeof(SampleRate)))
                        {
                            sourceDatasetName = $"{ 100 / (int)testedSampleRate } Hz"; // improve: remove magic number

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
                                                      new object[] { variableInfo, groupPath, sourceDatasetId, sourceDatasetId_status, targetFileId, sampleRate, aggregate_function_set, messageLog });

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

        private static void AggregateDataset<T>(VariableInfo variableInfo, string groupPath, long sourceDatasetId, long sourceDatasetId_status, long targetFileId, SampleRate sampleRate, hdf_aggregate_function_t[] aggregate_function_set, StreamWriter messageLog)
        {
            int chunkCount;
            long targetDatasetId = -1;
            string targetDatasetPath;
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
                    targetDatasetPath = $"{ groupPath }/{ (int)period} s_{aggregation_function.type }";

                    // if target dataset does not yet exist
                    if (!IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
                    {
                        Console.WriteLine($"\t\t{ (int)period } s");

                        if (sourceValueSet == null)
                        {
                            sourceValueSet = IOHelper.Read<T>(sourceDatasetId, DataContainerType.Dataset);
                            sourceValueSet_status = IOHelper.Read<byte>(sourceDatasetId_status, DataContainerType.Dataset);
                        }

                        chunkCount = sourceValueSet.Count() / (int)period / 100 * (int)sampleRate; // Improve: remove magic number

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
            int chunkSize;
            double[] result;

            //
            chunkSize = valueSet.Count() / targetDatasetLength;
            result = new double[targetDatasetLength];

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
            int chunkSize;
            double[] result;

            //
            chunkSize = valueSet.Count() / targetDatasetLength;
            result = new double[targetDatasetLength];

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
                                {
                                    bitField_and[x] = GenericBitOr<T>.BitOr(bitField_and[x], valueSet[baseIndex + i]);
                                }
                                else
                                {
                                    bitField_and[x] = GenericBitAnd<T>.BitAnd(bitField_and[x], valueSet[baseIndex + i]);
                                }
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

        #region "IMPORT"

        private static void ImportFiles(string dataBaseDirectoryPath, string sourceDirectoryPath, string campaignName, string dataWriterId)
        {
            string logDirectoryPath;
            string logFilePath;
            
            logDirectoryPath = Path.Combine(dataBaseDirectoryPath, "SUPPORT", "LOGS", "VdsTool");
            logFilePath = Path.Combine(logDirectoryPath, $"{ campaignName }.txt");

            try
            {
                Directory.GetDirectories(sourceDirectoryPath, $"{ campaignName }_V*").ToList().ForEach(currentDirectory =>
                {
                    Program.DownloadData(campaignName, Path.Combine(currentDirectory, dataWriterId), dataBaseDirectoryPath, logFilePath, 10);
                });
            }
            catch
            {
                //
            }
        }

        private static void DownloadData(string campaignName, string sourceDirectoryPath, string databaseDirectoryPath, string logFilePath, int period)
        {
            DateTime dateTimeBegin;
            DateTime dateTimeEnd;

            dateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            dateTimeBegin = dateTimeEnd.AddDays(-period);

            //
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            File.AppendAllText(logFilePath, $"BEGIN from { dateTimeBegin.ToString("yyyy-MM-dd") } to { dateTimeEnd.ToString("yyyy-MM-dd") }{ Environment.NewLine }");

            for (int i = 0; i <= (dateTimeEnd - dateTimeBegin).TotalDays; i++)
            {
                DateTime currentDateTime;

                string currentFileName;
                string currentSourceFilePath;
                string currentTargetFilePath;

                currentDateTime = dateTimeBegin.AddDays(i);
                currentFileName = $"{ campaignName }_V*_{ currentDateTime.ToString("yyyy-MM-ddTHH-mm-ssZ") }.h5";
                currentSourceFilePath = Directory.EnumerateFiles(sourceDirectoryPath, currentFileName).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(currentSourceFilePath))
                {
                    continue;
                }

                currentFileName = Path.GetFileName(currentSourceFilePath);
                currentTargetFilePath = Path.Combine(databaseDirectoryPath, "DB_NATIVE", currentDateTime.ToString("yyyy-MM"), currentFileName);

                System.Diagnostics.Trace.WriteLine(currentSourceFilePath);

                if (!File.Exists(currentTargetFilePath))
                {
                    try
                    {
                        Console.Write($"Copying file { currentFileName } ... ");
                        Directory.CreateDirectory(Path.GetDirectoryName(currentTargetFilePath));
                        File.Copy(currentSourceFilePath, currentTargetFilePath);
                        Console.WriteLine($"Done.");
                        File.AppendAllText(logFilePath, $"\tsuccessful: { currentSourceFilePath }{ Environment.NewLine }");
                    }
                    catch (Exception)
                    {
                        File.Delete(currentSourceFilePath);
                        Console.WriteLine($"Failed.");
                        File.AppendAllText(logFilePath, $"\tfailed: { currentSourceFilePath }{ Environment.NewLine }");
                    }
                }
            }

            File.AppendAllText(logFilePath, $"END{ Environment.NewLine }{ Environment.NewLine }");
        }

        #endregion

        private static void ClearCurrentLine()
        {
            Console.Write($"\r{ new string(' ', Console.WindowWidth - 1) }\r");
        }
    }
}
