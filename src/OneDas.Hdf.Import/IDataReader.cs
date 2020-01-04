using OneDas.DataStorage;
using OneDas.Extensibility;
using System.Collections.Generic;

namespace OneDas.Hdf.Import
{
    public interface IDataReader
    {
        List<VariableDescription> GetVariableDescriptions(string filePath);

        List<IDataStorage> GetData(string filePath, List<VariableDescription> variableDescriptionSet, bool convertToDouble);
    }
}
