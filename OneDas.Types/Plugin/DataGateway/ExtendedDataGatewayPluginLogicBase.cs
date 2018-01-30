using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Plugin
{
    public abstract class ExtendedDataGatewayPluginLogicBase : DataGatewayPluginLogicBase
    {
        #region "Constructors"

        public ExtendedDataGatewayPluginLogicBase(ExtendedDataGatewayPluginSettingsBase settings) : base(settings)
        {
            this.Settings = settings;

            this.UpdateDataPortSet();
        }

        #endregion

        #region "Properties"

        public new ExtendedDataGatewayPluginSettingsBase Settings { get; private set; }

        protected byte[] InputBuffer { get; private set; }
        protected byte[] OutputBuffer { get; private set; }

        protected List<DataPort> DataPortSet { get; set; }
        protected Dictionary<OneDasModule, List<DataPort>> ModuleToDataPortMap { get; private set; }

        protected GCHandle InputBufferHandle { get; private set; }
        protected GCHandle OutputBufferHandle { get; private set; }

        #endregion

        #region "Methods"

        public override void Configure()
        {
            (this.InputBuffer, this.InputBufferHandle) = this.PrepareBuffer(this.GetModuleToDataPortMap(DataDirection.Input), DataDirection.Input);
            (this.OutputBuffer, this.OutputBufferHandle) = this.PrepareBuffer(this.GetModuleToDataPortMap(DataDirection.Output), DataDirection.Output);
        }

        public override IEnumerable<DataPort> GetDataPortSet()
        {
            return this.DataPortSet;
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

        protected virtual void UpdateDataPortSet()
        {
            int indexInput;
            int indexOutput;

            this.DataPortSet = new List<DataPort>();

            // inputs
            indexInput = 0;
            indexOutput = 0;

            this.ModuleToDataPortMap = this.Settings.ModuleSet.ToDictionary(oneDasModule => oneDasModule, oneDasModule =>
            {
                List<DataPort> dataPortSet;

                switch (oneDasModule.DataDirection)
                {
                    case DataDirection.Input:

                        dataPortSet = this.CreateDataPortSet(oneDasModule, indexInput);
                        indexInput += oneDasModule.Size;
                        break;

                    case DataDirection.Output:

                        dataPortSet = this.CreateDataPortSet(oneDasModule, indexOutput);
                        indexOutput += oneDasModule.Size;
                        break;

                    default:
                        throw new ArgumentException();
                }

                return dataPortSet;
            });

            this.DataPortSet = ModuleToDataPortMap.SelectMany(moduleEntry => moduleEntry.Value).ToList();
        }

        protected virtual List<DataPort> CreateDataPortSet(OneDasModule oneDasModule, int index)
        {
            string prefix;

            switch (oneDasModule.DataDirection)
            {
                case DataDirection.Input:
                    prefix = "Input"; break;
                case DataDirection.Output:
                    prefix = "Output"; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Enumerable.Range(0, oneDasModule.Size).Select(i => new DataPort($"{ prefix } { index + i }", oneDasModule.DataType, oneDasModule.DataDirection, oneDasModule.Endianness)).ToList();
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

        private Dictionary<OneDasModule, List<DataPort>> GetModuleToDataPortMap(DataDirection dataDirection)
        {
            return ModuleToDataPortMap.Where(moduleEntry => moduleEntry.Key.DataDirection == dataDirection).ToDictionary(moduleEntry => moduleEntry.Key, moduleEntry => moduleEntry.Value);
        }

        #endregion
    }
}