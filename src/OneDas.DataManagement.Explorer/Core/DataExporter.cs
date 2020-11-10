using Microsoft.Extensions.Logging.Abstractions;
using OneDas.Buffers;
using OneDas.DataManagement.Infrastructure;
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

namespace OneDas.DataManagement.Explorer.Core
{
    public class DataExporter : IDisposable
    {
        #region Events

        public event EventHandler<ProgressUpdatedEventArgs> Progress;

        #endregion

        #region Fields

        private ulong _blockSizeLimit;
        private string _zipFilePath;
        private ZipArchive _zipArchive;
        private ExportParameters _parameters;

        #endregion

        #region Constructors

        public DataExporter(string zipFilePath, ExportParameters parameters, ulong blockSizeLimit)
        {
            _zipFilePath = zipFilePath;
            _parameters = parameters;
            _blockSizeLimit = blockSizeLimit;
        }

        #endregion

        #region Methods

        public bool WriteZipFileProjectEntry(ProjectSettings projectSettings, CancellationToken cancellationToken)
        {
            if (_zipArchive == null)
                _zipArchive = ZipFile.Open(_zipFilePath, ZipArchiveMode.Create);

            var channelDescriptionSet = projectSettings.Project.ToChannelDescriptions();
            var singleFile = _parameters.FileGranularity == FileGranularity.SingleFile;

            TimeSpan filePeriod;

            if (singleFile)
                filePeriod = _parameters.End - _parameters.Begin;
            else
                filePeriod = TimeSpan.FromSeconds((int)_parameters.FileGranularity);

            DataWriterExtensionSettingsBase settings;
            DataWriterExtensionLogicBase dataWriter;

            switch (_parameters.FileFormat)
            {
                case FileFormat.CSV:

                    settings = new CsvSettings() 
                    {
                        FilePeriod = filePeriod,
                        SingleFile = singleFile,
                        RowIndexFormat = _parameters.CsvRowIndexFormat,
                        SignificantFigures = _parameters.CsvSignificantFigures,
                    };

                    dataWriter = new CsvWriter((CsvSettings)settings, NullLogger.Instance);

                    break;

                case FileFormat.FAMOS:

                    settings = new FamosSettings() 
                    {
                        FilePeriod = filePeriod,
                        SingleFile = singleFile,
                    };

                    dataWriter = new FamosWriter((FamosSettings)settings, NullLogger.Instance);

                    break;

                case FileFormat.MAT73:

                    settings = new Mat73Settings()
                    {
                        FilePeriod = filePeriod,
                        SingleFile = singleFile,
                    };

                    dataWriter = new Mat73Writer((Mat73Settings)settings, NullLogger.Instance);

                    break;

                default:
                    throw new NotImplementedException();
            }

            // create temp directory
            var directoryPath = Path.Combine(Path.GetTempPath(), "OneDas.DataManagement.Explorer", Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryPath);

            // create custom meta data
            var customMetadataEntrySet = new List<CustomMetadataEntry>();
            //customMetadataEntrySet.Add(new CustomMetadataEntry("system_name", "HDF Explorer", CustomMetadataEntryLevel.File));

            // initialize data writer
            var projectName_splitted = projectSettings.Project.Id.Split('/');
            var dataWriterContext = new DataWriterContext("OneDAS Explorer", directoryPath, new OneDasProjectDescription(Guid.Empty, 0, projectName_splitted[1], projectName_splitted[2], projectName_splitted[3]), customMetadataEntrySet);
            dataWriter.Configure(dataWriterContext, channelDescriptionSet);

            // create temp files
            try
            {
                if (!this.CreateFiles(dataWriter, projectSettings, cancellationToken))
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
                if (cancellationToken.IsCancellationRequested)
                    return false;

                var zipArchiveEntry = _zipArchive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Optimal);

                this.OnProgressUpdated(new ProgressUpdatedEventArgs(currentFile / (double)fileCount, $"Writing file {currentFile + 1} / {fileCount} to ZIP archive ..."));

                using var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                using var zipArchiveEntryStream = zipArchiveEntry.Open();

                fileStream.CopyTo(zipArchiveEntryStream);
                currentFile++;
            }

            this.CleanUp(directoryPath);

            return true;
        }

        private bool CreateFiles(DataWriterExtensionLogicBase dataWriter, ProjectSettings projectSettings, CancellationToken cancellationToken)
        {
#warning: To handle native and aggregated datasets individually will probably lead to strange effects for data writers. Check this.

            var datasets = projectSettings.Project.Channels.SelectMany(channel => channel.Datasets);
            var nativeDatasets = datasets.Where(dataset => dataset.IsNative).ToList();
            var aggregatedDatasets = datasets.Where(dataset => !dataset.IsNative).ToList();
            var currentMode = string.Empty;

            var progressHandler = (EventHandler<double>)((sender, e) =>
            {
                this.OnProgressUpdated(new ProgressUpdatedEventArgs(e, $"Loading {currentMode} data ..."));
            });

            // native
            currentMode = "native";

            if (nativeDatasets.Any())
            {
                var reader = projectSettings.NativeDataReader;
                reader.Progress.ProgressChanged += progressHandler;

                try
                {
                    foreach (var progressRecord in reader.Read(nativeDatasets, _parameters.Begin, _parameters.End, _blockSizeLimit, cancellationToken))
                    {
                        this.ProcessData(dataWriter, progressRecord);
                    }
                }
                finally
                {
                    reader.Progress.ProgressChanged -= progressHandler;
                }
            }

            // aggregated
            currentMode = "aggregated";

            if (aggregatedDatasets.Any())
            {
                var reader = projectSettings.AggregationDataReader;
                reader.Progress.ProgressChanged += progressHandler;

                try
                {
                    foreach (var progressRecord in reader.Read(aggregatedDatasets, _parameters.Begin, _parameters.End, _blockSizeLimit, cancellationToken))
                    {
                        this.ProcessData(dataWriter, progressRecord);
                    }
                }
                finally
                {
                    reader.Progress.ProgressChanged -= progressHandler;
                }
            }

            // cancellation
            if (cancellationToken.IsCancellationRequested)
                return false;

            return true;
        }

        private void ProcessData(DataWriterExtensionLogicBase dataWriter, DataReaderProgressRecord progressRecord)
        {
            var buffers = progressRecord.DatasetToRecordMap.Values.Select(dataRecord =>
            {
                double[] data;

                if (progressRecord.DatasetToRecordMap.Keys.First().IsNative)
                    data = BufferUtilities.ApplyDatasetStatus2(dataRecord.Dataset, dataRecord.Status);
                else
                    data = (double[])dataRecord.Dataset;

                return (IBuffer)BufferUtilities.CreateSimpleBuffer(data);
            }).ToList();

            var period = progressRecord.End - progressRecord.Begin;
            dataWriter.Write(progressRecord.Begin, period, buffers);

            // clean up
            buffers = null;
            GC.Collect();
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
            this.Progress?.Invoke(this, e);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _zipArchive.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
