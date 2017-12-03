using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    [DataContract]
    public abstract class ExtendedDataGatewayPluginSettingsBase : DataGatewayPluginSettingsBase
    {
        #region "Constructors

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

        #endregion

        #region "Methods"

        public virtual void UpdateDataPortSet()
        {
            int index;

            this.DataPortSet = new List<DataPort>();

            // inputs
            index = 0;

            this.DataPortSet.AddRange(this.InputModuleSet.ToList().SelectMany(oneDasModule =>
            {
                IEnumerable<DataPort> dataPortSet;

                dataPortSet = this.CreateDataPortSet(oneDasModule, index);
                index += oneDasModule.Size;

                return dataPortSet;

            }).ToList());

            // outputs
            index = 0;

            this.DataPortSet.AddRange(this.OutputModuleSet.ToList().SelectMany(oneDasModule =>
            {
                IEnumerable<DataPort> dataPortSet;

                dataPortSet = this.CreateDataPortSet(oneDasModule, index);
                index += oneDasModule.Size;

                return dataPortSet;

            }).ToList());
        }

        public virtual IEnumerable<DataPort> CreateDataPortSet(OneDasModule oneDasModule, int index)
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

            return Enumerable.Range(0, oneDasModule.Size).Select(i => new DataPort($"{ prefix } { index + i }", oneDasModule.DataType, oneDasModule.DataDirection));
        }

        public override void Validate()
        {
            base.Validate();

            Contract.Requires(this.InputModuleSet != null);
            Contract.Requires(this.OutputModuleSet != null);
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
