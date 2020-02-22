using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OneDas.DataManagement.Database
{
    public abstract class CampaignElement
    {
        #region "Constructors"

        public CampaignElement(string name, CampaignElement parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        private protected CampaignElement()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Name { get; set; }

        [JsonIgnore]
        public CampaignElement Parent { get; set; }

        #endregion

        #region "Methods"

        public abstract string GetPath();

        public abstract IEnumerable<CampaignElement> GetChilds();

        public virtual void Initialize()
        {
            //
        }

        #endregion
    }
}
