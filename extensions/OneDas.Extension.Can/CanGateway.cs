using Microsoft.Extensions.Logging;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extension.Can
{
    public class CanGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private uint _cycleCounter;

        private Dictionary<uint, CanModule> _inputModuleSet;
        private List<CanModule> _outputModuleSet;

        #endregion

        #region "Constructors"

        public CanGateway(CanSettings settings, ILoggerFactory loggerFactory) : base(settings)
        {
            this.Settings = settings;
            this.Logger = loggerFactory.CreateLogger(this.DisplayName);

            this.CanDriver = settings.CanDriverType switch
            {
                CanDriverType.IxxatUsbToCanV2Compact => new IxxatUsbToCanV2Compact(settings),
                                                   _ => throw new NotSupportedException(),
            };

            this.LastSuccessfulUpdate.Restart();
        }

        #endregion

        #region Properties

        private new CanSettings Settings { get; }

        private ICanDriver CanDriver { get; }

        private ILogger Logger { get; }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            base.OnConfigure();

            _inputModuleSet = this.Settings.GetInputModuleSet().Cast<CanModule>().ToDictionary(module => module.Identifier, module => module);
            _outputModuleSet = this.Settings.GetOutputModuleSet().Cast<CanModule>().ToList();
        }

        protected override int SetDataPortBufferOffset(KeyValuePair<OneDasModule, List<DataPort>> moduleEntry, int bufferOffsetBase, DataDirection dataDirection)
        {
            var canModule = (CanModule)moduleEntry.Key;
            canModule.ByteOffset = bufferOffsetBase;

            return base.SetDataPortBufferOffset(moduleEntry, bufferOffsetBase, dataDirection);
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            if (_cycleCounter % this.Settings.FrameRateDivider == 0)
            {
                // input
                for (int i = 0; i < this.CanDriver.AvailableMessagesCount; i++)
                {
                    this.CanDriver.Receive(out var identifier, out var data);

                    var inputModule = _inputModuleSet[identifier];
                    data.CopyTo(this.GetInputBuffer().Slice(inputModule.ByteOffset));
                }

                // output
                _outputModuleSet.ForEach(module =>
                {
                    var data = this.GetOutputBuffer().Slice(module.ByteOffset, module.GetByteCount());
                    this.CanDriver.Send(module.Identifier, module.FrameFormat, data);
                });

                this.LastSuccessfulUpdate.Restart();
            }

            _cycleCounter++;

            if (_cycleCounter >= OneDasConstants.NativeSampleRate)
                _cycleCounter = 0;
        }

        protected override void FreeManagedResources()
        {
            base.FreeManagedResources();

            this.CanDriver?.Dispose();
        }

        #endregion
    }
}
