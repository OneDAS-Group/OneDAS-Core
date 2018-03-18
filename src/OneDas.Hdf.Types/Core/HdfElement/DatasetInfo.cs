using HDF.PInvoke;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class DatasetInfo : HdfElementBase
    {
        #region "Fields"

        List<SourceFileInfo> _sourceFileInfoSet;

        #endregion

        #region "Constructors"

        public DatasetInfo(string name, long typeId, HdfElementBase parent, bool isLazyLoading) : base(name, parent, isLazyLoading)
        {
            this.TypeId = typeId;

            _sourceFileInfoSet = new List<SourceFileInfo>();

            this.RegisterSyncGroup(nameof(DatasetInfo), new SyncContext(this.UpdateDatasetInfo));
        }

        #endregion

        #region "Properties"

        public long TypeId { get; set; }

        public List<SourceFileInfo> SourceFileInfoSet
        {
            get
            {
                this.Update(nameof(DatasetInfo));

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

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return new List<DatasetInfo> { };
        }

        protected override long GetId(long fileId)
        {
            return H5D.open(fileId, this.GetPath());
        }

        protected override void CloseId(long id)
        {
            if (H5I.is_valid(id) > 0) { H5D.close(id); }
        }

        private void UpdateDatasetInfo(FileContext fileContext, long datasetId)
        {
            long dataspaceId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            try
            {
                dataspaceId = H5D.get_space(datasetId);

                H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);

                _sourceFileInfoSet.Add(new SourceFileInfo(fileContext.FilePath, actualDimenionSet.First(), fileContext.DateTime));
            }
            finally
            {
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
            }
        }

        #endregion
    }
}