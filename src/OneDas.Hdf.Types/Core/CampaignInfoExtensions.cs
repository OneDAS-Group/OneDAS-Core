using HDF.PInvoke;
using OneDas.Database;
using OneDas.Hdf.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.Hdf.Core
{
    public static class CampaignInfoExtensions
    {
        public static void Update(this CampaignInfo campaignInfo, long campaignGroupId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            long datasetId = -1;

            var idx = 0UL;

            // read chunk dataset info
            datasetId = H5D.open(campaignGroupId, campaignInfo.ChunkDatasetInfo.Name);

            try
            {
                campaignInfo.ChunkDatasetInfo.Update(datasetId);
            }
            finally
            {
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
            }

            // continue
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
                        currentVariableInfo = campaignInfo.VariableInfoSet.FirstOrDefault(variableInfo => variableInfo.Name == name);

                        if (currentVariableInfo == null)
                        {
                            currentVariableInfo = new VariableInfo(name, campaignInfo);
                            campaignInfo.VariableInfoSet.Add(currentVariableInfo);
                        }

                        currentVariableInfo.Update(groupId, fileContext, updateSourceFileMap);
                    }
                }
                finally
                {
                    if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                }

                return 0;
            }
        }

        public static void Update(this VariableInfo variableInfo, long variableGroupId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            var idx = 0UL;

            variableInfo.VariableGroupSet = IOHelper.UpdateAttributeList(variableGroupId, "group_set", variableInfo.VariableGroupSet.ToArray()).ToList();

            if (fileContext.FormatVersion != 1)
            {
                variableInfo.UnitSet = IOHelper.UpdateAttributeList(variableGroupId, "unit_set", variableInfo.UnitSet.ToArray()).ToList();
                variableInfo.TransferFunctionSet = IOHelper.UpdateAttributeList(variableGroupId, "transfer_function_set", _transferFunctionSet.ToArray()).ToList();
            }

            H5L.iterate(variableGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

            int Callback(long variableGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long datasetId = -1;
                long typeId_do_not_close = -1;
                string name;

                DatasetInfo currentDatasetInfo;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    if (H5L.exists(variableGroupId2, name) > 0)
                    {
                        datasetId = H5D.open(variableGroupId2, name);

                        currentDatasetInfo = variableInfo.DatasetInfoSet.FirstOrDefault(datasetInfo => datasetInfo.Name == name);

                        if (currentDatasetInfo == null)
                        {
                            typeId_do_not_close = H5D.get_type(datasetId);
                            currentDatasetInfo = new DatasetInfo(name, typeId_do_not_close, variableInfo);

                            variableInfo.DatasetInfoSet.Add(currentDatasetInfo);
                        }

                        var dimensionSize = currentDatasetInfo.Update(datasetId);
                        var sourceFileInfo = new SourceFileInfo(fileContext.FilePath, dimensionSize, fileContext.DateTime);

                        updateSourceFileMap?.Invoke(currentDatasetInfo, sourceFileInfo);
                    }
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }

                return 0;
            }
        }

        public static ulong Update(this DatasetInfo datasetInfo, long datasetId)
        {
            long dataspaceId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            try
            {
                dataspaceId = H5D.get_space(datasetId);
                H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);
            }
            finally
            {
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
            }

            return actualDimenionSet.First();
        }
    }
}
