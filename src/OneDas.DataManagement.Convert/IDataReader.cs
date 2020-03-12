using OneDas.Extensibility;
using OneDas.Buffers;
using System.Collections.Generic;

namespace OneDas.DataManagement.Convert
{
    public interface IDataReader
    {
        List<VariableDescription> GetVariableDescriptions(string filePath);

        List<IBuffer> GetData(string filePath, List<VariableDescription> variableDescriptionSet);
    }
}
