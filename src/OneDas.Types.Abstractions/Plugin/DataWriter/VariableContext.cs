using OneDas.Infrastructure;
using System;
using System.Collections.Generic;

namespace OneDas.Plugin
{
    public class VariableContext
    {
        #region "Fields"

        private IList<IDataStorage> _dataStorageSet;

        #endregion

        #region "Constructors"

        public VariableContext(Guid guid, string variableName, string datasetName, string group, OneDasDataType dataType, ulong samplesPerDay, string unit, List<TransferFunction> transferFunctionSet, IList<IDataStorage> dataStorageSet)
        {
            this.Guid = guid;
            this.VariableName = variableName;
            this.DatasetName = datasetName;
            this.Group = group;
            this.DataType = dataType;
            this.SamplesPerDay = samplesPerDay;
            this.Unit = unit;
            this.TransferFunctionSet = transferFunctionSet;

            _dataStorageSet = dataStorageSet;
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

        #endregion

        #region "Methods"

        public IDataStorage GetDataStorage(int index)
        {
            return _dataStorageSet[index];
        }

        #endregion
    }
}