using HDF.PInvoke;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Hdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Hdf.VdsTool.Commands
{
    public class VdsCommand
    {
        #region Fields

        private DateTime _epochStart;
        private ILogger _logger;

        private Dictionary<string, List<byte>> _isChunkCompletedMap;
        private Dictionary<DatasetInfo, List<SourceFileInfo>> _datasetToSourceFilesMap;
        private Dictionary<DatasetInfo, long> _datasetToTypeIdMap;

        #endregion

        #region Constructors

        public VdsCommand(DateTime epochStart, ILogger logger)
        {
            _epochStart = epochStart;
            _logger = logger;
        }

        #endregion

        #region Methods

        public void Run()
        {
            string vdsFilePath;
            DateTime epochEnd;

            _isChunkCompletedMap = new Dictionary<string, List<byte>>();
            _datasetToSourceFilesMap = new Dictionary<DatasetInfo, List<SourceFileInfo>>();
            _datasetToTypeIdMap = new Dictionary<DatasetInfo, long>();

            var sourceDirectoryPathSet = new List<string>();

            if (_epochStart > DateTime.MinValue)
            {
                epochEnd = _epochStart.AddMonths(1);
                sourceDirectoryPathSet.Add(Path.Combine(Environment.CurrentDirectory, "DATA", _epochStart.ToString("yyyy-MM")));
                vdsFilePath = Path.Combine(Environment.CurrentDirectory, "VDS", $"{ _epochStart.ToString("yyyy-MM") }.h5");
            }
            else
            {
                _epochStart = new DateTime(2000, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = new DateTime(2030, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                sourceDirectoryPathSet.Add(Path.Combine(Environment.CurrentDirectory, "VDS"));
                vdsFilePath = Path.Combine(Environment.CurrentDirectory, "VDS.h5");
            }

            _logger.LogInformation($"Epoch start: {_epochStart.ToString("yyyy-MM-dd")}");
            _logger.LogInformation($"Epoch end: {epochEnd.ToString("yyyy-MM-dd")}");

            this.CreateVirtualDatasetFile(sourceDirectoryPathSet, vdsFilePath, _epochStart, epochEnd, _logger);
        }

        private void CreateVirtualDatasetFile(List<string> sourceDirectoryPathSet, string vdsFilePath, DateTime epochStart, DateTime epochEnd, ILogger logger)
        {
            long vdsFileId = -1;

            var actualDimenionSet = new ulong[1];
            var maximumDimensionSet = new ulong[1];

            var lastVariablePath = String.Empty;
            var tempFilePath = Path.GetTempFileName();

            var campaigns = new List<CampaignInfo>();
            var sourceFilePathSet = new List<string>();

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
                IOHelper.PrepareAttribute(vdsFileId, "date_time", new string[] { $"{epochStart.ToString("yyyy-MM-ddTHH-mm-ss")}Z" }, new ulong[] { 1 }, true);

                // create an index of all campaigns, variables and datasets
                foreach (string sourceFilePath in sourceFilePathSet)
                {
                    this.VdsSourceFile(sourceFilePath, campaigns, logger);
                }

                //foreach (var variableInfo in variableInfos)
                //{
                //    Console.WriteLine(variableInfo.Key);
                //    Console.WriteLine($"\tVariableNames:{ variableInfo.Value.VariableNames.Count }");
                //    Console.WriteLine($"\tDatasets: { variableInfo.Value.Datasets.Count }");

                //    foreach (var datasetInfo in variableInfo.Value.Datasets)
                //    {
                //        Console.WriteLine($"\t\t{ datasetInfo.Key }");
                //        Console.WriteLine($"\t\t\tLength: { datasetInfo.Value.Length }");
                //        Console.WriteLine($"\t\t\tSourceFileInfoSet: { datasetInfo.Value.SourceFileInfoSet.Count }");
                //    }
                //}

                // write the result into the temporary vds file
                foreach (var campaign in campaigns)
                {
                    this.VdsCampaign(vdsFileId, campaign, epochStart, epochEnd);
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

        private void VdsSourceFile(string sourceFilePath, List<CampaignInfo> campaigns, ILogger logger)
        {
            long sourceFileId = -1;

            var message = $"Processing file {Path.GetFileName(sourceFilePath)} ... ";
            logger.LogInformation(message);

            sourceFileId = H5F.open(sourceFilePath, H5F.ACC_RDONLY);

            try
            {
                if (sourceFileId < 0)
                    throw new Exception(ErrorMessage.VdsCommand_CouldNotOpenFile);

                GeneralHelper.UpdateCampaigns(sourceFileId, campaigns, this.UpdateSourceFileMap);

                // load is_chunk_completed_set
                campaigns.ForEach(campaignInfo =>
                {
                    // the averages database has no "is_chunk_completed_set"
                    if (_datasetToSourceFilesMap.ContainsKey(campaignInfo.ChunkDataset))
                    {
                        if (_datasetToSourceFilesMap[campaignInfo.ChunkDataset].Any(sourceFileInfo => sourceFileInfo.FilePath == sourceFilePath))
                        {
                            var key = $"{sourceFilePath}+{campaignInfo.GetPath()}";

                            if (!_isChunkCompletedMap.ContainsKey(key))
                            {
                                var isChunkCompletedSet = IOHelper.ReadDataset<byte>(sourceFileId, $"{campaignInfo.GetPath()}/is_chunk_completed_set");
                                _isChunkCompletedMap[key] = isChunkCompletedSet.ToList();
                            }
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

        private void VdsCampaign(long vdsFileId, CampaignInfo campaign, DateTime epochStart, DateTime epochEnd)
        {
            long campaignGroupId = -1;

            Console.WriteLine($"\n{campaign.Name}");

            campaignGroupId = IOHelper.OpenOrCreateGroup(vdsFileId, campaign.GetPath()).GroupId;

            try
            {
                // variable
                foreach (var variable in campaign.Variables)
                {
                    this.VdsVariable(vdsFileId, campaignGroupId, variable, epochStart, epochEnd, campaign.GetPath());
                }

                // don't forget is_chunk_completed_set
                this.VdsDataset(campaignGroupId, epochStart, epochEnd, campaign.ChunkDataset, campaign.GetPath());
            }
            finally
            {
                if (H5I.is_valid(campaignGroupId) > 0) { H5G.close(campaignGroupId); }
            }
        }

        private void VdsVariable(long vdsFileId, long vdsCampaignGroupId, VariableInfo variable, DateTime epochStart, DateTime epochEnd, string campaignPath)
        {
            long variableGroupId = -1;

            Console.WriteLine($"\t{variable.Name}");

            variableGroupId = IOHelper.OpenOrCreateGroup(vdsCampaignGroupId, variable.Name).GroupId;

            try
            {
                //// make hard links for each display name
                ////foreach (string variableName in variableInfo.Value.VariableNames)
                ////{
                ////    H5L.copy(vdsGroupIdSet[vdsGroupIdSet.Count() - 1], variableInfo.Key, vdsGroupIdSet[vdsGroupIdSet.Count() - 2], variableName);
                ////}

                IOHelper.PrepareAttribute(variableGroupId, "name_set", variable.VariableNames.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(variableGroupId, "group_set", variable.VariableGroups.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(variableGroupId, "unit_set", variable.Units.ToArray(), new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(variableGroupId, "transfer_function_set", variable.TransferFunctions.Select(tf => hdf_transfer_function_t.FromTransferFunction(tf)).ToArray(), new ulong[] { H5S.UNLIMITED }, true);

                // dataset
                foreach (var dataset in variable.Datasets)
                {
                    Console.WriteLine($"\t\t{ dataset.Name }");
                    this.VdsDataset(variableGroupId, epochStart, epochEnd, dataset, campaignPath);
                }

                // flush data - necessary to avoid AccessViolationException at H5F.close()
                H5F.flush(vdsFileId, H5F.scope_t.LOCAL);
            }
            finally
            {
                if (H5I.is_valid(variableGroupId) > 0) { H5G.close(variableGroupId); }
            }
        }

        private void VdsDataset(long groupId, DateTime epochStart, DateTime epochEnd, DatasetInfo dataset, string campaignPath)
        {
            long datasetId = -1;
            long spaceId = -1;
            long propertyId = -1;

            var createDataset = false;

            // the averages database has no "is_chunk_completed_set"
            if (!_datasetToTypeIdMap.ContainsKey(dataset))
                return;
                
            var typeId = _datasetToTypeIdMap[dataset];

            try
            {
                var sampleRate = dataset.SampleRate;
                var vdsLength = (ulong)(epochEnd - epochStart).Days * sampleRate.SamplesPerDay;

                spaceId = H5S.create_simple(1, new ulong[] { vdsLength }, new ulong[] { H5S.UNLIMITED });
                propertyId = H5P.create(H5P.DATASET_CREATE);

                foreach (SourceFileInfo sourceFileInfo in _datasetToSourceFilesMap[dataset])
                {
                    long sourceSpaceId = -1;

                    sourceSpaceId = H5S.create_simple(1, new ulong[] { sourceFileInfo.Length }, new ulong[] { sourceFileInfo.Length });

                    var relativeFilePath = $".{sourceFileInfo.FilePath.Remove(0, Environment.CurrentDirectory.TrimEnd('\\').TrimEnd('/').Length)}";
                    var key = $"{sourceFileInfo.FilePath}+{campaignPath}";

                    int chunkCount = 1440;
                    int firstIndex = 0;
                    int lastIndex = 1439;

                    // the averages database has no "is_chunk_completed_set"
                    if (_isChunkCompletedMap.ContainsKey(key))
                    {
                        var isChunkCompletedSet = _isChunkCompletedMap[key];
                        chunkCount = isChunkCompletedSet.Count;
                        firstIndex = isChunkCompletedSet.FindIndex(value => value > 0);
                        lastIndex = isChunkCompletedSet.FindLastIndex(value => value > 0);
                    }

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
                                throw new Exception($"start + block = {offset + start + block} > {vdsLength}");

                            H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, new ulong[] { offset + start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                            H5S.select_hyperslab(sourceSpaceId, H5S.seloper_t.SET, new ulong[] { start }, new ulong[] { stride }, new ulong[] { count }, new ulong[] { block });
                            H5P.set_virtual(propertyId, spaceId, relativeFilePath, dataset.GetPath(), sourceSpaceId);
                        }
                        finally
                        {
                            if (H5I.is_valid(sourceSpaceId) > 0) { H5S.close(sourceSpaceId); }
                        }
                    }
                }

                H5S.select_all(spaceId);

                if (TypeConversionHelper.GetTypeFromHdfTypeId(typeId) == typeof(double))
                {
                    var gcHandle = GCHandle.Alloc(Double.NaN, GCHandleType.Pinned);
                    H5P.set_fill_value(propertyId, typeId, gcHandle.AddrOfPinnedObject());
                    gcHandle.Free();
                }

                if (createDataset) // otherwise there will be an error, if set_virtual has never been called.
                    datasetId = H5D.create(groupId, dataset.Name, typeId, spaceId, H5P.DEFAULT, propertyId);
            }
            finally
            {
                if (H5I.is_valid(propertyId) > 0) { H5P.close(propertyId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(spaceId) > 0) { H5S.close(spaceId); }
                if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
            }
        }

        private void UpdateSourceFileMap(long datasetId, DatasetInfo dataset, SourceFileInfo sourceFileInfo)
        {
            if (!_datasetToSourceFilesMap.ContainsKey(dataset))
            {
                _datasetToSourceFilesMap[dataset] = new List<SourceFileInfo>();

                var typeId_do_not_close = H5D.get_type(datasetId);
                _datasetToTypeIdMap[dataset] = typeId_do_not_close;
            }

            _datasetToSourceFilesMap[dataset].Add(sourceFileInfo);
        }

        #endregion
    }
}
