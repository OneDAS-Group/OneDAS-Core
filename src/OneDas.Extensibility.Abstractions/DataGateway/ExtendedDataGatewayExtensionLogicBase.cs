using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Extensibility
{
    public abstract class ExtendedDataGatewayExtensionLogicBase : DataGatewayExtensionLogicBase
    {

        #region "Fields"

        private int _inputBufferSize;
        private int _outputBufferSize;
        private bool _freeInputBufferPtr;
        private bool _freeOutputBufferPtr;

        #endregion

        #region "Constructors"

        public ExtendedDataGatewayExtensionLogicBase(ExtendedDataGatewayExtensionSettingsBase settings) : base(settings)
        {
            this.Settings = settings;

            this.UpdateDataPortSet();

            (this.InputBufferPtr, _inputBufferSize) = this.PrepareBuffer(this.GetModuleToDataPortMap(DataDirection.Input), DataDirection.Input);
            (this.OutputBufferPtr, _outputBufferSize) = this.PrepareBuffer(this.GetModuleToDataPortMap(DataDirection.Output), DataDirection.Output);
        }

        #endregion

        #region "Properties"

        public new ExtendedDataGatewayExtensionSettingsBase Settings { get; private set; }

        protected List<DataPort> DataPortSet { get; set; }

        protected Dictionary<OneDasModule, List<DataPort>> ModuleToDataPortMap { get; private set; }

        protected IntPtr InputBufferPtr { get; private set; }

        protected IntPtr OutputBufferPtr { get; private set; }

        #endregion

        #region "Methods"

        public override IEnumerable<DataPort> GetDataPortSet()
        {
            return this.DataPortSet;
        }

        public unsafe Span<byte> GetInputBuffer()
        {
            return new Span<byte>(this.InputBufferPtr.ToPointer(), _inputBufferSize);
        }

        public unsafe Span<byte> GetOutputBuffer()
        {
            return new Span<byte>(this.OutputBufferPtr.ToPointer(), _outputBufferSize);
        }

        protected virtual (IntPtr bufferPtr, int size) PrepareBuffer(Dictionary<OneDasModule, List<DataPort>> moduleToDataPortMap, DataDirection dataDirection)
        {
            int currentBufferSize;
            int size;

            IntPtr bufferPtr;

            currentBufferSize = 0;

            // prepare data port pointers and calculate total length of buffer
            moduleToDataPortMap.ToList().ForEach(moduleEntry =>
            {
                currentBufferSize += this.SetDataPortBufferOffset(moduleEntry, currentBufferSize, dataDirection);
            });

            // create buffer
            (bufferPtr, size) = this.CreateBuffer(currentBufferSize, dataDirection);

            // 
            moduleToDataPortMap.ToList().ForEach(moduleEntry =>
            {
                moduleEntry.Value.ForEach(dataPort => dataPort.DataPtr = IntPtr.Add(bufferPtr, dataPort.DataPtr.ToInt32()));
            });

            return (bufferPtr, size);
        }

        protected virtual int SetDataPortBufferOffset(KeyValuePair<OneDasModule, List<DataPort>> moduleEntry, int bufferOffsetBase, DataDirection dataDirection)
        {
            int dataPortOffset;

            dataPortOffset = 0;

            moduleEntry.Value.ForEach(dataPort =>
            {
                dataPort.DataPtr = new IntPtr(bufferOffsetBase + dataPortOffset);
                dataPortOffset += OneDasUtilities.SizeOf(dataPort.DataType);
            });

            return moduleEntry.Key.GetByteCount();
        }

        protected virtual unsafe (IntPtr BufferPtr, int Size) CreateBuffer(int size, DataDirection dataDirection)
        {
            IntPtr ptr;

            switch (dataDirection)
            {
                case DataDirection.Input:
                    _freeInputBufferPtr = true;
                    break;

                case DataDirection.Output:
                    _freeOutputBufferPtr = true;
                    break;

                default:
                    throw new ArgumentException();
            }

            ptr = Marshal.AllocHGlobal(size);

            new Span<bool>(ptr.ToPointer(), size).Clear();

            return (ptr, size);
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

            this.DataPortSet = this.ModuleToDataPortMap.SelectMany(moduleEntry => moduleEntry.Value).ToList();
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
            if (_freeInputBufferPtr)
            {
                Marshal.FreeHGlobal(this.InputBufferPtr);
            }

            if (_freeOutputBufferPtr)
            {
                Marshal.FreeHGlobal(this.OutputBufferPtr);
            }
        }

        private Dictionary<OneDasModule, List<DataPort>> GetModuleToDataPortMap(DataDirection dataDirection)
        {
            return this.ModuleToDataPortMap.Where(moduleEntry => moduleEntry.Key.DataDirection == dataDirection).ToDictionary(moduleEntry => moduleEntry.Key, moduleEntry => moduleEntry.Value);
        }

        #endregion
    }
}