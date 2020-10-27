using HDF.PInvoke;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Hdf;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.DataManagement.Types.Hdf
{
    public static class ProjectExtensions
    {
        public static void Update(this ProjectInfo project, long projectGroupId, int formatVersion)
        {
            var idx = 0UL;

            GeneralHelper.SuppressErrors(() => H5L.iterate(projectGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero));

            int Callback(long projectGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long groupId = -1;

                try
                {
                    var name = Marshal.PtrToStringAnsi(intPtrName);

                    // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                    // and H5G_loc_info is not directly accessible
                    // only chance is to modify source code (H5Oget_info_by_name)
                    groupId = H5G.open(projectGroupId2, name);

                    if (groupId > -1)
                    {
                        var currentChannel = project.Channels.FirstOrDefault(channel => channel.Id == name);

                        if (currentChannel == null)
                        {
                            currentChannel = new ChannelInfo(name, project);
                            project.Channels.Add(currentChannel);
                        }

                        currentChannel.Update(groupId, formatVersion);
                    }
                }
                finally
                {
                    if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                }

                return 0;
            }
        }

        public static void Update(this ChannelInfo channel, long channelGroupId, int formatVersion)
        {
            var idx = 0UL;

            if (formatVersion == -1) // aggregation file
            {
                // do nothing
            }
            else
            {
                channel.Name = IOHelper.ReadAttribute<string>(channelGroupId, "name_set").Last();
                channel.Group = IOHelper.ReadAttribute<string>(channelGroupId, "group_set").Last();

                if (formatVersion != 1)
                {
                    channel.Unit = IOHelper.ReadAttribute<string>(channelGroupId, "unit_set").LastOrDefault();

                    // sometimes the unit property is null in the HDF file
                    if (string.IsNullOrWhiteSpace(channel.Unit))
                        channel.Unit = string.Empty;

                    // hdf_transfer_function_t to TransferFunction
                    var hdf_transfer_function_set = IOHelper.ReadAttribute<hdf_transfer_function_t>(channelGroupId, "transfer_function_set");
                    channel.TransferFunctions = hdf_transfer_function_set.Select(tf => tf.ToTransferFunction()).ToList();
                }
            }

            H5L.iterate(channelGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

            int Callback(long channelGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long datasetId = -1;

                try
                {
                    var name = Marshal.PtrToStringAnsi(intPtrName);

                    if (H5L.exists(channelGroupId2, name) > 0)
                    {
                        datasetId = H5D.open(channelGroupId2, name);

                        var currentDataset = channel.Datasets.FirstOrDefault(dataset => dataset.Id == name);

                        if (currentDataset == null)
                        {
                            currentDataset = new DatasetInfo(name, channel);
                            channel.Datasets.Add(currentDataset);
                        }

                        currentDataset.Update(datasetId);
                    }
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }

                return 0;
            }
        }


        public static void Update(this DatasetInfo dataset, long datasetId)
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
        }
    }
}
