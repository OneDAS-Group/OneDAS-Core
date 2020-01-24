using OneDas.DataStorage;
using OneDas.Extensibility;
using System.Collections.Generic;

namespace OneDas.DataManagement.Convert
{
    public interface IDataReader
    {
        List<VariableDescription> GetVariableDescriptions(string filePath);

        List<IDataStorage> GetData(string filePath, List<VariableDescription> variableDescriptionSet);
    }
}
