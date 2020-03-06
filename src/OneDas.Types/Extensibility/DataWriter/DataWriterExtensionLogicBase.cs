using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extensibility
{
#warning Add "CheckFileSize" method.
#warning Write variable description instead of comment?
    public abstract class DataWriterExtensionLogicBase : ExtensionLogicBase
    {
        #region "Fields"

        private DateTime _lastFileStartDateTime;
        private DateTime _lastWrittenDateTime;

        private IList<VariableDescription> _variableDescriptionSet;

        #endregion

        #region "Constructors"

        public DataWriterExtensionLogicBase(DataWriterExtensionSettingsBase settings, ILogger logger)
            : base(settings, logger)
        {
            this.Settings = settings;
            this.ChunkPeriod = TimeSpan.FromMinutes(1);
            this.ChunkCount = (ulong)((int)settings.FileGranularity / this.ChunkPeriod.TotalSeconds);
            this.FormatVersion = this.GetType().GetFirstAttribute<DataWriterFormatVersionAttribute>().FormatVersion;
        }

        #endregion

        #region "Properties"

        public new DataWriterExtensionSettingsBase Settings { get; }

        public int FormatVersion { get; }

        protected DataWriterContext DataWriterContext { get; private set; }

        protected TimeSpan ChunkPeriod { get; }

        protected ulong ChunkCount { get; }

        #endregion

        #region "Methods"

        public void Configure(DataWriterContext dataWriterContext, IList<VariableDescription> variableDescriptionSet)
        {
            this.DataWriterContext = dataWriterContext;

            _variableDescriptionSet = variableDescriptionSet;

            this.OnConfigure();
        }

        public void Write(DateTime dateTime, TimeSpan dataStoragePeriod, IList<IDataStorage> dataStorageSet)
        {
            if (dateTime < _lastWrittenDateTime)
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeAlreadyWritten);

            if (dateTime != dateTime.RoundDown(this.ChunkPeriod))
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeGranularityTooHigh);

            if (dataStoragePeriod.Seconds > 0 || dataStoragePeriod.Milliseconds > 0)
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeGranularityTooHigh);

            var dataStorageOffset = TimeSpan.Zero;
            var filePeriod = TimeSpan.FromSeconds((int)this.Settings.FileGranularity);
            var variableContextSet = _variableDescriptionSet.Zip(dataStorageSet, (variableDescription, dataStorage) => new VariableContext(variableDescription, dataStorage)).ToList();
            var variableContextGroupSet = variableContextSet
                            .GroupBy(variableContext => variableContext.VariableDescription.SampleRate.SamplesPerDay)
                            .Select(group => new VariableContextGroup(new SampleRateContainer(group.Key), group.ToList())).ToList();

            while (dataStorageOffset < dataStoragePeriod)
            {
                var currentDateTime = dateTime + dataStorageOffset;
                var fileStartDateTime = currentDateTime.RoundDown(filePeriod);
                var fileOffset = currentDateTime - fileStartDateTime;

                var remainingFilePeriod = filePeriod - fileOffset;
                var remainingDataStoragePeriod = dataStoragePeriod - dataStorageOffset;

                var period = new TimeSpan(Math.Min(remainingFilePeriod.Ticks, remainingDataStoragePeriod.Ticks));

                // check if file must be created or updated
                if (fileStartDateTime != _lastFileStartDateTime)
                {
                    this.OnPrepareFile(fileStartDateTime, variableContextGroupSet);

                    _lastFileStartDateTime = fileStartDateTime;
                }

                // write data
                foreach (var contextGroup in variableContextGroupSet)
                {
                    var sampleRate = contextGroup.SampleRate;

                    var actualFileOffset = this.TimeSpanToIndex(fileOffset, sampleRate);
                    var actualDataStorageOffset = this.TimeSpanToIndex(dataStorageOffset, sampleRate);
                    var actualPeriod = this.TimeSpanToIndex(period, sampleRate);

                    this.OnWrite(
                        contextGroup,
                        actualFileOffset,
                        actualDataStorageOffset,
                        actualPeriod
                    );

                    // message
                    var firstChunk = this.ToChunkIndex(actualFileOffset, sampleRate);
                    var lastChunk = this.ToChunkIndex(actualFileOffset + actualPeriod, sampleRate) - 1;

                    if (firstChunk == lastChunk)
                        this.Logger.LogInformation($"chunk { firstChunk + 1 } of { this.ChunkCount } written to file");
                    else
                        this.Logger.LogInformation($"chunks { firstChunk + 1 }-{ lastChunk + 1 } of { this.ChunkCount } written to file");
                }

                dataStorageOffset += period;
            }

            _lastWrittenDateTime = dateTime + dataStoragePeriod;
        }

        protected ulong TimeSpanToIndex(TimeSpan timeSpan, SampleRateContainer samplesRate)
        {
            return (ulong)(this.TimeSpanToIndexDouble(timeSpan, samplesRate));
        }

#warning This method is required since downloading 600 s average data causes an index value < 1, which in turn causes a division by zero in the function "ToChunkIndex". Check if this still holds when sample time mechanisms were revised.
        protected double TimeSpanToIndexDouble(TimeSpan timeSpan, SampleRateContainer samplesRate)
        {
            return timeSpan.TotalSeconds * (double)samplesRate.SamplesPerSecond;
        }

        protected ulong ToChunkIndex(ulong offset, SampleRateContainer sampleRate)
        {
            return (ulong)(offset / this.TimeSpanToIndexDouble(this.ChunkPeriod, sampleRate));
        }

        protected virtual void OnConfigure()
        {
            //
        }

        protected abstract void OnPrepareFile(DateTime startDateTime, List<VariableContextGroup> variableContextGroupSet);

        protected abstract void OnWrite(VariableContextGroup contextGroup, ulong fileOffset, ulong dataStorageOffset, ulong length);

        #endregion
    }
}