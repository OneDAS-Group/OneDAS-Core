using OneDas.Infrastructure;
using System.Collections.Generic;

namespace OneDas.DataManagement.Connector
{
    public class VariableInfo
    {
        #region Properties

        public List<string> VariableNames { get; set; }

        public List<string> VariableGroups { get; set; }

        public List<string> Units { get; set; }

        public List<TransferFunction> TransferFunctions { get; set; }

        #endregion
    }
}
