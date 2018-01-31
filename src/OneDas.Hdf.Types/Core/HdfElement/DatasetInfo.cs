using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class DatasetInfo : HdfElementBase
    {
        public DatasetInfo(string name, long typeId) : base(name)
        {
            this.TypeId = typeId;
            this.SourceFileInfoSet = new List<(string filePath, ulong length, DateTime dateTime)>();
        }

        public long TypeId { get; set; }
        public List<(string filePath, ulong length, DateTime dateTime)> SourceFileInfoSet { get; set; }

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Name}";
        }

        public override string GetDisplayName()
        {
            return this.Name;
        }

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return new List<DatasetInfo> { };
        }
    }
}