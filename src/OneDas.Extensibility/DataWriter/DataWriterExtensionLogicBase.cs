using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extensibility
{
    public abstract class DataWriterExtensionLogicBase : ExtensionLogicBase
    {
        #region "Fields"

        private DateTime _lastFileStartDateTime;
        private DateTime _lastWrittenDateTime;

        private ILogger _logger;
        private IList<VariableDescription> _variableDescriptionSet;

        #endregion

        #region "Constructors"

        public DataWriterExtensionLogicBase(DataWriterExtensionSettingsBase settings, ILoggerFactory loggerFactory) : base(settings)
        {
            this.Settings = settings;
            this.ChunkPeriod = TimeSpan.FromMinutes(1);
            this.ChunkCount = (ulong)((int)settings.FileGranularity / this.ChunkPeriod.TotalSeconds);
            this.FormatVersion = this.GetType().GetFirstAttribute<DataWriterFormatVersionAttribute>().FormatVersion;

            _logger = loggerFactory.CreateLogger(this.DisplayName);
        }

        #endregion

        #region "Properties"

        public new DataWriterExtensionSettingsBase Settings { get; private set; }

        public int FormatVersion { get; private set; }

        protected DataWriterContext DataWriterContext { get; private set; }
        protected TimeSpan ChunkPeriod { get; private set; }
        protected ulong ChunkCount { get; private set; }

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
            DateTime currentDateTime;
            DateTime fileStartDateTime;
            TimeSpan dataStorageOffset;

            TimeSpan filePeriod;
            TimeSpan fileOffset;

            TimeSpan remainingFilePeriod;
            TimeSpan remainingDataStoragePeriod;

            TimeSpan period;

            List<VariableContext> variableContextSet;
            Dictionary<ulong, List<VariableContext>> sampleRateToVariableContextMap;

            ulong actualFileOffset;
            ulong actualDataStorageOffset;
            ulong actualPeriod;
            ulong firstChunk;
            ulong lastChunk;

            if (dateTime < _lastWrittenDateTime)
            {
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeAlreadyWritten);
            }

            if (dateTime != dateTime.RoundDown(this.ChunkPeriod))
            {
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeGranularityTooHigh);
            }

            if (dataStoragePeriod.Seconds > 0 || dataStoragePeriod.Milliseconds > 0)
            {
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeGranularityTooHigh);
            }

            dataStorageOffset = TimeSpan.Zero;
            filePeriod = TimeSpan.FromSeconds((int)this.Settings.FileGranularity);
            variableContextSet = _variableDescriptionSet.Zip(dataStorageSet, (variableDescription, dataStorage) => new VariableContext(variableDescription, dataStorage)).ToList();
            sampleRateToVariableContextMap = variableContextSet.GroupBy(variableContext => variableContext.VariableDescription.SamplesPerDay).ToDictionary(group => group.Key, group => group.ToList());

            while (dataStorageOffset < dataStoragePeriod)
            {
                currentDateTime = dateTime + dataStorageOffset;
                fileStartDateTime = currentDateTime.RoundDown(filePeriod);
                fileOffset = currentDateTime - fileStartDateTime;

                remainingFilePeriod = filePeriod - fileOffset;
                remainingDataStoragePeriod = dataStoragePeriod - dataStorageOffset;

                period = new TimeSpan(Math.Min(remainingFilePeriod.Ticks, remainingDataStoragePeriod.Ticks));              

                // check if file must be created or updated
                if (fileStartDateTime != _lastFileStartDateTime)
                {
                    foreach (var entry in sampleRateToVariableContextMap)
                    {
                        this.OnPrepareFile(fileStartDateTime, entry.Key, entry.Value);
                    }

                    _lastFileStartDateTime = fileStartDateTime;
                }

                // write data
                foreach (var entry in sampleRateToVariableContextMap)
                {
                    actualFileOffset = this.TimeSpanToIndex(fileOffset, entry.Key);
                    actualDataStorageOffset = this.TimeSpanToIndex(dataStorageOffset, entry.Key);
                    actualPeriod = this.TimeSpanToIndex(period, entry.Key);

                    this.OnWrite(
                        entry.Key,
                        actualFileOffset,
                        actualDataStorageOffset,
                        actualPeriod,
                        entry.Value
                    );

                    // message
                    firstChunk = this.ToChunkIndex(actualFileOffset, entry.Key);
                    lastChunk = this.ToChunkIndex(actualFileOffset + actualPeriod, entry.Key) - 1;

                    if (firstChunk == lastChunk)
                    {
                        _logger.LogInformation($"chunk { firstChunk + 1 } of { this.ChunkCount } written to file");
                    }
                    else
                    {
                        _logger.LogInformation($"chunks { firstChunk + 1 }-{ lastChunk + 1 } of { this.ChunkCount } written to file");
                    }
                }

                dataStorageOffset += period;
            }

            _lastWrittenDateTime = dateTime + dataStoragePeriod;
        }

        protected ulong TimeSpanToIndex(TimeSpan timeSpan, ulong samplesPerDay)
        {
            return (ulong)(timeSpan.TotalSeconds * samplesPerDay / 86400);
        }

        protected ulong ToChunkIndex(ulong offset, ulong samplesPerDay)
        {
            return offset / this.TimeSpanToIndex(this.ChunkPeriod, samplesPerDay);
        }

        protected virtual void OnConfigure()
        {
            //
        }

        protected abstract void OnPrepareFile(DateTime startDateTime, ulong samplesPerDay, IList<VariableContext> variableContextSet);

        protected abstract void OnWrite(ulong samplesPerDay, ulong fileOffset, ulong dataStorageOffset, ulong length, IList<VariableContext> variableContextSet);

        #endregion
    }
}