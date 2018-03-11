using HDF.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class DatasetInfo : HdfElementBase
    {
        #region "Fields"

        List<(string filePath, ulong length, DateTime dateTime)> _sourceFileInfoSet;

        #endregion

        #region "Constructors"

        public DatasetInfo(string name, long typeId, HdfElementBase parent, bool isLazyLoading) : base(name, parent, isLazyLoading)
        {
            this.TypeId = typeId;

            _sourceFileInfoSet = new List<(string filePath, ulong length, DateTime dateTime)>();
        }

        #endregion

        #region "Properties"

        public long TypeId { get; set; }

        public List<(string filePath, ulong length, DateTime dateTime)> SourceFileInfoSet
        {
            get
            {
                this.Update();

                return _sourceFileInfoSet;
            }
        }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return $"{ this.Parent.GetPath() }/{ this.Name }";
        }

        public override string GetDisplayName()
        {
            return this.Name;
        }

        protected override void OnUpdate(FileContext fileContext)
        {
            long groupId = -1;
            long datasetId = -1;
            long dataspaceId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            try
            {
                groupId = H5G.open(fileContext.FileId, this.Parent.GetPath());

                if (H5L.exists(groupId, this.Name) > 0)
                {
                    datasetId = H5D.open(groupId, this.Name);
                    dataspaceId = H5D.get_space(datasetId);

                    H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);

                    _sourceFileInfoSet.Add((fileContext.FilePath, actualDimenionSet.First(), fileContext.DateTime));
                }
            }
            finally
            {
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
            }
        }

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return new List<DatasetInfo> { };
        }

        #endregion
    }
}