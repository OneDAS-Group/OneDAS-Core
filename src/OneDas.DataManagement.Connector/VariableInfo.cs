using OneDas.Infrastructure;
using System.Collections.Generic;

namespace OneDas.DataManagement.Connector
{
    public class VariableInfo
    {
        #region Properties

        public string Name { get; set; }

        public string Group { get; set; }

        public string Unit { get; set; }

        public List<TransferFunction> TransferFunctions { get; set; }

        #endregion
    }
}
