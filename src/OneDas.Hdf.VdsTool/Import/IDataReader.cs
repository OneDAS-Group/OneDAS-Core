using OneDas.DataStorage;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.Hdf.VdsTool.Import
{
    public interface IDataReader
    {
        (TimeSpan Period, List<IDataStorage> DataStorageSet) GetData(DateTime startDateTime);

        List<VariableDescription> GetVariableDescriptions();
    }
}
