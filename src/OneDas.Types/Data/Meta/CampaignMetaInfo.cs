using System.Collections.Generic;
using System.Linq;

namespace OneDas.Data
{
    public class CampaignMetaInfo
    {
        #region Constructors

        public CampaignMetaInfo()
        {
            this.ShortDescription = string.Empty;
            this.LongDescription = string.Empty;
            this.Variables = new List<VariableMetaInfo>();
        }

        #endregion

        #region Properties

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
