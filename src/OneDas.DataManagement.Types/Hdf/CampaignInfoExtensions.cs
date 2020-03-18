using HDF.PInvoke;
using OneDas.DataManagement.Database;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.DataManagement.Hdf
{
    public static class CampaignInfoExtensions
    {
        public static void Update(this CampaignInfo campaign, long campaignGroupId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            long datasetId = -1;

            var idx = 0UL;

            // read chunk dataset info
            if (IOHelper.CheckLinkExists(campaignGroupId, campaign.ChunkDataset.Id))
            {
                datasetId = H5D.open(campaignGroupId, campaign.ChunkDataset.Id);

                try
                {
                    campaign.ChunkDataset.Update(datasetId, fileContext, updateSourceFileMap);
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

                VariableInfo currentVariable;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                    // and H5G_loc_info is not directly accessible
                    // only chance is to modify source code (H5Oget_info_by_name)
                    groupId = H5G.open(campaignGroupId2, name);

                    if (groupId > -1)
                    {
                        currentVariable = campaign.Variables.FirstOrDefault(variable => variable.Id == name);

                        if (currentVariable == null)
                        {
                            currentVariable = new VariableInfo(name, campaign);
                            campaign.Variables.Add(currentVariable);
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

        public static void Update(this VariableInfo variable, long variableGroupId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            var idx = 0UL;

            variable.VariableNames = IOHelper.UpdateAttributeList(variableGroupId, "name_set", variable.VariableNames.ToArray()).ToList();
            variable.VariableGroups = IOHelper.UpdateAttributeList(variableGroupId, "group_set", variable.VariableGroups.ToArray()).ToList();

            if (fileContext.FormatVersion != 1)
            {
                variable.Units = IOHelper.UpdateAttributeList(variableGroupId, "unit_set", variable.Units.ToArray()).ToList();

                // TransferFunction to hdf_transfer_function_t
                var transferFunctionSet = variable.TransferFunctions.Select(tf => hdf_transfer_function_t.FromTransferFunction(tf));

                // hdf_transfer_function_t to TransferFunction
                variable.TransferFunctions = IOHelper.UpdateAttributeList(variableGroupId, "transfer_function_set", transferFunctionSet.ToArray())
                    .Select(tf => tf.ToTransferFunction())
                    .ToList();
            }

            H5L.iterate(variableGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

            int Callback(long variableGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long datasetId = -1;

                string name;

                DatasetInfo currentDataset;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    if (H5L.exists(variableGroupId2, name) > 0)
                    {
                        datasetId = H5D.open(variableGroupId2, name);

                        currentDataset = variable.Datasets.FirstOrDefault(dataset => dataset.Id == name);

                        if (currentDataset == null)
                        {
                            currentDataset = new DatasetInfo(name, variable);
                            variable.Datasets.Add(currentDataset);
                        }

                        currentDataset.Update(datasetId, fileContext, updateSourceFileMap);
                    }
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }

                return 0;
            }
        }

        public static void Update(this DatasetInfo dataset, long datasetId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            long dataspaceId = -1;
            long typeId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            try
            {
                dataspaceId = H5D.get_space(datasetId);
                H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);

                typeId = H5D.get_type(datasetId);
                dataset.DataType = OneDasUtilities.GetOneDasDataTypeFromType(TypeConversionHelper.GetTypeFromHdfTypeId(typeId));
            }
            finally
            {
                if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
            }

            var sourceFileInfo = new SourceFileInfo(fileContext.FilePath, actualDimenionSet.First(), fileContext.DateTime);
            updateSourceFileMap?.Invoke(datasetId, dataset, sourceFileInfo);
        }
    }
}
