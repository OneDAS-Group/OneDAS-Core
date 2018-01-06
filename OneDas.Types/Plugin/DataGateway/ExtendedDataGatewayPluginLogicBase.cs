using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    public abstract class ExtendedDataGatewayPluginLogicBase : DataGatewayPluginLogicBase
    {
        #region "Constructors"

        public ExtendedDataGatewayPluginLogicBase(ExtendedDataGatewayPluginSettingsBase settings, ILoggerFactory loggerFactory) : base(settings, loggerFactory)
        {
            this.Settings = settings;
        }

        #endregion

        #region "Properties"

        public new ExtendedDataGatewayPluginSettingsBase Settings { get; private set; }

        protected byte[] InputBuffer { get; private set; }
        protected byte[] OutputBuffer { get; private set; }

        protected GCHandle InputBufferHandle { get; private set; }
        protected GCHandle OutputBufferHandle { get; private set; }

        #endregion

        public override void Configure()
        {
            (this.InputBuffer, this.InputBufferHandle) = this.PrepareBuffer(this.Settings.GetModuleToDataPortMap(DataDirection.Input), DataDirection.Input);
            (this.OutputBuffer, this.OutputBufferHandle) = this.PrepareBuffer(this.Settings.GetModuleToDataPortMap(DataDirection.Output), DataDirection.Output);
        }

        protected virtual (byte[] Buffer, GCHandle BufferHandle) PrepareBuffer(Dictionary<OneDasModule, List<DataPort>> moduleToDataPortMap, DataDirection dataDirection)
        {
            int currentBufferSize;
            int firstUsableByte;

            byte[] buffer;
            GCHandle bufferHandle;

            currentBufferSize = 0;

            // calculate total length of buffer
            moduleToDataPortMap.ToList().ForEach(moduleEntry =>
            {
                currentBufferSize += this.SetDataPortBufferOffset(moduleEntry, currentBufferSize, dataDirection);
            });

            // create buffer
            (buffer, firstUsableByte) = this.CreateBuffer(currentBufferSize, dataDirection);
            bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            // 
            moduleToDataPortMap.ToList().ForEach(moduleEntry =>
            {
                moduleEntry.Value.ForEach(dataPort => dataPort.DataPtr = IntPtr.Add(bufferHandle.AddrOfPinnedObject(), firstUsableByte + dataPort.DataPtr.ToInt32()));
            });

            return (buffer, bufferHandle);
        }

        protected virtual int SetDataPortBufferOffset(KeyValuePair<OneDasModule, List<DataPort>> moduleEntry, int bufferOffsetBase, DataDirection dataDirection)
        {
            int dataPortOffset;

            dataPortOffset = 0;

            moduleEntry.Value.ForEach(dataPort =>
            {
                dataPort.DataPtr = new IntPtr(bufferOffsetBase + dataPortOffset);
                dataPortOffset += Marshal.SizeOf(InfrastructureHelper.GetTypeFromOneDasDataType(dataPort.OneDasDataType));
            });

            return moduleEntry.Key.GetByteCount();
        }

        protected virtual (byte[] Buffer, int FirstUsableByte) CreateBuffer(int size, DataDirection dataDirection)
        {
            return (new byte[size], 0);
        }

        protected override void FreeUnmanagedResources()
        {
            if (this.InputBufferHandle.IsAllocated)
            {
                this.InputBufferHandle.Free();
            }

            if (this.OutputBufferHandle.IsAllocated)
            {
                this.OutputBufferHandle.Free();
            }
        }
    }
}