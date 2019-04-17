using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.Extensibility
{
    public class VariableDescription
    {
        #region "Constructors"

        public VariableDescription(Guid guid, string variableName, string datasetName, string group, OneDasDataType dataType, ulong samplesPerDay, string unit, List<TransferFunction> transferFunctionSet, Type dataStorageType)
        {
            this.Guid = guid;
            this.VariableName = variableName;
            this.DatasetName = datasetName;
            this.Group = group;
            this.DataType = dataType;
            this.SamplesPerDay = samplesPerDay;
            this.Unit = unit;
            this.TransferFunctionSet = transferFunctionSet;
            this.DataStorageType = dataStorageType;

            if (!typeof(IDataStorage).IsAssignableFrom(dataStorageType))
            {
                throw new ArgumentException(ErrorMessage.VariableContext_DataStorageTypeInvalid);
            }
        }

        #endregion

        #region "Properties"

        public Guid Guid { get; private set; }
        public string VariableName { get; private set; }
        public string DatasetName { get; private set; }
        public string Group { get; private set; }
        public OneDasDataType DataType { get; private set; }
        public ulong SamplesPerDay { get; private set; }
        public string Unit { get; private set; }
        public List<TransferFunction> TransferFunctionSet { get; private set; }
        public Type DataStorageType { get; private set; }

        #endregion
    }
}