using HDF.PInvoke;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Hdf;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OneDas.Extension.Mat73
{
    // v1: initial version
    [DataWriterFormatVersion(1)]
    public class Mat73Writer : DataWriterExtensionLogicBase
    {
        #region "Fields"

        private const ulong USERBLOCK_SIZE = 512;

        private Mat73Settings _settings;

        private string _dataFilePath;
        private double _lastCompletedChunk;
        private long _fileId = -1;

        private TimeSpan _chunkPeriod;
        private IList<TextEntry> _textEntrySet;

        #endregion

        #region "Constructors"

        public Mat73Writer(Mat73Settings settings, ILogger logger) : base(settings, logger)
        {
            _settings = settings;

            _chunkPeriod = TimeSpan.FromMinutes(1);

            // check thread safety
            var isLibraryThreadSafe = 0U;
            H5.is_library_threadsafe(ref isLibraryThreadSafe);

            if (isLibraryThreadSafe <= 0)
                throw new Exception(ErrorMessage.Mat73Writer_HdfLibraryNotThreadSafe);
        }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            _textEntrySet = new List<TextEntry>();

            _textEntrySet.Add(new TextEntry("/info", "format_version", this.FormatVersion.ToString()));
            _textEntrySet.Add(new TextEntry("/info", "system_name", this.DataWriterContext.SystemName));
            _textEntrySet.Add(new TextEntry("/info", "date_time", string.Empty)); // will be set later in "this.OpenFile()"

            this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.File).ToList().ForEach(customMetadataEntry =>
            {
                _textEntrySet.Add(new TextEntry("/info", customMetadataEntry.Key, customMetadataEntry.Value));
            });

            _textEntrySet.Add(new TextEntry("/info", "campaign_first_level", this.DataWriterContext.CampaignDescription.PrimaryGroupName));
            _textEntrySet.Add(new TextEntry("/info", "campaign_second_level", this.DataWriterContext.CampaignDescription.SecondaryGroupName));
            _textEntrySet.Add(new TextEntry("/info", "campaign_name", this.DataWriterContext.CampaignDescription.CampaignName));
            _textEntrySet.Add(new TextEntry("/info", "campaign_version", this.DataWriterContext.CampaignDescription.Version.ToString()));

            this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.Campaign).ToList().ForEach(customMetadataEntry =>
            {
                _textEntrySet.Add(new TextEntry("/info", customMetadataEntry.Key, customMetadataEntry.Value));
            });

            if (this.DataWriterContext.CustomMetadataEntrySet.Count() > 58)
                throw new Exception("The number of custom metadata entries must not exceed 58.");
        }

        protected override void OnPrepareFile(DateTime startDateTime, List<VariableContextGroup> variableContextGroupSet)
        {
            var campaignDescription = this.DataWriterContext.CampaignDescription;
            _dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{campaignDescription.PrimaryGroupName}_{campaignDescription.SecondaryGroupName}_{campaignDescription.CampaignName}_V{campaignDescription.Version}_{startDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z.mat");

            if (_fileId > -1)
                this.CloseHdfFile(_fileId);

            this.OpenFile(_dataFilePath, startDateTime, variableContextGroupSet.SelectMany(contextGroup => contextGroup.VariableContextSet).ToList());
        }

        protected override void OnWrite(VariableContextGroup contextGroup, ulong fileOffset, ulong bufferOffset, ulong length)
        {
            long groupId = -1;

            try
            {
                var firstChunk = (long)this.ToChunkIndex(fileOffset, contextGroup.SampleRate);
                var lastChunk = (long)this.ToChunkIndex(fileOffset + length, contextGroup.SampleRate) - 1;

                groupId = H5G.open(_fileId, $"/info");

                _lastCompletedChunk = IOHelper.ReadDataset<double>(groupId, "last_completed_chunk").FirstOrDefault();

                // this does not work in conjunction with multiple context groups
                if (firstChunk <= _lastCompletedChunk)
                {
                    // throw new Exception(ErrorMessage.Mat73Writer_ChunkAlreadyWritten);
                }

                // write data
                for (int i = 0; i < contextGroup.VariableContextSet.Count(); i++)
                {
                    this.WriteData(fileOffset, bufferOffset, length, contextGroup.VariableContextSet[i]);
                }

                // write last_completed_chunk
                IOHelper.WriteDataset(groupId, "last_completed_chunk", new double[] { lastChunk });
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }

                H5F.flush(_fileId, H5F.scope_t.GLOBAL);
            }
        }

        protected override void FreeUnmanagedResources()
        {
            base.FreeUnmanagedResources();

            if (H5I.is_valid(_fileId) > 0) { this.CloseHdfFile(_fileId); }
        }

        private void OpenFile(string filePath, DateTime startDateTime, IList<VariableContext> variableContextSet)
        {
            long propertyId = -1;
            long datasetId = -1;
            long groupId = -1;

            bool isNew;

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
                        catch
                        {
                            //
                        }
                    }
                }

                // create file
                if (_fileId < 0)
                {
                    propertyId = H5P.create(H5P.FILE_CREATE);
                    H5P.set_userblock(propertyId, USERBLOCK_SIZE);
                    _fileId = H5F.create(filePath, H5F.ACC_TRUNC, propertyId);
                }

                if (_fileId < 0)
                    throw new Exception($"{ ErrorMessage.Mat73Writer_CouldNotOpenOrCreateFile } File: { filePath }.");

                // prepare channels
                foreach (VariableContext variableContext in variableContextSet)
                {
                    var periodInSeconds = (ulong)_settings.FileGranularity;
                    var samplesPerSecond = variableContext.VariableDescription.SampleRate.SamplesPerSecond;
                    (var chunkLength, var chunkCount) = GeneralHelper.CalculateChunkParameters(periodInSeconds, samplesPerSecond);

                    this.PrepareVariable(_fileId, variableContext.VariableDescription, chunkLength, chunkCount);
                }

                // info
                groupId = this.OpenOrCreateStruct(_fileId, "/info").GroupId;

                (datasetId, isNew) = this.OpenOrCreateVariable(groupId, "last_completed_chunk", 1, 1);

                if (isNew)
                    IOHelper.Write(datasetId, new double[] { -1 }, DataContainerType.Dataset);

                // text entries
                _textEntrySet[2].Content = startDateTime.ToString("yyyy-MM-ddTHH-mm-ss");

                this.PrepareAllTextEntries(_textEntrySet);
            }
            finally
            {
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(propertyId) > 0) { H5P.close(propertyId); }
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }

                H5F.flush(_fileId, H5F.scope_t.GLOBAL);

                // write preamble
                if (H5I.is_valid(_fileId) > 0)
                {
                    H5F.close(_fileId);

                    this.WritePreamble(filePath);
                }
            }

            _fileId = H5F.open(filePath, H5F.ACC_RDWR);
        }

        private unsafe void WriteData(ulong fileOffset, ulong bufferOffset, ulong length, VariableContext variableContext)
        {
            Contract.Requires(variableContext != null, nameof(variableContext));

            long groupId = -1;
            long datasetId = -1;
            long dataspaceId = -1;
            long dataspaceId_Buffer = -1;

            try
            {
                groupId = H5G.open(_fileId, $"/{ variableContext.VariableDescription.VariableName }");

                var datasetName = $"dataset_{ variableContext.VariableDescription.DatasetName.Replace(" ", "_") }";
                datasetId = H5D.open(groupId, datasetName);
                dataspaceId = H5D.get_space(datasetId);
                dataspaceId_Buffer = H5S.create_simple(1, new ulong[] { length }, null);

                var simpleBuffers = variableContext.Buffer.ToSimpleBuffer();

                // dataset
                H5S.select_hyperslab(dataspaceId,
                                    H5S.seloper_t.SET,
                                    new ulong[] { fileOffset },
                                    new ulong[] { 1 },
                                    new ulong[] { 1 },
                                    new ulong[] { length });

                var offset = (int)bufferOffset * simpleBuffers.ElementSize;
                var buffer = simpleBuffers.RawBuffer[offset..];

                fixed (byte* bufferPtr = buffer)
                {
                    if (H5D.write(datasetId, H5T.NATIVE_DOUBLE, dataspaceId_Buffer, dataspaceId, H5P.DEFAULT, new IntPtr(bufferPtr)) < 0)
                        throw new Exception(ErrorMessage.Mat73Writer_CouldNotWriteChunk_Dataset);
                }
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                if (H5I.is_valid(dataspaceId_Buffer) > 0) { H5S.close(dataspaceId_Buffer); }
            }
        }

        private void PrepareVariable(long locationId, VariableDescription variableDescription, ulong chunkLength, ulong chunkCount)
        {
            long groupId = -1;
            long datasetId = -1;

            try
            {
                if (chunkLength <= 0)
                    throw new Exception(ErrorMessage.Mat73Writer_SampleRateTooLow);

                // struct
                groupId = this.OpenOrCreateStruct(locationId, variableDescription.VariableName).GroupId;

                datasetId = this.OpenOrCreateVariable(groupId, $"dataset_{ variableDescription.DatasetName.Replace(" ", "_") }", chunkLength, chunkCount).DatasetId;
            }
            finally
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
            }
        }

        private ulong ToChunkIndex(ulong offset, SampleRateContainer sampleRate)
        {
            var length = _chunkPeriod.TotalSeconds * (double)sampleRate.SamplesPerSecond;
            return (ulong)(offset / length);
        }

        private void CloseHdfFile(long fileId)
        {
            if (H5I.is_valid(fileId) > 0) { H5F.close(fileId); }
        }

        // low level
        private (long DatasetId, bool IsNew) OpenOrCreateVariable(long locationId, string name, ulong chunkLength, ulong chunkCount)
        {
            long datasetId = -1;
            GCHandle gcHandle_fillValue = default;
            bool isNew;

            try
            {
                var fillValue = Double.NaN;
                gcHandle_fillValue = GCHandle.Alloc(fillValue, GCHandleType.Pinned);

                (datasetId, isNew) = IOHelper.OpenOrCreateDataset(locationId, name, H5T.NATIVE_DOUBLE, chunkLength, chunkCount, gcHandle_fillValue.AddrOfPinnedObject());

                this.PrepareStringAttribute(datasetId, "MATLAB_class", this.GetMatTypeFromType(typeof(double)));
            }
            catch (Exception)
            {
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                
                throw;
            }
            finally
            {
                if (gcHandle_fillValue.IsAllocated)
                    gcHandle_fillValue.Free();
            }

            return (datasetId, isNew);
        }

        private (long GroupId, bool IsNew) OpenOrCreateStruct(long locationId, string path)
        {
            long groupId = -1;
            bool isNew;

            try
            {
                (groupId, isNew) = IOHelper.OpenOrCreateGroup(locationId, path);

                this.PrepareStringAttribute(groupId, "MATLAB_class", "struct");
            }
            catch (Exception)
            {
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                
                throw;
            }

            return (groupId, isNew);
        }

        private void PrepareStringAttribute(long locationId, string name, string value)
        {
            long typeId = -1;
            long attributeId = -1;

            bool isNew;

            try
            {
                var classNamePtr = Marshal.StringToHGlobalAnsi(value);

                typeId = H5T.copy(H5T.C_S1);
                H5T.set_size(typeId, new IntPtr(value.Length));

                (attributeId, isNew) = IOHelper.OpenOrCreateAttribute(locationId, name, typeId, () =>
                {
                    long dataspaceId = -1;
                    long localAttributeId = -1;

                    try
                    {
                        dataspaceId = H5S.create(H5S.class_t.SCALAR);
                        localAttributeId = H5A.create(locationId, name, typeId, dataspaceId);
                    }
                    finally
                    {
                        if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                    }

                    return localAttributeId;
                });

                if (isNew)
                    H5A.write(attributeId, typeId, classNamePtr);
            }
            finally
            {
                if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                if (H5I.is_valid(attributeId) > 0) { H5A.close(attributeId); }
            }
        }

        private void PrepareInt32Attribute(long locationId, string name, Int32 value)
        {
            long attributeId = -1;
            bool isNew;

            try
            {
                (attributeId, isNew) = IOHelper.OpenOrCreateAttribute(locationId, name, H5T.NATIVE_INT32, 1, new ulong[] { 1 });

                if (isNew)
                    IOHelper.Write(attributeId, new Int32[] { value }, DataContainerType.Attribute);
            }
            finally
            {
                if (H5I.is_valid(attributeId) > 0) { H5A.close(attributeId); }
            }
        }

        private string GetMatTypeFromType(Type type)
        {
            if (type == typeof(Double))
                return "double";
            else if (type == typeof(char))
                return "char";
            else if (type == typeof(string))
                return "cell";
            else
                throw new NotImplementedException();
        }

        // low level -> preamble

        private void WritePreamble(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write))
            {
                var streamData1 = Encoding.ASCII.GetBytes($"MATLAB 7.3 MAT-file, Platform: PCWIN64, Created on: { DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture) } HDF5 schema 1.00 .                     ");
                var streamData2 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x49, 0x4D };
                var streamData3 = new byte[512 - streamData1.Length - streamData2.Length];

                fileStream.Write(streamData1, 0, streamData1.Length);
                fileStream.Write(streamData2, 0, streamData2.Length);
                fileStream.Write(streamData3, 0, streamData3.Length);
            }
        }

        // low level -> string
        private char GetRefsName(byte index)
        {
            if (index < 26)
                return (char)(index + 0x61);
            else if (index < 54)
                return (char)(index + 0x41);
            else if (index < 63)
                return (char)(index + 0x31);
            else if (index < 64)
                return (char)(index + 0x30);
            else
                throw new ArgumentException("argument 'index' must be < 64");
        }

        private void PrepareAllTextEntries(IList<TextEntry> textEntrySet)
        {
            var index = (byte)0;

            textEntrySet.ToList().ForEach(textEntry =>
            {
                long groupId = -1;
                bool isNew;

                try
                {
                    (groupId, isNew) = this.OpenOrCreateStruct(_fileId, textEntry.Path);
                    this.PrepareRefsCellString(textEntry, this.GetRefsName(index).ToString());
                    this.PrepareTextEntryCellString(textEntry, this.GetRefsName(index).ToString());
                }
                finally
                {
                    if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                }

                index++;
            });
        }

        private void PrepareRefsCellString(TextEntry textEntry, string refsEntryName)
        {
            long datasetId = -1;
            bool isNew;

            GCHandle gcHandle_data;

            gcHandle_data = default;

            try
            {
                var data = textEntry.Content.ToCodePoints().ToList().ConvertAll(value => (UInt16)value).ToArray();
                gcHandle_data = GCHandle.Alloc(data, GCHandleType.Pinned);

                (datasetId, isNew) = IOHelper.OpenOrCreateDataset(_fileId, $"/#refs#/{ refsEntryName }", H5T.NATIVE_UINT16, () =>
                {
                    long dataspaceId = -1;
                    long lcPropertyId = -1;

                    try
                    {
                        lcPropertyId = H5P.create(H5P.LINK_CREATE);
                        H5P.set_create_intermediate_group(lcPropertyId, 1);
                        dataspaceId = H5S.create_simple(2, new UInt64[] { (UInt64)data.Length, 1 }, null);
                        datasetId = H5D.create(_fileId, $"/#refs#/{ refsEntryName }", H5T.NATIVE_UINT16, dataspaceId, lcPropertyId, H5P.DEFAULT, H5P.DEFAULT);
                    }
                    catch
                    {
                        if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                        
                        throw;
                    }
                    finally
                    {
                        if (H5I.is_valid(lcPropertyId) > 0) { H5P.close(lcPropertyId); }
                        if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                    }

                    return datasetId;
                });

                if (isNew)
                    H5D.write(datasetId, H5T.NATIVE_UINT16, H5S.ALL, H5S.ALL, H5P.DEFAULT, gcHandle_data.AddrOfPinnedObject());

                this.PrepareStringAttribute(datasetId, "MATLAB_class", this.GetMatTypeFromType(typeof(char)));
                this.PrepareInt32Attribute(datasetId, "MATLAB_int_decode", 2);
            }
            finally
            {
                if (gcHandle_data.IsAllocated)
                    gcHandle_data.Free();

                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
            }
        }

        private void PrepareTextEntryCellString(TextEntry textEntry, string refsEntryName)
        {
            long datasetId = -1;
            bool isNew;

            var objectReferencePointer = IntPtr.Zero;

            try
            {
                objectReferencePointer = Marshal.AllocHGlobal(8);

                (datasetId, isNew) = IOHelper.OpenOrCreateDataset(_fileId, $"{ textEntry.Path }/{ textEntry.Name }", H5T.STD_REF_OBJ, () =>
                {
                    long dataspaceId = -1;

                    try
                    {
                        dataspaceId = H5S.create_simple(2, new UInt64[] { 1, 1 }, null);
                        datasetId = H5D.create(_fileId, $"{ textEntry.Path }/{ textEntry.Name }", H5T.STD_REF_OBJ, dataspaceId);
                    }
                    catch
                    {
                        if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                        
                        throw;
                    }
                    finally
                    {
                        if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                    }

                    return datasetId;
                });

                if (isNew)
                {
                    H5R.create(objectReferencePointer, _fileId, $"/#refs#/{ refsEntryName }", H5R.type_t.OBJECT, -1);
                    H5D.write(datasetId, H5T.STD_REF_OBJ, H5S.ALL, H5S.ALL, H5P.DEFAULT, objectReferencePointer);
                }

                this.PrepareStringAttribute(datasetId, "MATLAB_class", this.GetMatTypeFromType(typeof(string)));
            }
            finally
            {
                if (objectReferencePointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(objectReferencePointer);

                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
            }
        }

        //private void PrepareRefsTextEntryChar(TextEntry textEntry)
        //{
        //    long datasetId = -1;
        //    long groupId = -1;

        //    bool isNew;

        //    byte index;

        //    try
        //    {
        //        index = 0;

        //        (groupId, isNew) = IOHelper.OpenOrCreateGroup(_fileId, "#refs#");

        //        if (isNew)
        //        {
        //            (datasetId, isNew) = IOHelper.OpenOrCreateDataset(groupId, this.GetRefsName(index).ToString(), H5T.NATIVE_UINT64, () =>
        //            {
        //                long dataspaceId = -1;

        //                try
        //                {
        //                    dataspaceId = H5S.create_simple(2, new ulong[] { 6, 1 }, null);
        //                    datasetId = H5D.create(groupId, this.GetRefsName(index).ToString(), H5T.NATIVE_UINT64, dataspaceId);
        //                }
        //                catch (Exception)
        //                {
        //                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
        //                    
        //                    throw;
        //                }
        //                finally
        //                {
        //                    if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
        //                }
        //            });

        //            if (isNew)
        //            {
        //                IOHelper.Write(datasetId, new UInt64[] {  }, DataContainerType.Dataset);
        //            }
        //        }

        //private void PrepareTextEntryChar(TextEntry textEntry, byte index)
        //{
        //    long datasetId = -1;
        //    bool isNew;

        //    try
        //    {
        //        (datasetId, isNew) = IOHelper.OpenOrCreateDataset(_fileId, $"{ textEntry.Path }/{ textEntry.Name }", H5T.NATIVE_UINT32, () =>
        //        {
        //            long dataspaceId = -1;

        //            try
        //            {
        //                dataspaceId = H5S.create_simple(2, new UInt64[] { 1, 6 }, null);
        //                datasetId = H5D.create(_fileId, $"{ textEntry.Path }/{ textEntry.Name }", H5T.NATIVE_UINT32, dataspaceId);
        //            }
        //            catch (Exception)
        //            {
        //                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
        //                
        //                throw;
        //            }
        //            finally
        //            {
        //                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
        //            }

        //            return datasetId;
        //        });

        //        if (isNew)
        //        {
        //            IOHelper.Write(datasetId, new UInt32[] { 0xDD000000, 0x02, 0x01, 0x01, index, 0x01 }, DataContainerType.Dataset);
        //        }

        //        this.PrepareStringAttribute(datasetId, "MATLAB_class", this.GetMatTypeFromType(typeof(string)));
        //        this.PrepareInt32Attribute(datasetId, "MATLAB_object_decode", 3);
        //    }
        //    finally
        //    {
        //        if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
        //    }
        //}

        #endregion
    }
}