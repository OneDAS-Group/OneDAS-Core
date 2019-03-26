using HDF.PInvoke;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace OneDas.Hdf.Explorer.Core
{
    public class HdfDataLoader
    {
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        private CancellationToken _cancellationToken;

        public HdfDataLoader(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public bool WriteZipFileCampaignEntry(ZipArchive zipArchive, FileGranularity fileGranularity, FileFormat fileFormat, ZipSettings zipSettings)
        {
            IList<VariableDescription> variableDescriptionSet;
            IList<CustomMetadataEntry> customMetadataEntrySet;

            ZipArchiveEntry zipArchiveEntry;
            DataWriterExtensionLogicBase dataWriter;
            DataWriterExtensionSettingsBase settings;
            DataWriterContext dataWriterContext;

            string directoryPath;
            string[] campaignName_splitted;
            string[] filePathSet;

            int currentFile;
            int fileCount;

            // build variable descriptions
            variableDescriptionSet = new List<VariableDescription>();

            zipSettings.CampaignInfo.Value.ToList().ForEach(variableInfo =>
            {
                variableInfo.Value.ForEach(datasetName =>
                {
                    long groupId = -1;
                    long typeId = -1;
                    long datasetId = -1;

                    string displayName;
                    string groupName;
                    string unit;

                    ulong samplesPerDay;

                    OneDasDataType oneDasDataType;

                    hdf_transfer_function_t[] hdf_transfer_function_t_set;
                    List<TransferFunction> transferFunctionSet;

                    try
                    {
                        groupId = H5G.open(zipSettings.SourceFileId, $"{ zipSettings.CampaignInfo.Key }/{ variableInfo.Key }");
                        datasetId = H5D.open(groupId, datasetName);
                        typeId = H5D.get_type(datasetId);

                        displayName = IOHelper.ReadAttribute<string>(groupId, "name_set").Last();
                        groupName = IOHelper.ReadAttribute<string>(groupId, "group_set").Last();
                        unit = IOHelper.ReadAttribute<string>(groupId, "unit_set").LastOrDefault();
                        hdf_transfer_function_t_set = IOHelper.ReadAttribute<hdf_transfer_function_t>(groupId, "transfer_function_set");
                        transferFunctionSet = hdf_transfer_function_t_set.Select(tf => new TransferFunction(DateTime.ParseExact(tf.date_time, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture), tf.type, tf.option, tf.argument)).ToList();

                        oneDasDataType = OneDasUtilities.GetOneDasDataTypeFromType(TypeConversionHelper.GetTypeFromHdfTypeId(typeId));
                        samplesPerDay = OneDasUtilities.GetSamplesPerDayFromString(datasetName);

                        variableDescriptionSet.Add(new VariableDescription(new Guid(variableInfo.Key), displayName, datasetName, groupName, oneDasDataType, samplesPerDay, unit, transferFunctionSet, typeof(ISimpleDataStorage)));
                    }
                    finally
                    {
                        if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                        if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                        if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                    }
                });
            });

            dataWriter = null; // REMOVE ONE IMPLEMENTED PROPERLY

            switch (fileFormat)
            {
                case FileFormat.CSV:

                    //settings = new CsvSettings() { FileGranularity = fileGranularity };
                    //dataWriter = new CsvWriter((CsvSettings)settings, new LoggerFactory());

                    break;

                case FileFormat.GAM:

                    //settings = new GamSettings() { FileGranularity = fileGranularity };
                    //dataWriter = new GamWriter((GamSettings)settings, new LoggerFactory());

                    break;

                case FileFormat.MAT73:

                    //settings = new Mat73Settings() { FileGranularity = fileGranularity };
                    //dataWriter = new Mat73Writer((Mat73Settings)settings, new LoggerFactory());

                    break;

                default:
                    throw new NotImplementedException();
            }

            // create temp directory
            directoryPath = Path.Combine(Path.GetTempPath(), "OneDas.Hdf.Explorer", Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryPath);

            // create custom meta data
            customMetadataEntrySet = new List<CustomMetadataEntry>();
            //customMetadataEntrySet.Add(new CustomMetadataEntry("system_name", "HDF Explorer", CustomMetadataEntryLevel.File));

            // initialize data writer
            campaignName_splitted = zipSettings.CampaignInfo.Key.Split('/');
            dataWriterContext = new DataWriterContext("HDF Explorer", directoryPath, new OneDasCampaignDescription(Guid.Empty, 0, campaignName_splitted[1], campaignName_splitted[2], campaignName_splitted[3]), customMetadataEntrySet);
            dataWriter.Configure(dataWriterContext, variableDescriptionSet);

            // create temp files
            try
            {
                if (!this.CreateFiles(dataWriter, zipSettings))
                {
                    this.CleanUp(directoryPath);

                    return false;
                }
            }
            finally
            {
                dataWriter.Dispose();
            }

            // write zip archive entries
            filePathSet = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            currentFile = 0;
            fileCount = filePathSet.Count();

            foreach (string filePath in filePathSet)
            {
                zipArchiveEntry = zipArchive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Optimal);

                this.OnProgressUpdated(new ProgressUpdatedEventArgs(currentFile / (double)fileCount * 100, $"Writing file { currentFile + 1 } / { fileCount } to ZIP archive ..."));

                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (Stream zipArchiveEntryStream = zipArchiveEntry.Open())
                    {
                        fileStream.CopyTo(zipArchiveEntryStream);
                    }
                }

                currentFile++;
            }

            this.CleanUp(directoryPath);

            return true;
        }

        private bool CreateFiles(DataWriterExtensionLogicBase dataWriter, ZipSettings zipSettings)
        {
            DateTime dateTime;
            TimeSpan dataStoragePeriod;

            ulong currentRowCount;
            ulong lastRow;

            // START progress
            ulong currentDataset;
            ulong datasetCount;
            ulong currentSegment;
            ulong segmentCount;

            datasetCount = (ulong)zipSettings.CampaignInfo.Value.SelectMany(variableInfo => variableInfo.Value).Count();
            currentSegment = 0;
            segmentCount = (ulong)Math.Ceiling(zipSettings.Block / (double)zipSettings.SegmentLength);
            // END progress

            currentRowCount = zipSettings.SegmentLength;
            lastRow = zipSettings.Start + zipSettings.Block;

            IList<IDataStorage> dataStorageSet;

            // for each segment
            for (ulong currentStart = zipSettings.Start; currentStart < lastRow; currentStart += currentRowCount)
            {
                currentDataset = 0;
                dataStorageSet = new List<IDataStorage>();

                if (currentStart + currentRowCount > zipSettings.Start + zipSettings.Block)
                {
                    currentRowCount = zipSettings.Start + zipSettings.Block - currentStart;
                }

                // load data
                foreach (var variableInfo in zipSettings.CampaignInfo.Value)
                {
                    variableInfo.Value.ForEach(datasetName =>
                    {
                        dataStorageSet.Add(this.LoadDataset(zipSettings.SourceFileId, $"{ zipSettings.CampaignInfo.Key }/{ variableInfo.Key }/{ datasetName }", currentStart, 1, currentRowCount, 1));
                        this.OnProgressUpdated(new ProgressUpdatedEventArgs((currentSegment * (double)datasetCount + currentDataset) / (segmentCount * datasetCount) * 100, $"Loading dataset segment { currentSegment * datasetCount + currentDataset + 1 } / { segmentCount * datasetCount } ..."));
                        currentDataset++;
                    });
                }

                this.OnProgressUpdated(new ProgressUpdatedEventArgs((currentSegment * (double)datasetCount + currentDataset) / (segmentCount * datasetCount) * 100, $"Writing data of segment { currentSegment + 1 } / { segmentCount } ..."));

                dateTime = zipSettings.DateTimeBegin.AddSeconds((currentStart - zipSettings.Start) / zipSettings.SampleRate);
                dataStoragePeriod = TimeSpan.FromSeconds(currentRowCount / zipSettings.SampleRate);

                dataWriter.Write(dateTime, dataStoragePeriod, dataStorageSet);

                // clean up
                dataStorageSet = null;
                GC.Collect();

                if (_cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                currentSegment++;
            }

            return true;
        }

        private ISimpleDataStorage LoadDataset(long sourceFileId, string datasetPath, ulong start, ulong stride, ulong block, ulong count)
        {
            long datasetId = -1;
            long typeId = -1;

            Array dataset;
            Array dataset_status;

            Type genericType;

            ExtendedDataStorageBase extendedDataStorage;
            ISimpleDataStorage simpleDataStorage;

            dataset = IOHelper.ReadDataset(sourceFileId, datasetPath, start, stride, block, count);

            // apply status (only if native dataset)
            if (H5L.exists(sourceFileId, datasetPath + "_status") > 0)
            {
                try
                {
                    datasetId = H5D.open(sourceFileId, datasetPath);
                    typeId = H5D.get_type(datasetId);

                    dataset_status = IOHelper.ReadDataset(sourceFileId, datasetPath + "_status", start, stride, block, count).Cast<byte>().ToArray();

                    genericType = typeof(ExtendedDataStorage<>).MakeGenericType(TypeConversionHelper.GetTypeFromHdfTypeId(typeId));
                    extendedDataStorage = (ExtendedDataStorageBase)Activator.CreateInstance(genericType, dataset, dataset_status);

                    dataset_status = null;
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                    if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                }

                simpleDataStorage = extendedDataStorage.ToSimpleDataStorage();
                extendedDataStorage.Dispose();

                return simpleDataStorage;
            }
            else
            {
                return new SimpleDataStorage(dataset.Cast<double>().ToArray());
            }
        }

        private void CleanUp(string directoryPath)
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch
            {
                //
            }
        }

        protected virtual void OnProgressUpdated(ProgressUpdatedEventArgs e)
        {
            ProgressUpdated?.Invoke(this, e);
        }
    }
}
