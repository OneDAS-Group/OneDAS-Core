using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OneDas.DataManagement.Database
{
    public abstract class ProjectElement
    {
        #region "Constructors"

        public ProjectElement(string id, ProjectElement parent)
        {
            this.Id = id;
            this.Parent = parent;
        }

        private protected ProjectElement()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Id { get; set; }

        [JsonIgnore]
        public ProjectElement Parent { get; set; }

        #endregion

        #region "Methods"

        public abstract string GetPath();

        public abstract IEnumerable<ProjectElement> GetChilds();

        public virtual void Initialize()
        {
            //
        }

        #endregion
    }
}
