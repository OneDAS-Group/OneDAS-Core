using FluentModbus;
using Microsoft.Extensions.Logging;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace OneDas.Extension.ModbusTcp
{
    public class ModbusTcpServerGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private uint _cycleCounter;

        private ModbusTcpServerSettings _settings;
        private ModbusTcpServer _modbusServer;
        private IPEndPoint _localEndpoint;

        private List<ModbusTcpModule> _inputModuleSet;
        private List<ModbusTcpModule> _outputModuleSet;

        private ILogger _logger;

        #endregion

        #region "Constructors"

        public ModbusTcpServerGateway(ModbusTcpServerSettings settings, ILoggerFactory loggerFactory) : base(settings)
        {
            _settings = settings;
            _logger = loggerFactory.CreateLogger(this.DisplayName);

            _modbusServer = new ModbusTcpServer(_logger, isAsynchronous: false);
            _localEndpoint = new IPEndPoint(IPAddress.Parse(_settings.LocalIpAddress), (int)_settings.Port);

            this.LastSuccessfulUpdate.Restart();
        }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            base.OnConfigure();

            _inputModuleSet = _settings.GetInputModuleSet().Cast<ModbusTcpModule>().ToList();
            _outputModuleSet = _settings.GetOutputModuleSet().Cast<ModbusTcpModule>().ToList();

            _modbusServer.Start();
        }

        protected override int SetDataPortBufferOffset(KeyValuePair<OneDasModule, List<DataPort>> moduleEntry, int bufferOffsetBase, DataDirection dataDirection)
        {
            ModbusTcpModule modbusTcpModule;
            int dataPortInputOffset;

            modbusTcpModule = (ModbusTcpModule)moduleEntry.Key;
            modbusTcpModule.ByteOffset = bufferOffsetBase;

            dataPortInputOffset = 0;

            moduleEntry.Value.ForEach(dataPort =>
            {
                dataPort.DataPtr = new IntPtr(bufferOffsetBase + dataPortInputOffset);
                dataPortInputOffset += Marshal.SizeOf(OneDasUtilities.GetTypeFromOneDasDataType(dataPort.DataType));
            });

            return modbusTcpModule.Quantity * 2; // yields only even number of bytes (rounded up)
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            if (_cycleCounter % _settings.FrameRateDivider == 0)
            {
                _inputModuleSet.ForEach(module =>
                {
                    switch (module.ObjectType)
                    {
                        case ModbusTcpObjectTypeEnum.HoldingRegister:

                            _modbusServer.GetHoldingRegisterBuffer().Slice(module.StartingAddress * 2, module.ByteCount).CopyTo(this.GetInputBuffer().Slice(module.ByteOffset));
                            break;

                        case ModbusTcpObjectTypeEnum.DiscreteInput:
                        case ModbusTcpObjectTypeEnum.Coil:
                            throw new NotImplementedException();

                        case ModbusTcpObjectTypeEnum.InputRegister:
                        default:
                            throw new ArgumentException();
                    }
                });

                _outputModuleSet.ForEach(module =>
                {
                    switch (module.ObjectType)
                    {
                        case ModbusTcpObjectTypeEnum.InputRegister:

                            this.GetOutputBuffer().Slice(module.ByteOffset, module.ByteCount).CopyTo(_modbusServer.GetInputRegisterBuffer().Slice(module.StartingAddress * 2));
                            break;

                        case ModbusTcpObjectTypeEnum.HoldingRegister:

                            this.GetOutputBuffer().Slice(module.ByteOffset, module.ByteCount).CopyTo(_modbusServer.GetHoldingRegisterBuffer().Slice(module.StartingAddress * 2));
                            break;

                        case ModbusTcpObjectTypeEnum.DiscreteInput:
                        case ModbusTcpObjectTypeEnum.Coil:
                            throw new NotImplementedException();

                        default:
                            throw new ArgumentException();
                    }
                });

                _modbusServer.Update();
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

            _modbusServer?.Dispose();
        }

        #endregion
    }
}
