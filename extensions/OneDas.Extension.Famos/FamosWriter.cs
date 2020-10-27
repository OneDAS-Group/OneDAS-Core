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

        protected override void OnPrepareFile(DateTime startDateTime, List<ChannelContextGroup> channelContextGroupSet)
        {
            _dataFilePath = Path.Combine(this.DataWriterContext.DataDirectoryPath, $"{this.DataWriterContext.ProjectDescription.PrimaryGroupName}_{this.DataWriterContext.ProjectDescription.SecondaryGroupName}_{this.DataWriterContext.ProjectDescription.ProjectName}_V{ this.DataWriterContext.ProjectDescription.Version}_{startDateTime.ToString("yyyy-MM-ddTHH-mm-ss")}Z.dat");

            if (_famosFile != null)
                _famosFile.Dispose();

            this.OpenFile(_dataFilePath, startDateTime, channelContextGroupSet);
        }

        protected override void OnWrite(ChannelContextGroup contextGroup, ulong fileOffset, ulong bufferOffset, ulong length)
        {
            if (length <= 0)
                throw new Exception(ErrorMessage.FamosWriter_SampleRateTooLow);

            var simpleBuffers = contextGroup.ChannelContextSet.Select(channelContext => channelContext.Buffer.ToSimpleBuffer()).ToList();

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

        private void OpenFile(string dataFilePath, DateTime startDateTime, List<ChannelContextGroup> channelContextGroupSet)
        {
            if (File.Exists(dataFilePath))
                throw new Exception($"The file {dataFilePath} already exists. Extending an already existing file with additional channels is not supported.");

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

            // file -> project
            var projectGroup = new FamosFileGroup($"{this.DataWriterContext.ProjectDescription.PrimaryGroupName} / {this.DataWriterContext.ProjectDescription.SecondaryGroupName} / {this.DataWriterContext.ProjectDescription.ProjectName}");

            projectGroup.PropertyInfo = new FamosFilePropertyInfo(new List<FamosFileProperty>()
            {
                new FamosFileProperty("project_version", this.DataWriterContext.ProjectDescription.Version)
            });

            foreach (var customMetadataEntry in this.DataWriterContext.CustomMetadataEntrySet.Where(customMetadataEntry => customMetadataEntry.CustomMetadataEntryLevel == CustomMetadataEntryLevel.Project))
            {
                projectGroup.PropertyInfo.Properties.Add(new FamosFileProperty(customMetadataEntry.Key, customMetadataEntry.Value));
            }

            famosFile.Groups.Add(projectGroup);

            // for each context group
            foreach (var contextGroup in channelContextGroupSet)
            {
                var totalSeconds = (int)Math.Round(_settings.FilePeriod.TotalSeconds, MidpointRounding.AwayFromZero);
                var totalLength = (int)(totalSeconds * contextGroup.SampleRate.SamplesPerSecond);

                if (totalLength * (double)OneDasUtilities.SizeOf(OneDasDataType.FLOAT64) > 2 * Math.Pow(10, 9))
                    throw new Exception(ErrorMessage.FamosWriter_DataSizeExceedsLimit);

                // file -> project -> channels
                var field = new FamosFileField(FamosFileFieldType.MultipleYToSingleEquidistantTime);

                foreach (ChannelContext channelContext in contextGroup.ChannelContextSet)
                {
                    var dx = contextGroup.SampleRate.Period.TotalSeconds;
                    var channel = this.PrepareChannel(field, channelContext.ChannelDescription, (int)totalLength, startDateTime, dx);

                    projectGroup.Channels.Add(channel);
                }

                famosFile.Fields.Add(field);
                _spdToFieldIndexMap[contextGroup.SampleRate.SamplesPerDay] = famosFile.Fields.Count - 1;
            }

            //
            famosFile.Save(dataFilePath, _ => { });
            _famosFile = FamosFile.OpenEditable(dataFilePath);
        }

        private FamosFileChannel PrepareChannel(FamosFileField field, ChannelDescription channelDescription, int totalLength, DateTime startDateTme, double dx)
        {
            // component 
            var datasetName = $"{channelDescription.ChannelName}_{channelDescription.DatasetName.Replace(" ", "_")}";
            var calibration = new FamosFileCalibration(false, 1, 0, false, channelDescription.Unit);

            var component = new FamosFileAnalogComponent(datasetName, FamosFileDataType.Float64, totalLength, calibration)
            {
                XAxisScaling = new FamosFileXAxisScaling((decimal)dx) { Unit = "s" },
                TriggerTime = new FamosFileTriggerTime(startDateTme, FamosFileTimeMode.Unknown),
            };

            // attributes
            var channel = component.Channels.First();

            channel.PropertyInfo = new FamosFilePropertyInfo(new List<FamosFileProperty>()
            {
                new FamosFileProperty("name", channelDescription.ChannelName),
                new FamosFileProperty("group", channelDescription.Group),
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
