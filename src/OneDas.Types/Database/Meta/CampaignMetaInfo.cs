using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.Database
{
    [DebuggerDisplay("{Name,nq}")]
    public class CampaignMetaInfo
    {
        #region Constructors

        public CampaignMetaInfo(string name)
        {
            this.Name = name;
            this.ShortDescription = string.Empty;
            this.LongDescription = string.Empty;
            this.Variables = new List<VariableMetaInfo>();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public List<VariableMetaInfo> Variables { get; set; }

        #endregion

        #region Methods

        public void Purge()
        {
            var variablesToDelete = this.Variables.Where(variable =>
                string.IsNullOrWhiteSpace(variable.Description) &&
                string.IsNullOrWhiteSpace(variable.Unit) &&
                !variable.TransferFunctions.Any());

            foreach (var variable in variablesToDelete)
            {
                this.Variables.Remove(variable);
            }
        }

        #endregion
    }
}
