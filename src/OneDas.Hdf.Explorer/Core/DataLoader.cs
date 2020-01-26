using Microsoft.Extensions.Logging.Abstractions;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Extension.Csv;
using OneDas.Extension.Famos;
using OneDas.Extension.Mat73;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace OneDas.Hdf.Explorer.Core
{
    public class DataLoader
    {
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        private CancellationToken _cancellationToken;

        public DataLoader(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public bool WriteZipFileCampaignEntry(ZipArchive zipArchive, FileGranularity fileGranularity, FileFormat fileFormat, ZipSettings zipSettings)
        {
            var variableDescriptionSet = zipSettings.Campaign.ToVariableDescriptions();

            DataWriterExtensionSettingsBase settings;
            DataWriterExtensionLogicBase dataWriter;

            switch (fileFormat)
            {
                case FileFormat.CSV:

                    settings = new CsvSettings() { FileGranularity = fileGranularity };
                    dataWriter = new CsvWriter((CsvSettings)settings, NullLogger.Instance);

                    break;

                case FileFormat.FAMOS:

                    settings = new FamosSettings() { FileGranularity = fileGranularity };
                    dataWriter = new FamosWriter((FamosSettings)settings, NullLogger.Instance);

                    break;

                case FileFormat.MAT73:

                    settings = new Mat73Settings() { FileGranularity = fileGranularity };
                    dataWriter = new Mat73Writer((Mat73Settings)settings, NullLogger.Instance);

                    break;

                case FileFormat.CSV2:

                    settings = new CsvSettings() { FileGranularity = fileGranularity, RowIndexFormat = CsvRowIndexFormat.Unix };
                    dataWriter = new CsvWriter((CsvSettings)settings, NullLogger.Instance);

                    break;

                default:
                    throw new NotImplementedException();
            }

            // create temp directory
            var directoryPath = Path.Combine(Path.GetTempPath(), "OneDas.Hdf.Explorer", Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryPath);

            // create custom meta data
            var customMetadataEntrySet = new List<CustomMetadataEntry>();
            //customMetadataEntrySet.Add(new CustomMetadataEntry("system_name", "HDF Explorer", CustomMetadataEntryLevel.File));

            // initialize data writer
            var campaignName_splitted = zipSettings.Campaign.Name.Split('/');
            var dataWriterContext = new DataWriterContext("OneDAS Explorer", directoryPath, new OneDasCampaignDescription(Guid.Empty, 0, campaignName_splitted[1], campaignName_splitted[2], campaignName_splitted[3]), customMetadataEntrySet);
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
            var filePathSet = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            var currentFile = 0;
            var fileCount = filePathSet.Count();

            foreach (string filePath in filePathSet)
            {
                var zipArchiveEntry = zipArchive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Optimal);

                this.OnProgressUpdated(new ProgressUpdatedEventArgs(currentFile / (double)fileCount * 100, $"Writing file {currentFile + 1} / {fileCount} to ZIP archive ..."));

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
            // START progress
            var datasetCount = (ulong)zipSettings.Campaign.Variables.SelectMany(variable => variable.Datasets).Count();
            var currentSegment = 0UL;
            var segmentCount = (ulong)Math.Ceiling(zipSettings.Block / (double)zipSettings.SegmentLength);
            // END progress

            var currentRowCount = zipSettings.SegmentLength;
            var lastRow = zipSettings.Start + zipSettings.Block;

            // for each segment
            for (ulong currentStart = zipSettings.Start; currentStart < lastRow; currentStart += currentRowCount)
            {
                var currentDataset = 0UL;
                var dataStorageSet = new List<IDataStorage>();

                if (currentStart + currentRowCount > zipSettings.Start + zipSettings.Block)
                    currentRowCount = zipSettings.Start + zipSettings.Block - currentStart;

                // load data
                foreach (var variable in zipSettings.Campaign.Variables)
                {
                    variable.Datasets.ForEach(dataset =>
                    {
                        var datasetPath = $"{zipSettings.Campaign.Name}/{variable.Name}/{dataset.Name}";

                        dataStorageSet.Add(zipSettings.DataReader.LoadDataset(datasetPath, currentStart, currentRowCount));
                        this.OnProgressUpdated(new ProgressUpdatedEventArgs((currentSegment * (double)datasetCount + currentDataset) / (segmentCount * datasetCount) * 100, $"Loading dataset segment {currentSegment * datasetCount + currentDataset + 1} / {segmentCount * datasetCount} ..."));
                        currentDataset++;
                    });
                }

                this.OnProgressUpdated(new ProgressUpdatedEventArgs((currentSegment * (double)datasetCount + currentDataset) / (segmentCount * datasetCount) * 100, $"Writing data of segment {currentSegment + 1} / {segmentCount} ..."));

                var dateTime = zipSettings.DateTimeBegin.AddSeconds((currentStart - zipSettings.Start) / zipSettings.SampleRate);
                var dataStoragePeriod = TimeSpan.FromSeconds(currentRowCount / zipSettings.SampleRate);

                dataWriter.Write(dateTime, dataStoragePeriod, dataStorageSet);

                // clean up
                dataStorageSet = null;
                GC.Collect();

                if (_cancellationToken.IsCancellationRequested)
                    return false;

                currentSegment++;
            }

            return true;
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
            this.ProgressUpdated?.Invoke(this, e);
        }
    }
}
