using Microsoft.Extensions.Logging;
using OneDas.Buffers;
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

        private DateTime _lastFileBegin;
        private DateTime _lastWrite;

        private IList<VariableDescription> _variableDescriptions;

        #endregion

        #region "Constructors"

        public DataWriterExtensionLogicBase(DataWriterExtensionSettingsBase settings, ILogger logger)
            : base(settings, logger)
        {
            this.Settings = settings;
            this.BasePeriod = TimeSpan.FromSeconds(1);
            this.FormatVersion = this.GetType().GetFirstAttribute<DataWriterFormatVersionAttribute>().FormatVersion;
        }

        #endregion

        #region "Properties"

        public new DataWriterExtensionSettingsBase Settings { get; }

        public int FormatVersion { get; }

        protected DataWriterContext DataWriterContext { get; private set; }

        protected TimeSpan BasePeriod { get; }

        #endregion

        #region "Methods"

        public void Configure(DataWriterContext dataWriterContext, IList<VariableDescription> variableDescriptions)
        {
            this.DataWriterContext = dataWriterContext;
            _variableDescriptions = variableDescriptions;

            this.OnConfigure();
        }

        public void Write(DateTime begin, TimeSpan bufferPeriod, IList<IBuffer> buffers)
        {
            if (begin < _lastWrite)
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeAlreadyWritten);

            if (begin != begin.RoundDown(this.BasePeriod))
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeGranularityTooHigh);

            if (bufferPeriod.Milliseconds > 0)
                throw new ArgumentException(ErrorMessage.DataWriterExtensionLogicBase_DateTimeGranularityTooHigh);

            var bufferOffset = TimeSpan.Zero;
            var filePeriod = TimeSpan.FromSeconds((int)this.Settings.FileGranularity);

            var variableContexts = _variableDescriptions
                .Zip(buffers, (variableDescription, buffer) => new VariableContext(variableDescription, buffer))
                .ToList();

            var variableContextGroups = variableContexts
                .GroupBy(variableContext => variableContext.VariableDescription.SampleRate)
                .Select(group => new VariableContextGroup(group.Key, group.ToList()))
                .ToList();

            while (bufferOffset < bufferPeriod)
            {
                var currentBegin = begin + bufferOffset;
                var fileBegin = currentBegin.RoundDown(filePeriod);
                var fileOffset = currentBegin - fileBegin;

                var remainingFilePeriod = filePeriod - fileOffset;
                var remainingBufferPeriod = bufferPeriod - bufferOffset;

                var period = new TimeSpan(Math.Min(remainingFilePeriod.Ticks, remainingBufferPeriod.Ticks));

                // ensure that file granularity is low enough for all sample rates
                foreach (var contextGroup in variableContextGroups)
                {
                    var sampleRate = contextGroup.SampleRate;
                    var length = (decimal)this.Settings.FileGranularity * sampleRate.SamplesPerSecond;

                    if (length < 1)
                        throw new Exception(ErrorMessage.DataWriterExtensionLogicBase_FileGranularityTooHigh);
                }

                // check if file must be created or updated
                if (fileBegin != _lastFileBegin)
                {
                    this.OnPrepareFile(fileBegin, variableContextGroups);

                    _lastFileBegin = fileBegin;
                }

                // write data
                foreach (var contextGroup in variableContextGroups)
                {
                    var sampleRate = contextGroup.SampleRate;

                    var actualFileOffset = this.TimeSpanToIndex(fileOffset, sampleRate);
                    var actualBufferOffset = this.TimeSpanToIndex(bufferOffset, sampleRate);
                    var actualPeriod = this.TimeSpanToIndex(period, sampleRate);

                    this.OnWrite(
                        contextGroup,
                        actualFileOffset,
                        actualBufferOffset,
                        actualPeriod
                    );

                    this.Logger.LogInformation($"data written to file");
                }

                bufferOffset += period;
            }

            _lastWrite = begin + bufferPeriod;
        }

        protected virtual void OnConfigure()
        {
            //
        }

        protected abstract void OnPrepareFile(DateTime startDateTime, List<VariableContextGroup> variableContextGroupSet);

        protected abstract void OnWrite(VariableContextGroup contextGroup, ulong fileOffset, ulong bufferOffset, ulong length);

        private ulong TimeSpanToIndex(TimeSpan timeSpan, SampleRateContainer sampleRate)
        {
            return (ulong)(timeSpan.TotalSeconds * (double)sampleRate.SamplesPerSecond);
        }

        #endregion
    }
}