using HDF.PInvoke;
using OneDas.DataManagement.Hdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class HdfCampaignInfo : CampaignElement
    {
        #region "Constructors"

        public HdfCampaignInfo(string id) : base(id, null)
        {
            this.ChunkDataset = new HdfDatasetInfo("is_chunk_completed_set", this);
            this.Variables = new List<HdfVariableInfo>();
        }

        #endregion

        #region "Properties"

        public HdfDatasetInfo ChunkDataset { get; set; }

        public List<HdfVariableInfo> Variables { get; set; }

        #endregion

        #region "Methods"

        public void Update(long campaignGroupId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            long datasetId = -1;

            var idx = 0UL;

            // read chunk dataset info
            if (IOHelper.CheckLinkExists(campaignGroupId, this.ChunkDataset.Id))
            {
                datasetId = H5D.open(campaignGroupId, this.ChunkDataset.Id);

                try
                {
                    this.ChunkDataset.Update(datasetId, fileContext, updateSourceFileMap);
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }
            }

            // continue
            GeneralHelper.SuppressErrors(() => H5L.iterate(campaignGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero));

            int Callback(long campaignGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long groupId = -1;
                string name;

                HdfVariableInfo currentVariable;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                    // and H5G_loc_info is not directly accessible
                    // only chance is to modify source code (H5Oget_info_by_name)
                    groupId = H5G.open(campaignGroupId2, name);

                    if (groupId > -1)
                    {
                        currentVariable = this.Variables.FirstOrDefault(variable => variable.Id == name);

                        if (currentVariable == null)
                        {
                            currentVariable = new HdfVariableInfo(name, this);
                            this.Variables.Add(currentVariable);
                        }

                        currentVariable.Update(groupId, fileContext, updateSourceFileMap);
                    }
                }
                finally
                {
                    if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                }

                return 0;
            }
        }

        public override string GetPath()
        {
            return this.Id;
        }

        public override IEnumerable<CampaignElement> GetChilds()
        {
            return this.Variables;
        }

        #endregion
    }
}
