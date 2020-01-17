using HDF.PInvoke;
using OneDas.Database;
using OneDas.Hdf.IO;
using OneDas.Infrastructure;
using System;
using System.Globalization;
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
                campaignInfo.ChunkDatasetInfo.Update(datasetId, fileContext, updateSourceFileMap);
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
                        currentVariableInfo = campaignInfo.VariableInfos.FirstOrDefault(variableInfo => variableInfo.Name == name);

                        if (currentVariableInfo == null)
                        {
                            currentVariableInfo = new VariableInfo(name, campaignInfo);
                            campaignInfo.VariableInfos.Add(currentVariableInfo);
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

            variableInfo.VariableNames = IOHelper.UpdateAttributeList(variableGroupId, "name_set", variableInfo.VariableNames.ToArray()).ToList();
            variableInfo.VariableGroups = IOHelper.UpdateAttributeList(variableGroupId, "group_set", variableInfo.VariableGroups.ToArray()).ToList();

            if (fileContext.FormatVersion != 1)
            {
                variableInfo.Units = IOHelper.UpdateAttributeList(variableGroupId, "unit_set", variableInfo.Units.ToArray()).ToList();

                // TransferFunction to hdf_transfer_function_t
                var transferFunctionSet = variableInfo.TransferFunctions.Select(tf => hdf_transfer_function_t.FromTransferFunction(tf));

                // hdf_transfer_function_t to TransferFunction
                variableInfo.TransferFunctions = IOHelper.UpdateAttributeList(variableGroupId, "transfer_function_set", transferFunctionSet.ToArray())
                    .Select(tf => tf.ToTransferFunction())
                    .ToList();
            }

            H5L.iterate(variableGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

            int Callback(long variableGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long datasetId = -1;

                string name;

                DatasetInfo currentDatasetInfo;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    if (H5L.exists(variableGroupId2, name) > 0)
                    {
                        datasetId = H5D.open(variableGroupId2, name);

                        currentDatasetInfo = variableInfo.DatasetInfos.FirstOrDefault(datasetInfo => datasetInfo.Name == name);

                        if (currentDatasetInfo == null)
                        {
                            currentDatasetInfo = new DatasetInfo(name, variableInfo);
                            variableInfo.DatasetInfos.Add(currentDatasetInfo);
                        }

                        currentDatasetInfo.Update(datasetId, fileContext, updateSourceFileMap);
                    }
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }

                return 0;
            }
        }

        public static void Update(this DatasetInfo datasetInfo, long datasetId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
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

            var sourceFileInfo = new SourceFileInfo(fileContext.FilePath, actualDimenionSet.First(), fileContext.DateTime);
            updateSourceFileMap?.Invoke(datasetId, datasetInfo, sourceFileInfo);
        }
    }
}
