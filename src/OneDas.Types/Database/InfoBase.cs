using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Database
{
    [DataContract]
    public abstract class InfoBase
    {
        #region "Constructors"

        public InfoBase(string name, InfoBase parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string Name { get; set; }

        public InfoBase Parent { get; set; }

        #endregion

        #region "Methods"

        public abstract string GetPath();
        public abstract IEnumerable<InfoBase> GetChildSet();

        #endregion
    }
}
