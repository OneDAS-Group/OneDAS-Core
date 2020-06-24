using Microsoft.Extensions.Logging;
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

        private double _unixStart;
        private DateTime _unixEpoch;
        private DateTime _lastFileStartDateTime;
        private NumberFormatInfo _nfi;

        #endregion

        #region "Constructors"

        public CsvWriter(CsvSettings settings, ILogger logger) : base(settings, logger)
        {
            _settings = settings;

            _unixEpoch = new DateTime(1970, 01, 01);

            _nfi = new NumberFormatInfo()
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = string.Empty
            };
        }
        
        #endregion

        #region "Methods"

        protected override void OnPrepareFile(DateTime startDateTime, List<VariableContextGroup> variableContextGroupSet)
        {
            _lastFileStartDateTime = startDateTime;
            _unixStart = (startDateTime - _unixEpoch).TotalSeconds;

            foreach (var contextGroup in variableContextGroupSet)
            {
                var dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{this.DataWriterContext.CampaignDescription.PrimaryGroupName}_{this.DataWriterContext.CampaignDescription.SecondaryGroupName}_{this.DataWriterContext.CampaignDescription.CampaignName}_V{this.DataWriterContext.CampaignDescription.Version}_{startDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z_{contextGroup.SampleRate.SamplesPerDay}_samples_per_day.csv");

                if (!File.Exists(dataFilePath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(File.Open(dataFilePath, FileMode.Append, FileAccess.Write)))
                    {
                        // comment
                        streamWriter.WriteLine($"# format_version: { this.FormatVersion };");
                        streamWriter.WriteLine($"# system_name: { this.DataWriterContext.SystemName };");
                        streamWriter.WriteLine($"# date_time: { startDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ") };");
                        streamWriter.WriteLine($"# samples_per_day: { contextGroup.SampleRate.SamplesPerDay };");

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

                        /* variable name */
                        switch (_settings.RowIndexFormat)
                        {
                            case CsvRowIndexFormat.Index:
                                streamWriter.Write("index;");
                                break;
                            case CsvRowIndexFormat.Unix:
                                streamWriter.Write("unix time;");
                                break;
                            default:
                                throw new NotSupportedException($"The row index format '{_settings.RowIndexFormat}' is not supported.");
                        }

                        foreach (var variableContext in contextGroup.VariableContextSet)
                        {
                            streamWriter.Write($"{ variableContext.VariableDescription.VariableName };");
                        }

                        streamWriter.WriteLine();

                        /* dataset name */
                        streamWriter.Write("-;");

                        foreach (var variableContext in contextGroup.VariableContextSet)
                        {
                            streamWriter.Write($"{ variableContext.VariableDescription.DatasetName };");
                        }

                        streamWriter.WriteLine();

                        /* unit */
                        streamWriter.Write("-;");

                        foreach (var variableContext in contextGroup.VariableContextSet)
                        {
                            streamWriter.Write($"{ variableContext.VariableDescription.Unit };");
                        }

                        streamWriter.WriteLine();
                    }
                }
            }
        }

        protected override void OnWrite(VariableContextGroup contextGroup, ulong fileOffset, ulong bufferOffset, ulong length)
        {
            var campaignDescription = this.DataWriterContext.CampaignDescription;
            var dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{campaignDescription.PrimaryGroupName}_{campaignDescription.SecondaryGroupName}_{campaignDescription.CampaignName}_V{campaignDescription.Version }_{_lastFileStartDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z_{contextGroup.SampleRate.SamplesPerDay}_samples_per_day.csv");

            if (length <= 0)
                throw new Exception(ErrorMessage.CsvWriter_SampleRateTooLow);

            var simpleBuffers = contextGroup.VariableContextSet.Select(variableContext => variableContext.Buffer.ToSimpleBuffer()).ToList();

            using (StreamWriter streamWriter = new StreamWriter(File.Open(dataFilePath, FileMode.Append, FileAccess.Write)))
            {
                for (ulong rowIndex = 0; rowIndex < length; rowIndex++)
                {
                    switch (_settings.RowIndexFormat)
                    {
                        case CsvRowIndexFormat.Index:
                            streamWriter.Write($"{string.Format(_nfi, "{0:N0}", fileOffset + rowIndex)};");
                            break;
                        case CsvRowIndexFormat.Unix:
                            streamWriter.Write($"{string.Format(_nfi, "{0:N5}", _unixStart + (double)((fileOffset + rowIndex) / contextGroup.SampleRate.SamplesPerSecond))};");
                            break;
                        default:
                            throw new NotSupportedException($"The row index format '{_settings.RowIndexFormat}' is not supported.");
                    }

                    for (int i = 0; i < simpleBuffers.Count; i++)
                    {
                        var value = simpleBuffers[i].Buffer[(int)(bufferOffset + rowIndex)];
                        streamWriter.Write($"{string.Format(_nfi, $"{{0:G{_settings.SignificantPlaces}}}", value)};");
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        #endregion
    }
}
