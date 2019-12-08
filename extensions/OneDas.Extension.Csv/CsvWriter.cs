using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OneDas.Extension.Csv
{
    // v1: initial version
    [DataWriterFormatVersion(1)]
    public class CsvWriter : DataWriterExtensionLogicBase
    {
        #region "Fields"

        private CsvSettings _settings;
        private NumberFormatInfo _numberFormatInfo;

        private DateTime _lastFileStartDateTime;

        #endregion

        #region "Constructors"

        public CsvWriter(CsvSettings settings, ILoggerFactory loggerFactory) : base(settings, loggerFactory)
        {
            _settings = settings;

            _numberFormatInfo = new NumberFormatInfo();

            _numberFormatInfo.NumberDecimalSeparator = ".";
            _numberFormatInfo.NumberDecimalDigits = 2;
            _numberFormatInfo.NumberGroupSeparator = string.Empty;
        }

        #endregion

        #region "Methods"

        protected override void OnPrepareFile(DateTime startDateTime, ulong samplesPerDay, IList<VariableContext> variableContextSet)
        {
            string dataFilePath;

            _lastFileStartDateTime = startDateTime;
            dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{ this.DataWriterContext.CampaignDescription.PrimaryGroupName }_{ this.DataWriterContext.CampaignDescription.SecondaryGroupName }_{ this.DataWriterContext.CampaignDescription.CampaignName }_V{ this.DataWriterContext.CampaignDescription.Version }_{ startDateTime.ToString("yyyy-MM-ddTHH-mm-ss") }Z_{ samplesPerDay }_samples_per_day.csv");

            if (!File.Exists(dataFilePath))
            {
                using (StreamWriter streamWriter = new StreamWriter(File.Open(dataFilePath, FileMode.Append, FileAccess.Write)))
                {
                    // comment
                    streamWriter.WriteLine($"# format_version: { this.FormatVersion };");
                    streamWriter.WriteLine($"# system_name: { this.DataWriterContext.SystemName };");
                    streamWriter.WriteLine($"# date_time: { startDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ") };");
                    streamWriter.WriteLine($"# samples_per_day: { samplesPerDay };");

                    foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.File))
                    {
                        streamWriter.WriteLine($"# { customMetadataEntry.Key }: { customMetadataEntry.Value };");
                    }

                    streamWriter.WriteLine($"# campaign_first_level: { this.DataWriterContext.CampaignDescription.PrimaryGroupName };");
                    streamWriter.WriteLine($"# campaign_second_level: { this.DataWriterContext.CampaignDescription.SecondaryGroupName };");
                    streamWriter.WriteLine($"# campaign_name: { this.DataWriterContext.CampaignDescription.CampaignName };");
                    streamWriter.WriteLine($"# campaign_version: { this.DataWriterContext.CampaignDescription.Version };");

                    foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.Campaign))
                    {
                        streamWriter.WriteLine($"# { customMetadataEntry.Key }: { customMetadataEntry.Value };");
                    }

                    // header
                    streamWriter.Write("index;");

                    foreach (VariableContext variableContext in variableContextSet)
                    {
                        streamWriter.Write($"{ variableContext.VariableDescription.VariableName };");
                    }

                    streamWriter.WriteLine();
                    streamWriter.Write("-;");

                    foreach (VariableContext variableContext in variableContextSet)
                    {
                        streamWriter.Write($"{ variableContext.VariableDescription.DatasetName };");
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        protected override void OnWrite(ulong samplesPerDay, ulong fileOffset, ulong dataStorageOffset, ulong length, IList<VariableContext> variableContextSet)
        {
            string dataFilePath;
            IList<ISimpleDataStorage> simpleDataStorageSet;

            dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{ this.DataWriterContext.CampaignDescription.PrimaryGroupName }_{ this.DataWriterContext.CampaignDescription.SecondaryGroupName }_{ this.DataWriterContext.CampaignDescription.CampaignName }_V{ this.DataWriterContext.CampaignDescription.Version }_{ _lastFileStartDateTime.ToString("yyyy-MM-ddTHH-mm-ss") }Z_{ samplesPerDay }_samples_per_day.csv");

            if (length <= 0)
                throw new Exception(ErrorMessage.CsvWriter_SampleRateTooLow);

            simpleDataStorageSet = variableContextSet.Select(variableContext => variableContext.DataStorage.ToSimpleDataStorage()).ToList();

            using (StreamWriter streamWriter = new StreamWriter(File.Open(dataFilePath, FileMode.Append, FileAccess.Write)))
            {
                for (ulong rowIndex = 0; rowIndex < length; rowIndex++)
                {
                    streamWriter.Write($"{ string.Format(_numberFormatInfo, "{0:N}", fileOffset + rowIndex) };");

                    for (int i = 0; i < simpleDataStorageSet.Count; i++)
                    {
                        object value;

                        value = simpleDataStorageSet[i].DataBuffer[(int)(dataStorageOffset + rowIndex)];
                        streamWriter.Write($"{ string.Format(_numberFormatInfo, "{0:N}", value) };");
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        #endregion
    }
}
