using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.Extensibility
{
    public class VariableDescription
    {
        #region "Constructors"

        public VariableDescription(Guid guid, string variableName, string datasetName, string group, OneDasDataType dataType, SampleRateContainer sampleRate, string unit, List<TransferFunction> transferFunctionSet, DataStorageType dataStorageType)
        {
            this.Guid = guid;
            this.VariableName = variableName;
            this.DatasetName = datasetName;
            this.Group = group;
            this.DataType = dataType;
            this.SampleRate = sampleRate;
            this.Unit = unit;
            this.TransferFunctionSet = transferFunctionSet;
            this.DataStorageType = dataStorageType;
        }

        #endregion

        #region "Properties"

        public Guid Guid { get; set; }

        public string VariableName { get; private set; }

        public string DatasetName { get; private set; }

        public string Group { get; private set; }

        public OneDasDataType DataType { get; set; }

        public SampleRateContainer SampleRate { get; private set; }

        public string Unit { get; private set; }

        public List<TransferFunction> TransferFunctionSet { get; private set; }

        public DataStorageType DataStorageType { get; set; }

        #endregion
    }
}