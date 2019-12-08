using HDF.PInvoke;
using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Extension.Hdf
{
    // v1: initial version
    // v2: 2017-08-07 -> unit_set, transfer_function_set
    [DataWriterFormatVersion(2)]
    public class HdfWriter : DataWriterExtensionLogicBase
    {
        #region "Fields"

        private HdfSettings _settings;

        private IList<string> _systemTimeChangedSet;
        private bool _systemTimeChanged_second;

        private string _dataFilePath;

        private long _lastCompletedChunk;
        private long _fileId = -1;

        IntPtr _isChunkCompletedIntPtr;

        #endregion

        #region "Constructors"

        public HdfWriter(HdfSettings settings, ILoggerFactory loggerFactory) : base(settings, loggerFactory)
        {
            uint isLibraryThreadSafe;

            _settings = settings;

            _systemTimeChangedSet = new List<string>();
            _isChunkCompletedIntPtr = Marshal.AllocHGlobal(1);

            Marshal.WriteByte(_isChunkCompletedIntPtr, 1);

            // check thread safety
            isLibraryThreadSafe = 0;
            H5.is_library_threadsafe(ref isLibraryThreadSafe);

            if (isLibraryThreadSafe <= 0)
            {
                throw new Exception(ErrorMessage.HdfWriter_HdfLibraryNotThreadSafe);
            }
        }

        #endregion

        #region "Methods"

        protected override void OnPrepareFile(DateTime startDateTime, ulong samplesPerDay, IList<VariableContext> variableContextSet)
        {
            _dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{this.DataWriterContext.CampaignDescription.PrimaryGroupName}_{this.DataWriterContext.CampaignDescription.SecondaryGroupName}_{this.DataWriterContext.CampaignDescription.CampaignName}_V{this.DataWriterContext.CampaignDescription.Version}_{startDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z.h5");

            if (_fileId > -1)
                this.CloseHdfFile(_fileId);

#warning Use Span.GetPinnableReference, remove DataBufferPtr property and pass an already sliced Span to the writers instead of 'dataStorage.DataBufferPtr + (int)dataStorageOffset + ...' and 'simpleDataStorageSet[i].DataBuffer[(int)(dataStorageOffset + rowIndex)]'?
            this.OpenFile(_dataFilePath, startDateTime, variableContextSet);

            _systemTimeChangedSet.Clear();
        }

        protected override void OnWrite(ulong samplesPerDay, ulong fileOffset, ulong dataStorageOffset, ulong length, IList<VariableContext> variableContextSet)
        {
            long datasetId = -1;
            long dataspaceId = -1;
            long dataspaceId_memory = -1;
            long groupId = -1;

            long result = -1;

            long firstChunk;
            long lastChunk;

            try
            {
                firstChunk = (long)this.ToChunkIndex(fileOffset, samplesPerDay);
                lastChunk = (long)this.ToChunkIndex(fileOffset + length, samplesPerDay) - 1;

                groupId = H5G.open(_fileId, $"/{this.DataWriterContext.CampaignDescription.PrimaryGroupName}/{this.DataWriterContext.CampaignDescription.SecondaryGroupName}/{this.DataWriterContext.CampaignDescription.CampaignName}");
                datasetId = H5D.open(groupId, "is_chunk_completed_set");
                dataspaceId = H5D.get_space(datasetId);
                dataspaceId_memory = H5S.create_simple(1, new ulong[] { 1 }, new ulong[] { 1 });

                _lastCompletedChunk = IOHelper.ReadAttribute<long>(groupId, "last_completed_chunk").FirstOrDefault();

                if (firstChunk <= _lastCompletedChunk)
                {
                    //throw new Exception(ErrorMessage.HdfLogic_ChunkAlreadyWritten);
                }

                // write data
                for (int i = 0; i < variableContextSet.Count(); i++)
                {
                    this.WriteData(fileOffset, dataStorageOffset, length, variableContextSet[i]);
                }

                // system time changed
                IOHelper.PrepareAttribute(_fileId, "system_time_changed_set", _systemTimeChangedSet.ToArray(), new ulong[] { H5S.UNLIMITED }, true);

                // write is_chunk_completed_set
                for (long chunkIndex = firstChunk; chunkIndex <= lastChunk; chunkIndex++)
                {
                    result = H5S.select_hyperslab(dataspaceId, H5S.seloper_t.SET, new ulong[] { (ulong)chunkIndex }, new ulong[] { 1 }, new ulong[] { 1 }, new ulong[] { 1 });
                    result = H5D.write(datasetId, H5T.NATIVE_UINT8, dataspaceId_memory, dataspaceId, H5P.DEFAULT, _isChunkCompletedIntPtr);
                }

                // write last_completed_chunk
                IOHelper.WriteAttribute(groupId, "last_completed_chunk", new long[] { lastChunk });
            }
            finally
            {
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                if (H5I.is_valid(dataspaceId_memory) > 0) { H5S.close(dataspaceId_memory); }
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }

                H5F.flush(_fileId, H5F.scope_t.GLOBAL);
            }
        }

        protected override void FreeUnmanagedResources()
        {
            base.FreeUnmanagedResources();

            if (H5I.is_valid(_fileId) > 0)
                this.CloseHdfFile(_fileId);

            Marshal.FreeHGlobal(_isChunkCompletedIntPtr);
        }

        private void OpenFile(string filePath, DateTime startDateTime, IList<VariableContext> variableContextSet)
        {
            long groupId = -1;
            long datasetId = -1;
            long typeId = -1;

            try
            {
                _fileId = -1;

                // open file
                if (File.Exists(filePath))
                {
                    _fileId = H5F.open(filePath, H5F.ACC_RDWR, H5P.DEFAULT);

                    if (_fileId < 0)
                    {
                        try
                        {
                            File.Copy(filePath, filePath + ".backup");
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }

                if (_fileId < 0)
                    _fileId = H5F.create(filePath, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);

                if (_fileId < 0)
                    throw new Exception($"{ ErrorMessage.HdfWriter_CouldNotOpenOrCreateFile } File: { filePath }.");

                // file
                IOHelper.PrepareAttribute(_fileId, "format_version", new int[] { typeof(HdfWriter).GetFirstAttribute<DataWriterFormatVersionAttribute>().FormatVersion }, new ulong[] { 1 }, true);
                IOHelper.PrepareAttribute(_fileId, "system_name", new string[] { $"{ this.DataWriterContext.SystemName }" }, new ulong[] { 1 }, true);
                IOHelper.PrepareAttribute(_fileId, "date_time", new string[] { $"{ startDateTime.ToString("yyyy-MM-ddTHH-mm-ss") }Z" }, new ulong[] { 1 }, true);
                IOHelper.PrepareAttribute(_fileId, "system_time_changed_set", new string[] { }, new ulong[] { H5S.UNLIMITED }, false);

                foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.File))
                {
                    IOHelper.PrepareAttribute(_fileId, customMetadataEntry.Key, new string[] { customMetadataEntry.Value }, new ulong[] { 1 }, true);
                }

                // file -> campaign
                groupId = IOHelper.OpenOrCreateGroup(_fileId, $"/{this.DataWriterContext.CampaignDescription.PrimaryGroupName}/{this.DataWriterContext.CampaignDescription.SecondaryGroupName}/{this.DataWriterContext.CampaignDescription.CampaignName}").GroupId;

                IOHelper.PrepareAttribute(groupId, "campaign_version", new int[] { this.DataWriterContext.CampaignDescription.Version }, new ulong[] { 1 }, true);

                foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.Campaign))
                {
                    IOHelper.PrepareAttribute(groupId, customMetadataEntry.Key, new string[] { customMetadataEntry.Value }, new ulong[] { 1 }, true);
                }

                // file -> campaign -> channels
                foreach (VariableContext variableContext in variableContextSet)
                {
                    this.PrepareVariable(groupId, variableContext.VariableDescription);
                }

                // file -> chunk info
                IOHelper.PrepareAttribute(groupId, "last_completed_chunk", new long[] { -1 }, new ulong[] { 1 }, true);
                typeId = TypeConversionHelper.GetHdfTypeIdFromType(_fileId, typeof(byte));
                datasetId = IOHelper.OpenOrCreateDataset(groupId, "is_chunk_completed_set", typeId, this.ChunkCount, 1).DatasetId;
            }
            catch
            {
                if (H5I.is_valid(_fileId) > 0) { H5F.close(_fileId); }

                throw;
            }
            finally
            {
                if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }

                H5F.flush(_fileId, H5F.scope_t.GLOBAL);
            }
        }

        /// <summary>
        /// Writes data to the specified chunk.
        /// </summary>
        /// <param name="chunkNumber">The chunk number.</param>
        /// <param name="dataStorage">The <see cref="DataStorage(Of T)"/>.</param>
        /// <param name="storageType">The <see cref="StorageType"/>.</param>
        private void WriteData(ulong fileOffset, ulong dataStorageOffset, ulong length, VariableContext variableContext)
        {
            Contract.Requires(variableContext != null, nameof(variableContext));

            long groupId = -1;

            string datasetName;
            long datasetTypeId = -1;
            long datasetId = -1;
            long dataspaceId = -1;
            long dataspaceId_Memory = -1;

            string datasetName_Status;
            long datasetTypeId_Status = -1;
            long datasetId_Status = -1;
            long dataspaceId_Status = -1;
            long dataspaceId_Memory_Status = -1;

            IDataStorage dataStorage;
            IExtendedDataStorage extendedDataStorage;
            Type elementType;

            try
            {
                dataStorage = variableContext.DataStorage;
                extendedDataStorage = dataStorage as IExtendedDataStorage;

                elementType = OneDasUtilities.GetTypeFromOneDasDataType(variableContext.VariableDescription.DataType);
                groupId = H5G.open(_fileId, $"/{ this.DataWriterContext.CampaignDescription.PrimaryGroupName }/{ this.DataWriterContext.CampaignDescription.SecondaryGroupName }/{ this.DataWriterContext.CampaignDescription.CampaignName }/{ variableContext.VariableDescription.Guid.ToString() }");

                // dataset
                datasetName = variableContext.VariableDescription.DatasetName;
                datasetTypeId = TypeConversionHelper.GetHdfTypeIdFromType(elementType);
                datasetId = H5D.open(groupId, datasetName);
                dataspaceId = H5D.get_space(datasetId);
                dataspaceId_Memory = H5S.create_simple(1, new ulong[] { length }, null);

                H5S.select_hyperslab(dataspaceId,
                                    H5S.seloper_t.SET,
                                    new ulong[] { fileOffset },
                                    new ulong[] { 1 },
                                    new ulong[] { 1 },
                                    new ulong[] { length });

                if (elementType.IsPrimitive)
                {
                    if (H5D.write(datasetId, datasetTypeId, dataspaceId_Memory, dataspaceId, H5P.DEFAULT, dataStorage.DataBufferPtr + (int)dataStorageOffset * extendedDataStorage.ElementSize) < 0)
                    {
                        throw new Exception(ErrorMessage.HdfWriter_CouldNotWriteChunk_Dataset);
                    }
                }
                else
                {
                    throw new Exception(ErrorMessage.HdfWriter_ElementTypeNonPrimitive);
                }

                // dataset status
                if (extendedDataStorage != null)
                {
                    datasetName_Status = $"{datasetName}_status";
                    datasetTypeId_Status = TypeConversionHelper.GetHdfTypeIdFromType(typeof(byte));
                    datasetId_Status = H5D.open(groupId, datasetName_Status);
                    dataspaceId_Status = H5D.get_space(datasetId_Status);
                    dataspaceId_Memory_Status = H5S.create_simple(1, new ulong[] { length }, null);

                    H5S.select_hyperslab(dataspaceId_Status, 
                        H5S.seloper_t.SET, 
                        new ulong[] { fileOffset }, 
                        new ulong[] { 1 }, 
                        new ulong[] { 1 }, 
                        new ulong[] { length });

                    if (H5D.write(datasetId_Status, datasetTypeId_Status, dataspaceId_Memory_Status, dataspaceId_Status, H5P.DEFAULT, extendedDataStorage.StatusBufferPtr + (int)dataStorageOffset) < 0)
                    {
                        throw new Exception(ErrorMessage.HdfWriter_CouldNotWriteChunk_Status);
                    }
                }
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }

                // dataset status
                if (H5I.is_valid(datasetId_Status) > 0) { H5D.close(datasetId_Status); }
                if (H5I.is_valid(dataspaceId_Status) > 0) { H5S.close(dataspaceId_Status); }
                if (H5I.is_valid(dataspaceId_Memory_Status) > 0) { H5S.close(dataspaceId_Memory_Status); }
                if (H5I.is_valid(datasetTypeId_Status) > 0) { H5T.close(datasetTypeId_Status); }

                // dataset
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                if (H5I.is_valid(dataspaceId_Memory) > 0) { H5S.close(dataspaceId_Memory); }
                if (H5I.is_valid(datasetTypeId) > 0) { H5T.close(datasetTypeId); }
            }
        }

        private void PrepareVariable(long locationId, VariableDescription variableDescription)
        {
            Contract.Requires(variableDescription != null, nameof(variableDescription));

            long groupId = -1;
            long datasetId = -1;
            long datasetTypeId = -1;
            long datasetId_status = -1;

            ulong chunkLength;

            string datasetName = null;

            hdf_transfer_function_t[] transferFunctionSet;

            try
            {
                // chunk length
                chunkLength = this.TimeSpanToIndex(this.ChunkPeriod, variableDescription.SamplesPerDay);

                if (chunkLength <= 0)
                    throw new Exception(ErrorMessage.HdfWriter_SampleRateTooLow);

                // group (GUID)
                groupId = IOHelper.OpenOrCreateGroup(locationId, variableDescription.Guid.ToString()).GroupId;

                // attributes
                transferFunctionSet = variableDescription.TransferFunctionSet.Select(tf => new hdf_transfer_function_t(tf.DateTime.ToString("yyyy-MM-ddTHH-mm-ssZ"), tf.Type, tf.Option, tf.Argument)).ToArray();

                IOHelper.PrepareAttribute(groupId, "name_set", new string[] { variableDescription.VariableName }, new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(groupId, "group_set", new string[] { variableDescription.Group }, new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(groupId, "comment_set", new string[] { "yyyy-MM-ddTHH-mm-ssZ: Comment1" }, new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(groupId, "unit_set", new string[] { variableDescription.Unit }, new ulong[] { H5S.UNLIMITED }, true);
                IOHelper.PrepareAttribute(groupId, "transfer_function_set", transferFunctionSet, new ulong[] { H5S.UNLIMITED }, true);

                // dataset (native)
                datasetName = variableDescription.DatasetName;
                datasetTypeId = TypeConversionHelper.GetHdfTypeIdFromType(OneDasUtilities.GetTypeFromOneDasDataType(variableDescription.DataType));
                datasetId = IOHelper.OpenOrCreateDataset(groupId, datasetName, datasetTypeId, chunkLength, this.ChunkCount).DatasetId;

                // dataset (status)
                if (typeof(IExtendedDataStorage).IsAssignableFrom(variableDescription.DataStorageType))
                {
                    datasetName = $"{datasetName}_status";
                    datasetId_status = IOHelper.OpenOrCreateDataset(groupId, datasetName, H5T.NATIVE_UINT8, chunkLength, this.ChunkCount).DatasetId;
                }
            }
            finally
            {
                if (H5I.is_valid(datasetId_status) > 0) { H5D.close(datasetId_status); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(datasetTypeId) > 0) { H5T.close(datasetTypeId); }

                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
            }          
        }

        private void CloseHdfFile(long fileId)
        {
            if (H5I.is_valid(fileId) > 0)
                H5F.close(fileId);
        }

        #endregion
    }
}
