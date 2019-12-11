using FluentModbus;
using Microsoft.Extensions.Logging;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extension.Modbus
{
    public abstract class ModbusClientGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private uint _cycleCounter;

        private Task _task;
        private ManualResetEventSlim _manualResetEvent;

        private List<ModbusModule> _inputModuleSet;
        private List<ModbusModule> _outputModuleSet;

        private List<(ModbusModule Module, byte[] Buffer)> _inboxSet;
        private List<(ModbusModule Module, byte[] Buffer)> _outboxSet;

        #endregion

        #region "Constructors"

        public ModbusClientGateway(ModbusClient modbusClient, ModbusClientSettings settings, ILoggerFactory loggerFactory) : base(settings)
        {
            this.ModbusClient = modbusClient;
            this.Settings = settings;

            this.Logger = loggerFactory.CreateLogger(this.DisplayName);

            _manualResetEvent = new ManualResetEventSlim(false);
        }

        #endregion

        #region Properties

        protected ModbusClient ModbusClient { get; }

        protected new ModbusClientSettings Settings { get; }

        protected ILogger Logger { get; }

        protected CancellationTokenSource CTS { get; private set; }

        #endregion

        #region "Methods"

        protected abstract void Connect();

        protected abstract void Reconnect();

        protected abstract void Disconnect();

        protected override void OnConfigure()
        {
            base.OnConfigure();

            _inputModuleSet = this.Settings.GetInputModuleSet().Cast<ModbusModule>().ToList();
            _outputModuleSet = this.Settings.GetOutputModuleSet().Cast<ModbusModule>().ToList();

            _inboxSet = _inputModuleSet.Select(module => (module, new byte[module.ByteCount])).ToList();
            _outboxSet = _outputModuleSet.Select(module => (module, new byte[module.ByteCount])).ToList();

            this.CTS = new CancellationTokenSource();

            // run async task
            _task = Task.Run(() =>
            {
                _manualResetEvent.Wait(this.CTS.Token);

                while (!this.CTS.IsCancellationRequested)
                {
                    try
                    {
                        this.Connect();

                        // inbox set
                        _inboxSet.ForEach(inboxContext =>
                        {
                            ModbusModule module;
                            byte[] buffer;

                            module = inboxContext.Module;
                            buffer = inboxContext.Buffer;

                            switch (module.ObjectType)
                            {
                                case ModbusObjectTypeEnum.HoldingRegister:
                                    this.ModbusClient.ReadHoldingRegisters(this.Settings.UnitIdentifier, module.StartingAddress, module.Quantity).CopyTo(buffer);
                                    break;
                                case ModbusObjectTypeEnum.InputRegister:
                                    this.ModbusClient.ReadInputRegisters(this.Settings.UnitIdentifier, module.StartingAddress, module.Quantity).CopyTo(buffer);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        });

                        // outbox set
                        _outboxSet.ForEach(outBoxContext =>
                        {
                            ModbusModule modbusModule;
                            byte[] buffer;

                            modbusModule = outBoxContext.Module;
                            buffer = outBoxContext.Buffer;

                            switch (modbusModule.ObjectType)
                            {
                                case ModbusObjectTypeEnum.HoldingRegister:

                                    if (modbusModule.Quantity == 1)
                                        this.ModbusClient.WriteSingleRegister(this.Settings.UnitIdentifier, modbusModule.StartingAddress, buffer);
                                    else
                                        this.ModbusClient.WriteMultipleRegisters(this.Settings.UnitIdentifier, modbusModule.StartingAddress, buffer);

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
                        this.Logger.LogWarning(mex.Message);
                        this.CTS.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception)
                    {
                        this.Reconnect();
                    }

                    _manualResetEvent.Reset();
                    _manualResetEvent.Wait(this.CTS.Token);
                }
            }, this.CTS.Token);
        }

        protected override int SetDataPortBufferOffset(KeyValuePair<OneDasModule, List<DataPort>> moduleEntry, int bufferOffsetBase, DataDirection dataDirection)
        {
            base.SetDataPortBufferOffset(moduleEntry, bufferOffsetBase, dataDirection);

            var modbusModule = (ModbusModule)moduleEntry.Key;
            modbusModule.ByteOffset = bufferOffsetBase;

            return modbusModule.Quantity * 2; // yields only even number of bytes (rounded up)
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            if (_cycleCounter % this.Settings.FrameRateDivider == 0)
            {
                // manualResetEvent is in state "unset", i.e. Modbus update task has finished
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

            this.Disconnect();

            this.CTS?.Cancel();
            _manualResetEvent?.Set();

            try
            {
                _task?.Wait();
            }
            catch (Exception ex) when (ex.InnerException.GetType() == typeof(TaskCanceledException))
            {
                //
            }

            _inboxSet = null;
            _outboxSet = null;
        }

        #endregion
    }
}
