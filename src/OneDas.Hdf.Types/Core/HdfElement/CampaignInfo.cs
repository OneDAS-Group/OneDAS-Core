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

            this.RegisterSyncGroup(nameof(CampaignInfo), new SyncContext(this.UpdateCampaignInfo));
        }

        #endregion

        #region "Properties"

        [DataMember]
        public DatasetInfo ChunkDatasetInfo
        {
            get
            {
                this.Update(nameof(CampaignInfo));

                return _chunkDatasetInfo;
            }
        }

        [DataMember]
        public List<VariableInfo> VariableInfoSet
        {
            get
            {
                this.Update(nameof(CampaignInfo));

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

        protected override long GetId(long fileId)
        {
            return H5G.open(fileId, this.GetPath());
        }

        protected override void CloseId(long id)
        {
            if (H5I.is_valid(id) > 0) { H5G.close(id); }
        }

        private void UpdateCampaignInfo(FileContext fileContext, long campaignGroupId)
        {
            ulong idx;

            idx = 0;

            _chunkDatasetInfo.Update(fileContext);

            GeneralHelper.SuppressErrors(() => H5L.iterate(campaignGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero));

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
