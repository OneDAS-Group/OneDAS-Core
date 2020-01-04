using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Extension.Hdf;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.Import
{
    public class Importer
    {
        #region Fields

        string _databaseDirectoryPath;

        ILogger _logger;
        IDataReader _dataReader;

        #endregion

        #region Constructors

        public Importer(string databaseDirectoryPath, IDataReader dataReader, ILogger logger)
        {
            OneDasUtilities.ValidateDatabaseFolderPath(databaseDirectoryPath);

            _databaseDirectoryPath = databaseDirectoryPath;
            _dataReader = dataReader;
            _logger = logger;
        }

        #endregion

        #region Methods

        public void ConvertFiles(string campaignName, int version, string fileNameFormat, uint periodPerFile, int days, string systemName, bool convertToDouble)
        {
            var sourceDirectoryPath = Path.Combine(_databaseDirectoryPath, "DB_ORIGIN", $"{campaignName.Replace('/', '_')}_V{version}");
            var dateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            var dateTimeBegin = dateTimeEnd.AddDays(-days);

            // convert
            for (int i = 0; i <= days; i++)
            {
                dateTimeBegin = dateTimeBegin.AddDays(1);

                var currentSourceDirectoryPath = Path.Combine(sourceDirectoryPath, dateTimeBegin.ToString("yyyy-MM"));
                var currentTargetDirectoryPath = Path.Combine(_databaseDirectoryPath, "DB_IMPORT", dateTimeBegin.ToString("yyyy-MM"));
                var currentTargetFileName = $"{campaignName.Replace('/', '_')}_V{version}_{dateTimeBegin.ToString("yyyy-MM-ddTHH-mm-ssZ")}.h5";
                var currentTargetFilePath = Path.Combine(currentTargetDirectoryPath, currentTargetFileName);

                if (File.Exists(currentTargetFilePath))
                {
                    _logger.LogInformation($"Target file '{currentTargetFilePath}' already exists.");
                }
                else if (!Directory.Exists(currentSourceDirectoryPath) || !Directory.EnumerateFileSystemEntries(currentSourceDirectoryPath).Any())
                {
                    _logger.LogInformation($"Source folder '{currentSourceDirectoryPath}' does not exist or is empty.");
                }
                else
                {
                    Directory.CreateDirectory(currentTargetDirectoryPath);

                    this.InternalConvertFiles(currentSourceDirectoryPath, currentTargetDirectoryPath,
                                              dateTimeBegin, campaignName, version, fileNameFormat, 
                                              TimeSpan.FromSeconds(periodPerFile), systemName, 
                                              convertToDouble);
                }
            }
        }

        private void InternalConvertFiles(string sourceDirectoryPath, string targetDirectoryPath, DateTime dateTimeBegin, string campaignName, int version, string fileNameFormat, TimeSpan periodPerFile, string systemName, bool convertToDouble)
        {
            var firstFilePath = Directory.EnumerateFiles(sourceDirectoryPath).FirstOrDefault();
            var variableDescriptionSet = _dataReader.GetVariableDescriptions(firstFilePath);
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
                var sourceFilePath = Path.Combine(sourceDirectoryPath, fileName);

                currentOffset += periodPerFile;

                if (!File.Exists(sourceFilePath))
                {
                    _logger.LogWarning($"Source file '{sourceFilePath}' does not exist.");
                    continue;
                }

                var message = $"Processing file '{sourceFilePath}' ... ";

                try
                {
                    _logger.LogInformation(message);

                    var dataStorageSet = _dataReader.GetData(sourceFilePath, variableDescriptionSet, convertToDouble);

                    // check actual file size
                    for (int i = 0; i < variableDescriptionSet.Count(); i++)
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

                    _logger.LogInformation($"{message}Done.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{message}Failed. Reason: {ex.Message}");
                }
            }
        }

        #endregion
    }
}
