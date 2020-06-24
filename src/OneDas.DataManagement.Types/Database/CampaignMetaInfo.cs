using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class CampaignMetaInfo
    {
        #region Constructors

        public CampaignMetaInfo(string id)
        {
            this.Id = id;
            this.ShortDescription = string.Empty;
            this.LongDescription = string.Empty;
            this.Variables = new List<VariableMetaInfo>();
        }

        private CampaignMetaInfo()
        {
            //
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public List<VariableMetaInfo> Variables { get; set; }

        #endregion

        #region Methods

        public void Initialize(CampaignInfo campaign)
        {
            if (string.IsNullOrWhiteSpace(this.ShortDescription))
                this.ShortDescription = "<no description available>";

            if (string.IsNullOrWhiteSpace(this.LongDescription))
                this.LongDescription = "<no description available>";

            // create missing variable meta instances
            var variablesToAdd = new List<VariableMetaInfo>();

            foreach (var referenceVariable in campaign.Variables)
            {
                var exists = this.Variables.Any(variable => variable.Id == referenceVariable.Id);

                if (!exists)
                    variablesToAdd.Add(new VariableMetaInfo(referenceVariable.Id));
            }

            this.Variables.AddRange(variablesToAdd);
            this.Variables = this.Variables.OrderBy(variable => variable.Id).ToList();
        }

        #endregion
    }
}
