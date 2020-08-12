using ImcFamosFile;
using Microsoft.Extensions.Logging;
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

        private string _dataFilePath;
        private FamosFile _famosFile;
        private FamosSettings _settings;
        private Dictionary<ulong, int> _spdToFieldIndexMap;

        #endregion
        
        #region "Constructors"

        public FamosWriter(FamosSettings settings, ILogger logger) : base(settings, logger)
        {
            _settings = settings;

            _spdToFieldIndexMap = new Dictionary<ulong, int>();
        }

        #endregion

        #region "Methods"

        protected override void OnPrepareFile(DateTime startDateTime, List<VariableContextGroup> variableContextGroupSet)
        {
            _dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{this.DataWriterContext.CampaignDescription.PrimaryGroupName}_{this.DataWriterContext.CampaignDescription.SecondaryGroupName}_{this.DataWriterContext.CampaignDescription.CampaignName}_V{ this.DataWriterContext.CampaignDescription.Version}_{startDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z.dat");

            if (_famosFile != null)
                _famosFile.Dispose();

            this.OpenFile(_dataFilePath, startDateTime, variableContextGroupSet);
        }

        protected override void OnWrite(VariableContextGroup contextGroup, ulong fileOffset, ulong bufferOffset, ulong length)
        {
            if (length <= 0)
                throw new Exception(ErrorMessage.FamosWriter_SampleRateTooLow);

            var simpleBuffers = contextGroup.VariableContextSet.Select(variableContext => variableContext.Buffer.ToSimpleBuffer()).ToList();

            var fieldIndex = _spdToFieldIndexMap[contextGroup.SampleRate.SamplesPerDay];
            var field = _famosFile.Fields[fieldIndex];

            _famosFile.Edit(writer =>
            {
                for (int i = 0; i < simpleBuffers.Count; i++)
                {
                    var component = field.Components[i];
                    var data = simpleBuffers[i].Buffer.Slice((int)bufferOffset, (int)length);

                    _famosFile.WriteSingle(writer, component, (int)fileOffset, data);
                }
            });
        }

        private void OpenFile(string dataFilePath, DateTime startDateTime, List<VariableContextGroup> variableContextGroupSet)
        {
            if (File.Exists(dataFilePath))
                throw new Exception($"The file {dataFilePath} already exists. Extending an already existing file with additional variables is not supported.");

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

            // for each context group
            foreach (var contextGroup in variableContextGroupSet)
            {
                var totalSeconds = (int)Math.Round(_settings.FilePeriod.TotalSeconds, MidpointRounding.AwayFromZero);
                var totalLength = (int)(totalSeconds * contextGroup.SampleRate.SamplesPerSecond);

                if (totalLength * (double)OneDasUtilities.SizeOf(OneDasDataType.FLOAT64) > 2 * Math.Pow(10, 9))
                    throw new Exception(ErrorMessage.FamosWriter_DataSizeExceedsLimit);

                // file -> campaign -> channels
                var field = new FamosFileField(FamosFileFieldType.MultipleYToSingleEquidistantTime);

                foreach (VariableContext variableContext in contextGroup.VariableContextSet)
                {
                    var dx = contextGroup.SampleRate.Period.TotalSeconds;
                    var variable = this.PrepareVariable(field, variableContext.VariableDescription, (int)totalLength, startDateTime, dx);

                    campaignGroup.Channels.Add(variable);
                }

                famosFile.Fields.Add(field);
                _spdToFieldIndexMap[contextGroup.SampleRate.SamplesPerDay] = famosFile.Fields.Count - 1;
            }

            //
            famosFile.Save(dataFilePath, _ => { });
            _famosFile = FamosFile.OpenEditable(dataFilePath);
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

        protected override void FreeManagedResources()
        {
            base.FreeManagedResources();

            _famosFile?.Dispose();
        }

        #endregion
    }
}
