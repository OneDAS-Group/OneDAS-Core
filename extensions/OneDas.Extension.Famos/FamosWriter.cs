using ImcFamosFile;
using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.Extension.Famos
{
    // v1: initial version
    [DataWriterFormatVersion(1)]
    public class FamosWriter : DataWriterExtensionLogicBase
    {
        #region "Fields"

        private FamosSettings _settings;

        private DateTime _lastFileStartDateTime;

        #endregion

        #region "Constructors"

        public FamosWriter(FamosSettings settings, ILoggerFactory loggerFactory) : base(settings, loggerFactory)
        {
            _settings = settings;
        }

        #endregion

        #region "Methods"

        protected override void OnPrepareFile(DateTime startDateTime, ulong samplesPerDay, IList<VariableContext> variableContextSet)
        {
            string dataFilePath;

            _lastFileStartDateTime = startDateTime;
            dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{this.DataWriterContext.CampaignDescription.PrimaryGroupName}_{this.DataWriterContext.CampaignDescription.SecondaryGroupName}_{this.DataWriterContext.CampaignDescription.CampaignName}_V{ this.DataWriterContext.CampaignDescription.Version}_{startDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z_{ samplesPerDay }_samples_per_day.dat");

            if (!File.Exists(dataFilePath))
            {
                var famosFile = new FamosFileHeader();

                // file
                var metadataGroup = new FamosFileGroup("Metadata");

                metadataGroup.PropertyInfo = new FamosFilePropertyInfo(new List<FamosFileProperty>()
                {
                    new FamosFileProperty("format_version", this.FormatVersion),
                    new FamosFileProperty("system_name", this.DataWriterContext.SystemName),
                    new FamosFileProperty("date_time", startDateTime),
                });

                foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.File))
                {
                    metadataGroup.PropertyInfo.Properties.Add(new FamosFileProperty(customMetadataEntry.Key, customMetadataEntry.Value));
                }

                famosFile.Groups.Add(metadataGroup);

                // file -> campaign
                var campaignGroup = new FamosFileGroup($"{this.DataWriterContext.CampaignDescription.PrimaryGroupName} / {this.DataWriterContext.CampaignDescription.SecondaryGroupName} / {this.DataWriterContext.CampaignDescription.CampaignName}");

                campaignGroup.PropertyInfo = new FamosFilePropertyInfo(new List<FamosFileProperty>()
                {
                    new FamosFileProperty("campaign_version", this.DataWriterContext.CampaignDescription.Version)
                });

                foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.Campaign))
                {
                    campaignGroup.PropertyInfo.Properties.Add(new FamosFileProperty(customMetadataEntry.Key, customMetadataEntry.Value));
                }

                famosFile.Groups.Add(campaignGroup);

                // chunk length
                var chunkLength = this.TimeSpanToIndex(this.ChunkPeriod, variableContextSet.First().VariableDescription.SamplesPerDay);

                if (chunkLength <= 0)
                    throw new Exception(ErrorMessage.FamosWriter_SampleRateTooLow);

                var totalLength = chunkLength * this.ChunkCount;

                if (totalLength * (double)OneDasUtilities.SizeOf(OneDasDataType.FLOAT64) > 2 * Math.Pow(10, 9))
                    throw new Exception(ErrorMessage.FamosWriter_DataSizeExceedsLimit);

                // file -> campaign -> channels
                var field = new FamosFileField(FamosFileFieldType.MultipleYToSingleEquidistantTime);

                foreach (VariableContext variableContext in variableContextSet)
                {
                    var dx = 1.0 / (samplesPerDay / 86400);
                    var variable = this.PrepareVariable(field, variableContext.VariableDescription, (int)totalLength, startDateTime, dx);

                    campaignGroup.Channels.Add(variable);
                }

                famosFile.Fields.Add(field);

                //
                famosFile.Save(dataFilePath, _ => { });
            }
        }

        protected override void OnWrite(ulong samplesPerDay, ulong fileOffset, ulong dataStorageOffset, ulong length, IList<VariableContext> variableContextSet)
        {
            string dataFilePath;
            IList<ISimpleDataStorage> simpleDataStorageSet;

            dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{ this.DataWriterContext.CampaignDescription.PrimaryGroupName }_{ this.DataWriterContext.CampaignDescription.SecondaryGroupName }_{ this.DataWriterContext.CampaignDescription.CampaignName }_V{ this.DataWriterContext.CampaignDescription.Version }_{ _lastFileStartDateTime.ToString("yyyy-MM-ddTHH-mm-ss") }Z_{ samplesPerDay }_samples_per_day.dat");

            if (length <= 0)
                throw new Exception(ErrorMessage.FamosWriter_SampleRateTooLow);

            simpleDataStorageSet = variableContextSet.Select(variableContext => variableContext.DataStorage.ToSimpleDataStorage()).ToList();

            using (var famosFile = FamosFile.OpenEditable(dataFilePath))
            {
                var field = famosFile.Fields.First();

                famosFile.Edit(writer =>
                {
                    for (int i = 0; i < simpleDataStorageSet.Count; i++)
                    {
                        var component = field.Components[i];
                        var data = simpleDataStorageSet[i].DataBuffer.Slice((int)dataStorageOffset, (int)length);

                        famosFile.WriteSingle(writer, component, (int)fileOffset, data);
                    }
                });
            }
        }

        private FamosFileChannel PrepareVariable(FamosFileField field, VariableDescription variableDescription, int totalLength, DateTime startDateTme, double dx)
        {
            // component 
            var datasetName = $"{variableDescription.VariableName}_{variableDescription.DatasetName.Replace(" ", "_")}";
            var calibration = new FamosFileCalibration(false, 1, 0, false, variableDescription.Unit);

            var component = new FamosFileAnalogComponent(datasetName, FamosFileDataType.Float64, totalLength, calibration)
            {
                XAxisScaling = new FamosFileXAxisScaling((decimal)dx) { Unit = "s" },
                TriggerTime = new FamosFileTriggerTime(startDateTme, FamosFileTimeMode.Unknown),
            };

            // attributes
            var channel = component.Channels.First();

            channel.PropertyInfo = new FamosFilePropertyInfo(new List<FamosFileProperty>()
            {
                new FamosFileProperty("name", variableDescription.VariableName),
                new FamosFileProperty("group", variableDescription.Group),
                new FamosFileProperty("comment", "yyyy-MM-ddTHH-mm-ssZ: Comment1"),
            });

            field.Components.Add(component);

            return channel;
        }

        #endregion
    }
}
