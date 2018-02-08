using HDF.PInvoke;
using MathNet.Numerics.Statistics;
using OneDas.Common;
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

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            H5.is_library_threadsafe(ref _isLibraryThreadSafe);

            while (true)
            {
                Console.CursorVisible = true;

                while (true)
                {
                    Console.Clear();

                    if (_isLibraryThreadSafe < 0)
                    {
                        Console.WriteLine("Warning: libary is not thread safe!");
                    }

                    Console.WriteLine("Please enter the base directory path of the HDF files:");

                    Program.BaseDirectoryPath = Console.ReadLine() + @"\";

                    if (Directory.Exists(Program.BaseDirectoryPath) &&
                        Directory.Exists(Path.Combine(Program.BaseDirectoryPath, "DB_AGGREGATION")) &&
                        Directory.Exists(Path.Combine(Program.BaseDirectoryPath, "DB_IMPORT")) &&
                        Directory.Exists(Path.Combine(Program.BaseDirectoryPath, "DB_NATIVE")) &&
                        Directory.Exists(Path.Combine(Program.BaseDirectoryPath, "VDS")))
                    {
                        break;
                    }
                }

                Console.CursorVisible = false;

                new MainMenuNavigator();
            }
        }

        #region "VDS"

        public static void CreateVirtualDatasetFile(List<string> sourceDirectoryPathSet, string vdsFilePath, DateTime epochStart, DateTime epochEnd, bool isTopLevel)
        {        
            long sourceFileId = -1;
            long vdsFileId = -1;
            long vdsCampaignGroupId = -1;
            long vdsVariableGroupId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            string lastVariablePath = String.Empty;
            string vdsGroupPath;

            var campaignInfoSet = new Dictionary<string, CampaignInfo>();
            var sourceFilePathSet = new List<string>();

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
            
            // write attribute
            IOHelper.PrepareAttribute(vdsFileId, "date_time", new string[] { $"{ epochStart.ToString("yyyy-MM-ddTHH-mm-ss") }Z" }, new ulong[] { 1 }, true);

            // for each source file
            foreach (string sourceFilePath in sourceFilePathSet)
            {
                Console.Write($"Processing file { Path.GetFileName(sourceFilePath) } ... ");

                sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);

                if (sourceFileId < 0)
                {
                    throw new Exception(ErrorMessage.Program_CouldNotOpenFile);
                }

                GeneralHelper.UpdateCampaignInfoSet(sourceFileId, campaignInfoSet);

                Console.WriteLine("Done.");

                // clean up
                H5F.close(sourceFileId);
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
                Console.WriteLine($"\n{ campaignInfo.Key }");

                vdsGroupPath = campaignInfo.Key;
                vdsCampaignGroupId = IOHelper.OpenOrCreateGroup(vdsFileId, vdsGroupPath).GroupId;

                // variable
                foreach (var variableInfo in campaignInfo.Value.VariableInfoSet)
                {                
                    Console.WriteLine($"\t{variableInfo.Key}");

                    vdsVariableGroupId = IOHelper.OpenOrCreateGroup(vdsCampaignGroupId, variableInfo.Key).GroupId;

                    //// make hard links for each display name
                    ////foreach (string variableName in variableInfo.Value.VariableNameSet)
                    ////{
                    ////    H5L.copy(vdsGroupIdSet[vdsGroupIdSet.Count() - 1], variableInfo.Key, vdsGroupIdSet[vdsGroupIdSet.Count() - 2], variableName);
                    ////}

                    IOHelper.PrepareAttribute(vdsVariableGroupId, "name_set", variableInfo.Value.VariableNameSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                    IOHelper.PrepareAttribute(vdsVariableGroupId, "group_set", variableInfo.Value.VariableGroupSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                    IOHelper.PrepareAttribute(vdsVariableGroupId, "unit_set", variableInfo.Value.UnitSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                    IOHelper.PrepareAttribute(vdsVariableGroupId, "transfer_function_set", variableInfo.Value.TransferFunctionSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);

                    // dataset
                    foreach (var datasetInfo in variableInfo.Value.DatasetInfoSet)
                    {
                        Console.WriteLine($"\t\t{ datasetInfo.Key }");
                        Program.MergeDatasets(vdsVariableGroupId, $"{ vdsGroupPath }/{ variableInfo.Key }", epochStart, epochEnd, datasetInfo.Value, true);
                    }

                    // flush data - necessary to avoid AccessViolationException at H5F.close()
                    H5F.flush(vdsFileId, H5F.scope_t.LOCAL);

                    // clean up
                    H5G.close(vdsVariableGroupId);
                }

                // don't forget is_chunk_completed_set
                Program.MergeDatasets(vdsCampaignGroupId, vdsGroupPath, epochStart, epochEnd, campaignInfo.Value.ChunkDatasetInfo, false);

                // clean up
                H5G.close(vdsCampaignGroupId);
            }

            // clean up
            H5F.close(vdsFileId);
        }

        private static void MergeDatasets(long groupId, string groupPath, DateTime epochStart, DateTime epochEnd, DatasetInfo datasetInfo, bool closeType)
        {
            string datasetName;

            ulong lengthPerDay;
            ulong vdsLength;

            long datasetId = -1;
            long spaceId = -1;
            long propertyId = -1;
            long result = -1;

            GCHandle gcHandle;

            //
            datasetName = datasetInfo.Name;
            lengthPerDay = InfrastructureHelper.GetSamplesPerDayFromString(datasetName);
            vdsLength = (ulong)(epochEnd - epochStart).Days * lengthPerDay;
            spaceId = H5S.create_simple(1, new ulong[] { vdsLength }, new ulong[] { H5S.UNLIMITED });
            propertyId = H5P.create(H5P.DATASET_CREATE);

            foreach ((string sourceFilePath, ulong length, DateTime dateTime) in datasetInfo.SourceFileInfoSet)
            {
                ulong start;
                ulong stride;
                ulong count;
                ulong block;

                long sourceSpaceId = -1;

                sourceSpaceId = H5S.create_simple(1, new ulong[] { length }, new ulong[] { length });

                start = (ulong)((dateTime - epochStart).TotalDays * lengthPerDay);
                stride = 1;
                count = 1;
                block = length;

                if (start + stride > vdsLength)
                {
                    throw new Exception($"start + stride = { start + stride } > { vdsLength }");
                }

                result = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                result = H5P.set_virtual(propertyId, spaceId, sourceFilePath, $"{ groupPath }/{ datasetName }", sourceSpaceId);
                
                // clean up
                H5S.close(sourceSpaceId);
            }

            H5S.select_all(spaceId);

            if (TypeConversionHelper.GetTypeFromHdfTypeId(datasetInfo.TypeId) == typeof(double))
            {
                gcHandle = GCHandle.Alloc(Double.NaN, GCHandleType.Pinned);
                H5P.set_fill_value(propertyId, datasetInfo.TypeId, gcHandle.AddrOfPinnedObject());
                gcHandle.Free();
            }

            datasetId = H5D.create(groupId, datasetName, datasetInfo.TypeId, spaceId, H5P.DEFAULT, propertyId);

            // clean up
            H5P.close(propertyId);
            H5D.close(datasetId);
            H5S.close(spaceId);

            if (closeType)
            {
                H5T.close(datasetInfo.TypeId);
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
            isEscaped = false;

            Console.CursorVisible = true;

            // type
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for type ({hdf_aggregate_function.type}): ");

                optionSet = new List<string>() { "mean", "min", "max", "std", "min_bitwise", "max_bitwise" };

                if (!string.IsNullOrWhiteSpace(type = ConsoleHelper.ReadLine(optionSet, ref isEscaped)) && InfrastructureHelper.CheckNamingConvention(type, out tmp))
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
                Console.Write($"Enter value for argument ({hdf_aggregate_function.argument}): ");

                optionSet = new List<string>() { "none" };

                if (!string.IsNullOrWhiteSpace(argument = ConsoleHelper.ReadLine(optionSet, ref isEscaped)))
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
            isEscaped = false;

            Console.CursorVisible = true;
                        
            // date / time
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for date/time ({hdf_transfer_function.date_time}): ");

                optionSet = new List<string>() { "0001-01-01" };
                dateTime_tmp = ConsoleHelper.ReadLine(optionSet, ref isEscaped);

                if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    hdf_transfer_function.date_time = dateTime.ToString("yyyy-MM-ddTHH-mm-ssZ");
                    break;
                }
                else if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
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
                Console.Write($"Enter value for type ({hdf_transfer_function.type}): ");

                optionSet = new List<string>() { "polynomial", "function" };

                if (!string.IsNullOrWhiteSpace(type = ConsoleHelper.ReadLine(optionSet, ref isEscaped)))
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
                Console.Write($"Enter value for option ({hdf_transfer_function.option}): ");

                optionSet = new List<string>() { "permanent" };

                if (!string.IsNullOrWhiteSpace(option = ConsoleHelper.ReadLine(optionSet, ref isEscaped)))
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
                Console.Write($"Enter value for argument ({hdf_transfer_function.argument}): ");

                optionSet = new List<string>() { };

                if (!string.IsNullOrWhiteSpace(argument = ConsoleHelper.ReadLine(optionSet, ref isEscaped)))
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
            isEscaped = false;

            Console.CursorVisible = true;

            // date / time
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter value for date/time ({hdf_tag.date_time}): ");

                optionSet = new List<string>() { "0001-01-01" };
                dateTime_tmp = ConsoleHelper.ReadLine(optionSet, ref isEscaped);

                if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
                {
                    hdf_tag.date_time = dateTime.ToString("yyyy-MM-ddTHH-mm-ssZ");
                    break;
                }
                else if (DateTime.TryParseExact(dateTime_tmp, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
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
                Console.Write($"Enter value for name ({hdf_tag.name}): ");

                optionSet = new List<string>() { };

                if (!string.IsNullOrWhiteSpace(name = ConsoleHelper.ReadLine(optionSet, ref isEscaped)) && InfrastructureHelper.CheckNamingConvention(name, out tmp))
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
                Console.Write($"Enter value for mode ({hdf_tag.mode}): ");

                optionSet = new List<string>() { "none", "start", "end" };

                if (!string.IsNullOrWhiteSpace(comment = ConsoleHelper.ReadLine(optionSet, ref isEscaped)))
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
                Console.Write($"Enter value for comment ({hdf_tag.comment}): ");

                optionSet = new List<string>() { "none"};

                if ((mode = ConsoleHelper.ReadLine(optionSet, ref isEscaped)).Any())
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

        #endregion

        #region "AGGREGATION"

        public static void CreateAggregatedFiles(string sourceDirectoryPath, string targetDirectoryPath, string logDirectoryPath, string vdsMetaFilePath, DateTime epochStart, DateTime epochEnd)
        {
            IList<string> dateTime;
            IList<string> filePathSet;
            hdf_aggregate_function_t[] aggregate_function_set;
            SampleRate sampleRate = default;

            var campaignInfoSet = new Dictionary<string, CampaignInfo>();

            int index;

            string sourceDatasetName = string.Empty;
            string targetFilePath;
            string groupPath;
            string logFilePath;

            long vdsMetaFileId = -1;
            long vdsMetaGroupId = -1;
            long sourceFileId = -1;
            long sourceDatasetId = -1;
            long sourceDatasetId_status = -1;
            long targetFileId = -1;
            long typeId = -1;

            if (Directory.Exists(sourceDirectoryPath))
            {
                filePathSet = Directory.GetFiles(sourceDirectoryPath);
            }
            else
            {
                filePathSet = new List<string>();
            }

            // vdsMetaFileId
            vdsMetaFileId = H5F.open(vdsMetaFilePath, H5F.ACC_RDONLY);

            // sourceFilePath
            foreach (string sourceFilePath in filePathSet)
            {
                targetFileId = -1;
                logFilePath = Path.Combine(logDirectoryPath, $"aggregations_{Path.GetFileNameWithoutExtension(sourceFilePath)}.txt");

                using (StreamWriter messageLog = new StreamWriter(new FileStream(logFilePath, FileMode.Append)))
                {
                    Console.WriteLine($"file: {sourceFilePath}");
                    Console.WriteLine();

                    // sourceFileId
                    sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);
                    campaignInfoSet = GeneralHelper.GetCampaignInfoSet(sourceFileId);

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

                    // create attribute if necessary
                    if (H5A.exists(targetFileId, "date_time") == 0)
                    {
                        dateTime = IOHelper.ReadAttribute<string>(sourceFileId, "date_time");
                        IOHelper.PrepareAttribute(targetFileId, "date_time", dateTime.ToArray(), new ulong[] { 1 }, true);
                    }

                    // campaignInfo
                    foreach (var campaignInfo in campaignInfoSet)
                    {
                        index = 0;

                        // variableInfo
                        foreach (var variableInfo in campaignInfo.Value.VariableInfoSet)
                        {
                            index++;
                            Console.WriteLine($"{variableInfo.Value.VariableNameSet.Last()} ({index}/{campaignInfo.Value.VariableInfoSet.Count()})");

                            //if (variableInfo.Key != "e5d628c9-3592-4245-92a5-deb3e678b788")
                            //{
                            //    continue;
                            //}

                            groupPath = $"{campaignInfo.Value.Name}/{variableInfo.Value.Name}";

                            // if meta data entry exists
                            if (IOHelper.CheckLinkExists(vdsMetaFileId, groupPath))
                            {
                                vdsMetaGroupId = H5G.open(vdsMetaFileId, groupPath);

                                if (H5A.exists(vdsMetaGroupId, "aggregate_function_set") > 0)
                                {
                                    aggregate_function_set = IOHelper.ReadAttribute<hdf_aggregate_function_t>(vdsMetaGroupId, "aggregate_function_set");

                                    // find proper data source
                                    foreach (SampleRate testedSampleRate in Enum.GetValues(typeof(SampleRate)))
                                    {
                                        sourceDatasetName = $"{ 100 / (int)testedSampleRate } Hz"; // improve: remove magic number

                                        if (variableInfo.Value.DatasetInfoSet.Where(x => x.Value.Name == sourceDatasetName).Any())
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
                                        messageLog.WriteLine($"No appropriate data source was found. Skipping variable.");
                                        return;
                                    }

                                    typeId = H5D.get_type(sourceDatasetId);

                                    // invoke Program.DoAggregationStuff
                                    GeneralHelper.InvokeGenericMethod(typeof(Program), null, nameof(Program.DoAggregationStuff), 
                                                                  BindingFlags.NonPublic | BindingFlags.Static,
                                                                  TypeConversionHelper.GetTypeFromHdfTypeId(typeId),
                                                                  new object[] { variableInfo.Value, groupPath, sourceDatasetId, sourceDatasetId_status, targetFileId, sampleRate, aggregate_function_set, messageLog });

                                    // clean up
                                    H5D.close(sourceDatasetId_status);
                                    H5D.close(sourceDatasetId);
                                    H5T.close(typeId);
                                }
                                else
                                {
                                    messageLog.WriteLine($"The attribute 'aggregate_function_set' of variable {groupPath} does not exist in file VDS_META.h5. Skipping variable.");
                                }

                                // clean up
                                H5G.close(vdsMetaGroupId);
                            }
                            else
                            {
                                messageLog.WriteLine($"The variable {groupPath} does not exist in file VDS_META.h5. Skipping variable.");
                            }

                            Console.CursorTop -= 1;
                            Program.ClearCurrentLine();
                        }
                    }

                    Console.CursorTop -= 1;

                    // clean up
                    H5F.close(targetFileId);
                    H5F.close(sourceFileId);
                }
            }

            // clean up
            H5F.close(vdsMetaFileId);
        }

        private static void DoAggregationStuff<T>(VariableInfo variableInfo, string groupPath, long sourceDatasetId, long sourceDatasetId_status, long targetFileId, SampleRate sampleRate, hdf_aggregate_function_t[] aggregate_function_set, StreamWriter messageLog)
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
                Console.WriteLine($"\t{aggregation_function.type}");

                // for each period
                foreach (Period period in Enum.GetValues(typeof(Period)))
                {
                    targetDatasetPath = $"{groupPath}/{(int)period} s_{aggregation_function.type}";

                    // if target dataset does not yet exist
                    if (!IOHelper.CheckLinkExists(targetFileId, targetDatasetPath))
                    {
                        Console.WriteLine($"\t\t{(int)period} s");

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

                                messageLog.WriteLine($"The type {aggregation_function.type} of attribute 'aggregate_function_set' is unknown. Skipping period {period}.");

                                continue;
                        }

                        targetDatasetId = IOHelper.OpenOrCreateDataset(targetFileId, targetDatasetPath, H5T.NATIVE_DOUBLE, (ulong)chunkCount, 1).DatasetId;

                        IOHelper.Write(targetDatasetId, targetValueSet.ToArray(), DataContainerType.Dataset);
                        IOHelper.PrepareAttribute(targetDatasetId, "aggregate_function_set", new hdf_aggregate_function_t[] { aggregation_function }, new ulong[] { 1 }, true);

                        Console.CursorTop -= 1;
                        Program.ClearCurrentLine();

                        // clean up
                        H5F.flush(targetDatasetId, H5F.scope_t.LOCAL);
                        H5D.close(targetDatasetId);
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

        private static void ClearCurrentLine()
        {
            Console.Write($"\r{new string(' ', Console.WindowWidth - 1)}\r");
        }
    }
}
