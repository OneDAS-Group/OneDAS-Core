using FluentModbus;
using Microsoft.Extensions.Logging;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Extension.Modbus
{
    public abstract class ModbusServerGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private uint _cycleCounter;

        private List<ModbusModule> _inputModuleSet;
        private List<ModbusModule> _outputModuleSet;

        #endregion

        #region "Constructors"

        public ModbusServerGateway(ModbusServer modbusServer, ModbusServerSettings settings, ILoggerFactory loggerFactory) : base(settings)
        {
            this.ModbusServer = modbusServer;
            this.Settings = settings;

            this.Logger = loggerFactory.CreateLogger(this.DisplayName);

            this.LastSuccessfulUpdate.Restart();
        }

        #endregion

        #region Properties

        protected ModbusServer ModbusServer { get; }

        protected new ModbusServerSettings Settings { get; }

        protected ILogger Logger { get; }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            base.OnConfigure();

            _inputModuleSet = this.Settings.GetInputModuleSet().Cast<ModbusModule>().ToList();
            _outputModuleSet = this.Settings.GetOutputModuleSet().Cast<ModbusModule>().ToList();
        }

        protected override int SetDataPortBufferOffset(KeyValuePair<OneDasModule, List<DataPort>> moduleEntry, int bufferOffsetBase, DataDirection dataDirection)
        {
            ModbusModule modbusModule;
            int dataPortInputOffset;

            modbusModule = (ModbusModule)moduleEntry.Key;
            modbusModule.ByteOffset = bufferOffsetBase;

            dataPortInputOffset = 0;

            moduleEntry.Value.ForEach(dataPort =>
            {
                dataPort.DataPtr = new IntPtr(bufferOffsetBase + dataPortInputOffset);
                dataPortInputOffset += Marshal.SizeOf(OneDasUtilities.GetTypeFromOneDasDataType(dataPort.DataType));
            });

            return modbusModule.Quantity * 2; // yields only even number of bytes (rounded up)
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            if (_cycleCounter % this.Settings.FrameRateDivider == 0)
            {
                _inputModuleSet.ForEach(module =>
                {
                    switch (module.ObjectType)
                    {
                        case ModbusObjectTypeEnum.HoldingRegister:

                            this.ModbusServer.GetHoldingRegisterBuffer().Slice(module.StartingAddress * 2, module.ByteCount).CopyTo(this.GetInputBuffer().Slice(module.ByteOffset));
                            break;

                        case ModbusObjectTypeEnum.DiscreteInput:
                        case ModbusObjectTypeEnum.Coil:
                            throw new NotImplementedException();

                        case ModbusObjectTypeEnum.InputRegister:
                        default:
                            throw new ArgumentException();
                    }
                });

                _outputModuleSet.ForEach(module =>
                {
                    switch (module.ObjectType)
                    {
                        case ModbusObjectTypeEnum.InputRegister:

                            this.GetOutputBuffer().Slice(module.ByteOffset, module.ByteCount).CopyTo(this.ModbusServer.GetInputRegisterBuffer().Slice(module.StartingAddress * 2));
                            break;

                        case ModbusObjectTypeEnum.HoldingRegister:

                            this.GetOutputBuffer().Slice(module.ByteOffset, module.ByteCount).CopyTo(this.ModbusServer.GetHoldingRegisterBuffer().Slice(module.StartingAddress * 2));
                            break;

                        case ModbusObjectTypeEnum.DiscreteInput:
                        case ModbusObjectTypeEnum.Coil:
                            throw new NotImplementedException();

                        default:
                            throw new ArgumentException();
                    }
                });

                this.ModbusServer.Update();
                this.LastSuccessfulUpdate.Restart();
            }

            _cycleCounter++;

            if (_cycleCounter >= OneDasConstants.NativeSampleRate)
            {
                _cycleCounter = 0;
            }
        }

        protected override void FreeManagedResources()
        {
            base.FreeManagedResources();

            this.ModbusServer?.Dispose();
        }

        #endregion
    }
}
