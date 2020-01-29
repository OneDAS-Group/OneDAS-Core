using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OneDas.DataManagement.Database
{
    public abstract class InfoBase
    {
        #region "Constructors"

        public InfoBase(string name, InfoBase parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        private protected InfoBase()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Name { get; set; }

        [JsonIgnore]
        public InfoBase Parent { get; set; }

        #endregion

        #region "Methods"

        public abstract string GetPath();

        public abstract IEnumerable<InfoBase> GetChilds();

        public virtual void Initialize()
        {
            //
        }

        #endregion
    }
}
