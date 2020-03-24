using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class VariableMetaInfo
    {
        #region "Constructors"

        public VariableMetaInfo(string id)
        {
            this.Id = id;
            this.Description = string.Empty;
            this.Unit = string.Empty;
            this.TransferFunctions = new List<TransferFunction>();
        }

        private VariableMetaInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Id { get; set; }

        public string Description { get; set; }

        public string Unit { get; set; }

        public List<TransferFunction> TransferFunctions { get; set; }

        #endregion
    }
}
