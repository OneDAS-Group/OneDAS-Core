using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Plugin
{
    public abstract class DataWriterPluginLogicBase : PluginLogicBase
    {
        #region "Fields"

        private DateTime _lastFileStartDateTime;
        private DateTime _lastWrittenDateTime;

        private Dictionary<ulong, List<int>> _samplesPerDayToIndexMap;
        private Dictionary<ulong, List<VariableContext>> _sampleRateToVariableContextMap;

        private ILogger _logger;
        private object samplesPerDayCategory;

        #endregion

        #region "Constructors"

        public DataWriterPluginLogicBase(DataWriterPluginSettingsBase settings, ILoggerFactory loggerFactory) : base(settings)
        {
            this.Settings = settings;
            this.ChunkPeriod = TimeSpan.FromMinutes(1);
            this.ChunkCount = (ulong)((int)settings.FileGranularity / this.ChunkPeriod.TotalSeconds);
            this.FormatVersion = this.GetType().GetFirstAttribute<DataWriterFormatVersionAttribute>().FormatVersion;

            _logger = loggerFactory.CreateLogger(this.DisplayName);
        }

        #endregion

        #region "Properties"

        public new DataWriterPluginSettingsBase Settings { get; private set; }

        public int FormatVersion { get; private set; }

        protected DataWriterContext DataWriterContext { get; private set; }
        protected TimeSpan ChunkPeriod { get; private set; }
        protected ulong ChunkCount { get; private set; }

        #endregion

        #region "Methods"

        public void Initialize(DateTime dateTime, DataWriterContext dataWriterContext, IList<VariableContext> variableContextSet)
        {
            DateTime fileStartDateTime;

            this.DataWriterContext = dataWriterContext;

            fileStartDateTime = dateTime.RoundDown(new TimeSpan(0, 0, (int)this.Settings.FileGranularity));

            // samples per day dictionary
            _samplesPerDayToIndexMap = new Dictionary<ulong, List<int>>();

            for (int i = 0; i < variableContextSet.Count(); i++)
            {
                ulong samplesPerDay;

                samplesPerDay = variableContextSet[i].SamplesPerDay;

                if (!_samplesPerDayToIndexMap.ContainsKey(samplesPerDay))
                {
                    _samplesPerDayToIndexMap[samplesPerDay] = new List<int>();
                }

                _samplesPerDayToIndexMap[samplesPerDay].Add(i);
            }

            _sampleRateToVariableContextMap = _samplesPerDayToIndexMap.ToDictionary(entry => entry.Key, entry => entry.Value.Select(index => variableContextSet[index]).ToList());

            this.OnInitialize();
            this.PrepareFile(fileStartDateTime);
        }

        public void Write(DateTime dateTime, TimeSpan dataStoragePeriod, int dataStorageIndex)
        {
            DateTime currentDateTime;
            DateTime fileStartDateTime;
            TimeSpan dataStorageOffset;

            TimeSpan filePeriod;
            TimeSpan fileOffset;

            TimeSpan remainingFilePeriod;
            TimeSpan remainingDataStoragePeriod;

            TimeSpan period;

            ulong actualFileOffset;
            ulong actualDataStorageOffset;
            ulong actualPeriod;
            ulong firstChunk;
            ulong lastChunk;

            if (dateTime < _lastWrittenDateTime)
            {
                throw new ArgumentException(ErrorMessage.DataWriterPluginLogicBase_DateTimeAlreadyWritten);
            }

            if (dateTime != dateTime.RoundDown(this.ChunkPeriod))
            {
                throw new ArgumentException(ErrorMessage.DataWriterPluginLogicBase_DateTimeGranularityTooHigh);
            }

            if (dataStoragePeriod.Seconds > 0 || dataStoragePeriod.Milliseconds > 0)
            {
                throw new ArgumentException(ErrorMessage.DataWriterPluginLogicBase_DateTimeGranularityTooHigh);
            }

            dataStorageOffset = TimeSpan.Zero;
            filePeriod = TimeSpan.FromSeconds((int)this.Settings.FileGranularity);

            while (true)
            {
                currentDateTime = dateTime + dataStorageOffset;
                fileStartDateTime = currentDateTime.RoundDown(filePeriod);
                fileOffset = currentDateTime - fileStartDateTime;

                remainingFilePeriod = filePeriod - fileOffset;
                remainingDataStoragePeriod = dataStoragePeriod - dataStorageOffset;

                period = new TimeSpan(Math.Min(remainingFilePeriod.Ticks, remainingDataStoragePeriod.Ticks));

                this.PrepareFile(fileStartDateTime);

                foreach (var entry in _samplesPerDayToIndexMap)
                {
                    actualFileOffset = this.TimeSpanToIndex(fileOffset, entry.Key);
                    actualDataStorageOffset = this.TimeSpanToIndex(dataStorageOffset, entry.Key);
                    actualPeriod = this.TimeSpanToIndex(period, entry.Key);

                    this.OnWrite(
                        entry.Key,
                        actualFileOffset,
                        actualDataStorageOffset,
                        actualPeriod,
                        _sampleRateToVariableContextMap[entry.Key],
                        dataStorageIndex
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

                // 
                dataStorageOffset = dataStorageOffset + period;

                if (dataStorageOffset >= dataStoragePeriod)
                {
                    break;
                }
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

        protected virtual void OnInitialize()
        {
            //
        }

        /// <summary>
        /// Should be called during initialization to handle new variables). 
        /// Should also be called during IDataWriterPlugin.Write to handle creation of new / next file.
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="enforceInitialization"></param>
        protected abstract void OnPrepareFile(DateTime startDateTime, ulong samplesPerDay, IList<VariableContext> variableContextSet);

        protected abstract void OnWrite(ulong samplesPerDay, ulong fileOffset, ulong dataStorageOffset, ulong length, IList<VariableContext> variableContextSet, int dataStorageIndex);

        private void PrepareFile(DateTime fileStartDateTime)
        {
            if (fileStartDateTime != _lastFileStartDateTime)
            {
                foreach (var sampleRateCategory in _samplesPerDayToIndexMap)
                {
                    this.OnPrepareFile(fileStartDateTime, sampleRateCategory.Key, _sampleRateToVariableContextMap[sampleRateCategory.Key]);
                }

                _lastFileStartDateTime = fileStartDateTime;
            }
        }

        #endregion

        /*
        Initialize()
            OnInitialize()
            PrepareFile(true) -> OnPrepareFile()
        
        Write()
            PrepareFile(false) -> OnPrepareFile()
            OnWrite()
          
        */
    }
}