using OneDas.DataStorage;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;

namespace OneDas.Hdf.VdsTool.Import
{
    public interface IDataReader
    {
        // pass period per file parameter because its not always determinable from the file itself
        List<VariableDescription> GetVariableDescriptions(string filePath, TimeSpan periodPerFile);

        List<IDataStorage> GetData(string filePath, List<VariableDescription> variableDescriptionSet, bool convertToDouble);
    }
}
