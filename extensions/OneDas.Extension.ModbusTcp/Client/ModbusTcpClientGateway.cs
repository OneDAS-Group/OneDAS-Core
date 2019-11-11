using FluentModbus;
using Microsoft.Extensions.Logging;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extension.ModbusTcp
{
    public class ModbusTcpClientGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private uint _cycleCounter;

        private ModbusTcpClientSettings _settings;
        private ModbusTcpClient _modbusClient;
        private IPEndPoint _remoteEndpoint;

        private Task _task;
        private ManualResetEventSlim _manualResetEvent;
        private CancellationTokenSource _cts;

        private List<ModbusTcpModule> _inputModuleSet;
        private List<ModbusTcpModule> _outputModuleSet;

        private List<(ModbusTcpModule Module, byte[] Buffer)> _inboxSet;
        private List<(ModbusTcpModule Module, byte[] Buffer)> _outboxSet;

        private ILogger _logger;

        #endregion

        #region "Constructors"

        public ModbusTcpClientGateway(ModbusTcpClientSettings settings, ILoggerFactory loggerFactory) : base(settings)
        {
            _settings = settings;

            _modbusClient = new ModbusTcpClient();
            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(_settings.RemoteIpAddress), (int)_settings.Port);
            _manualResetEvent = new ManualResetEventSlim(false);

            _logger = loggerFactory.CreateLogger(this.DisplayName);
        }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            base.OnConfigure();

            _inputModuleSet = _settings.GetInputModuleSet().Cast<ModbusTcpModule>().ToList();
            _outputModuleSet = _settings.GetOutputModuleSet().Cast<ModbusTcpModule>().ToList();

            _inboxSet = _inputModuleSet.Select(module => (module, new byte[module.ByteCount])).ToList();
            _outboxSet = _outputModuleSet.Select(module => (module, new byte[module.ByteCount])).ToList();

            _cts = new CancellationTokenSource();

            // run async task
            _task = Task.Run(() =>
            {
                _manualResetEvent.Wait(_cts.Token);

                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        if (!_modbusClient.IsConnected)
                        {
                            _modbusClient.Connect(_remoteEndpoint);
                            _logger.LogInformation($"TCP connection established");
                        }

                        // inbox set
                        _inboxSet.ForEach(inboxContext =>
                        {
                            ModbusTcpModule module;
                            byte[] buffer;

                            module = inboxContext.Module;
                            buffer = inboxContext.Buffer;

                            switch (module.ObjectType)
                            {
                                case ModbusTcpObjectTypeEnum.HoldingRegister:
                                    _modbusClient.ReadHoldingRegisters(_settings.UnitIdentifier, module.StartingAddress, module.Quantity).CopyTo(buffer);
                                    break;
                                case ModbusTcpObjectTypeEnum.InputRegister:
                                    _modbusClient.ReadInputRegisters(_settings.UnitIdentifier, module.StartingAddress, module.Quantity).CopyTo(buffer);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        });

                        // outbox set
                        _outboxSet.ForEach(outBoxContext =>
                        {
                            ModbusTcpModule modbusTcpModule;
                            byte[] buffer;

                            modbusTcpModule = outBoxContext.Module;
                            buffer = outBoxContext.Buffer;

                            switch (modbusTcpModule.ObjectType)
                            {
                                case ModbusTcpObjectTypeEnum.HoldingRegister:

                                    if (modbusTcpModule.Quantity == 1)
                                    {
                                        _modbusClient.WriteSingleRegister(_settings.UnitIdentifier, modbusTcpModule.StartingAddress, buffer);
                                    }
                                    else
                                    {
                                        _modbusClient.WriteMultipleRegisters(_settings.UnitIdentifier, modbusTcpModule.StartingAddress, buffer);
                                    }

                                    break;

                                default:
                                    throw new NotImplementedException();
                            }
                        });

                        this.LastSuccessfulUpdate.Restart();
                    }
                    catch (NotImplementedException)
                    {
                        throw;
                    }
                    catch (ModbusException mex)
                    {
                        _logger.LogWarning(mex.Message);
                        _cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception)
                    {
                        _logger.LogWarning($"TCP connection or Modbus TCP communication failure");
                        _cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                        try
                        {
                            _modbusClient.Connect(_remoteEndpoint);
                            _logger.LogInformation($"TCP connection re-established");
                        }
                        catch
                        {
                            _logger.LogWarning($"TCP connection recovery failed");
                            _cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
                        }
                    }

                    _manualResetEvent.Reset();
                    _manualResetEvent.Wait(_cts.Token);
                }
            }, _cts.Token);
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
                // manualResetEvent is in state "unset", i.e. ModbusTCP update task has finished
                if (!_manualResetEvent.Wait(TimeSpan.Zero))
                {
                    // inbox set
                    _inboxSet.ForEach(inboxContext =>
                    {
                        inboxContext.Buffer.CopyTo(this.GetInputBuffer().Slice(inboxContext.Module.ByteOffset, inboxContext.Module.ByteCount));
                    });

                    // outbox set
                    _outboxSet.ForEach(outboxContext =>
                    {
                        this.GetOutputBuffer().Slice(outboxContext.Module.ByteOffset, outboxContext.Module.ByteCount).CopyTo(outboxContext.Buffer);
                    });
                }

                _manualResetEvent.Set();
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

            _modbusClient?.Disconnect();

            _cts?.Cancel();
            _manualResetEvent?.Set();
            _task?.Wait();

            _inboxSet = null;
            _outboxSet = null;
        }

        #endregion
    }
}
