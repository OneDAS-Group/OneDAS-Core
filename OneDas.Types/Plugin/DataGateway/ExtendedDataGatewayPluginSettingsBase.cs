using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    [DataContract]
    public abstract class ExtendedDataGatewayPluginSettingsBase : DataGatewayPluginSettingsBase
    {
        #region "Constructors"

        public ExtendedDataGatewayPluginSettingsBase()
        {
            this.InputModuleSet = new List<OneDasModule>();
            this.OutputModuleSet = new List<OneDasModule>();

            this.UpdateDataPortSet();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public IEnumerable<OneDasModule> InputModuleSet;

        [DataMember]
        public IEnumerable<OneDasModule> OutputModuleSet;

        public List<DataPort> DataPortSet { get; protected set; }
        public Dictionary<OneDasModule, List<DataPort>> ModuleToDataPortMap { get; protected set; }

        #endregion

        #region "Methods"

        public virtual void UpdateDataPortSet()
        {
            int indexInput;
            int indexOutput;

            this.DataPortSet = new List<DataPort>();

            // inputs
            indexInput = 0;
            indexOutput = 0;

            this.ModuleToDataPortMap = this.InputModuleSet.Concat(this.OutputModuleSet).ToDictionary(oneDasModule => oneDasModule, oneDasModule =>
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

        public virtual List<DataPort> CreateDataPortSet(OneDasModule oneDasModule, int index)
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

        public override IEnumerable<DataPort> GetDataPortSet()
        {
            return this.DataPortSet;
        }

        #endregion

        #region "Serialization"

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            this.UpdateDataPortSet();
        }

        #endregion
    }
}
