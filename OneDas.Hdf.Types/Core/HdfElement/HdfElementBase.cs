using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public abstract class HdfElementBase
    {
        public HdfElementBase(string name)
        {
            this.Name = name;
        }

        [DataMember]
        public string Name { get; set; }

        public HdfElementBase Parent { get; set; }

        public abstract string GetPath();
        public abstract string GetDisplayName();
        public abstract IEnumerable<HdfElementBase> GetChildSet();
    }
}
