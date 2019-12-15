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

        public CanGateway(CanSettings settings, ILogger logger) : base(settings, logger)
        {
            this.Settings = settings;

            this.LastSuccessfulUpdate.Restart();
        }

        #endregion

        #region Properties

        private new CanSettings Settings { get; }

        private ICanDevice CanDevice { get; set; }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            base.OnConfigure();

            this.CanDevice?.Dispose();

#warning: Implement reconnection logic, when e.g. USB device is plugged out. Do not prevent OneDAS from starting.
            this.CanDevice = this.Settings.CanDeviceType switch
            {
                CanDeviceType.IxxatUsbToCanV2Compact => new IxxatUsbToCanV2Compact(this.Settings),
                CanDeviceType.CanLoopbackDevice => new CanLoopbackDevice(),
                _ => throw new NotSupportedException()
            };

            this.Logger.LogInformation($"Device '{this.Settings.HardwareId}' with bit rate '{this.Settings.BitRate}' initialized.");

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
                for (int i = 0; i < this.CanDevice.AvailableMessagesCount; i++)
                {
                    if (this.CanDevice.Receive(out var identifier, out var data))
                    {
                        var inputModule = _inputModuleSet[identifier];
                        data.CopyTo(this.GetInputBuffer().Slice(inputModule.ByteOffset));
                    }
                }

                // output
                _outputModuleSet.ForEach(module =>
                {
                    var data = this.GetOutputBuffer().Slice(module.ByteOffset, module.GetByteCount());
                    this.CanDevice.Send(module.Identifier, module.FrameFormat, data);
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

            this.CanDevice?.Dispose();
        }

        #endregion
    }
}
