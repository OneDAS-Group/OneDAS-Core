using HDF.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class CampaignInfo : HdfElementBase
    {
        #region "Fields"

        private DatasetInfo _chunkDatasetInfo;
        private List<VariableInfo> _variableInfoSet;

        #endregion

        #region "Constructors"

        public CampaignInfo(string name, HdfElementBase parent, bool isLazyLoading) : base(name, parent, isLazyLoading)
        {
            _chunkDatasetInfo = new DatasetInfo("is_chunk_completed_set", TypeConversionHelper.GetHdfTypeIdFromType(typeof(bool)), this, this.IsLazyLoading);
            _variableInfoSet = new List<VariableInfo>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public DatasetInfo ChunkDatasetInfo
        {
            get
            {
                this.Update();

                return _chunkDatasetInfo;
            }
        }

        [DataMember]
        public List<VariableInfo> VariableInfoSet
        {
            get
            {
                this.Update();

                return _variableInfoSet;
            }
        }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return this.Name;
        }

        public override string GetDisplayName()
        {
            return this.Name;
        }

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return this.VariableInfoSet;
        }

        protected override void OnUpdate(FileContext fileContext)
        {
            ulong idx;
            long campaignGroupId;

            idx = 0;
            campaignGroupId = H5G.open(fileContext.FileId, this.GetPath());

            try
            {
                _chunkDatasetInfo.Update(fileContext);

                GeneralHelper.SuppressErrors(() => H5L.iterate(campaignGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero));
            }
            finally
            {
                if (H5I.is_valid(campaignGroupId) > 0) { H5G.close(campaignGroupId); }
            }

            int Callback(long campaignGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long groupId = -1;
                string name;

                VariableInfo currentVariableInfo;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                    // and H5G_loc_info is not directly accessible
                    // only chance is to modify source code (H5Oget_info_by_name)
                    groupId = H5G.open(campaignGroupId2, name);

                    if (groupId > -1)
                    {
                        currentVariableInfo = _variableInfoSet.FirstOrDefault(variableInfo => variableInfo.Name == name);

                        if (currentVariableInfo == null)
                        {
                            currentVariableInfo = new VariableInfo(name, this, this.IsLazyLoading);
                            _variableInfoSet.Add(currentVariableInfo);
                        }

                        currentVariableInfo.Update(fileContext);
                    }
                }
                finally
                {
                    if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                }

                return 0;
            }
        }

        #endregion
    }
}
