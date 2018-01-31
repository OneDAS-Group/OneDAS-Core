using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using OneDas.Hdf.IO;
using HDF.PInvoke;

namespace OneDas.Hdf.Core
{
    public static class GeneralHelper
    {
        public static void UpdateCampaignInfoSet(long fileId, Dictionary<string, CampaignInfo> campaignInfoSet)
        {
            GeneralHelper.InternalUpdateCampaignInfoSet(fileId, campaignInfoSet, null);
        }

        public static CampaignInfo GetCampaignInfo(long fileId, string campaignGroupPath)
        {
            return GeneralHelper.InternalUpdateCampaignInfoSet(fileId, null, campaignGroupPath).FirstOrDefault().Value;
        }

        public static Dictionary<string, CampaignInfo> GetCampaignInfoSet(long fileId)
        {
            return GeneralHelper.InternalUpdateCampaignInfoSet(fileId, null, null);
        }

        private static Dictionary<string, CampaignInfo> InternalUpdateCampaignInfoSet(long fileId, Dictionary<string, CampaignInfo> campaignInfoSet = null, string campaignGroupPath = "")
        {
            ulong idx;
            int formatVersion;

            CampaignInfo campaignInfo;
            VariableInfo variableInfo;
            DatasetInfo datasetInfo;
            DatasetInfo chunkDatasetInfo;

            H5E.auto_t func;
            IntPtr clientData;
            StringBuilder filePath;
            DateTime dateTime;

            //
            idx = 0;

            campaignInfo = null;
            variableInfo = null;
            datasetInfo = null;
            chunkDatasetInfo = null;

            func = null;
            clientData = IntPtr.Zero;
            filePath = new StringBuilder(260);

            //
            if (campaignInfoSet == null)
            {
                campaignInfoSet = new Dictionary<string, CampaignInfo>();
            }

            if (H5A.exists(fileId, "format_version") > 0) // raw file
            {
                formatVersion = IOHelper.ReadAttribute<int>(fileId, "format_version").First();
            }
            else // virtual file
            {
                formatVersion = -1;
            }

            dateTime = DateTime.ParseExact(IOHelper.ReadAttribute<string>(fileId, "date_time").First(), "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            H5F.get_name(fileId, filePath, new IntPtr(260));

            H5E.get_auto(H5E.DEFAULT, ref func, ref clientData);
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
            H5L.iterate(fileId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi("/"));
            H5E.set_auto(H5E.DEFAULT, func, clientData);

            return campaignInfoSet;

            // callback
            int Callback(long groupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                long groupId2 = -1;
                long datasetId = -1;
                long dataspaceId = -1;
                long typeId_do_not_close = -1;

                ulong idx2 = 0;
                ulong[] actualDimenionSet = new ulong[1];
                ulong[] maximumDimensionSet = new ulong[1];

                int level;

                string name;
                string fullName;
                string userData;

                H5O.type_t objectType;

                //
                name = Marshal.PtrToStringAnsi(intPtrName);
                userData = Marshal.PtrToStringAnsi(intPtrUserData);
                fullName = GeneralHelper.CombinePath(userData, name);
                level = userData.Split("/".ToArray()).Count();

                // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                // and H5G_loc_info is not directly accessible
                // only chance is to modify source code (H5Oget_info_by_name)
                datasetId = H5D.open(groupId, name);

                if (datasetId > -1)
                {
                    objectType = H5O.type_t.DATASET;
                }
                else
                {
                    groupId2 = H5G.open(groupId, name);

                    if (groupId2 > -1)
                    {
                        objectType = H5O.type_t.GROUP;
                    }
                    else
                    {
                        objectType = H5O.type_t.UNKNOWN;
                    }
                }

                switch (level)
                {
                    case 1:
                    case 2:
                        break;

                    case 3:

                        if (objectType == H5O.type_t.GROUP)
                        {
                            if (!string.IsNullOrWhiteSpace(campaignGroupPath) && fullName != campaignGroupPath)
                            {
                                return 0;
                            }

                            if (campaignInfoSet.ContainsKey(fullName))
                            {
                                campaignInfo = campaignInfoSet[fullName];
                            }
                            else
                            {
                                typeId_do_not_close = TypeConversionHelper.GetHdfTypeIdFromType(typeof(bool));
                                chunkDatasetInfo = new DatasetInfo("is_chunk_completed_set", typeId_do_not_close);
                                campaignInfo = new CampaignInfo(fullName, chunkDatasetInfo);
                                campaignInfoSet.Add(fullName, campaignInfo);
                            }

                            datasetId = H5D.open(groupId2, "is_chunk_completed_set");
                            dataspaceId = H5D.get_space(datasetId);

                            if (datasetId > -1)
                            {
                                H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);
                                campaignInfo.ChunkDatasetInfo.SourceFileInfoSet.Add((filePath.ToString(), actualDimenionSet.First(), dateTime));
                            }
                        }

                        break;

                    case 4:

                        if (objectType == H5O.type_t.GROUP)
                        {
                            if (campaignInfo.VariableInfoSet.ContainsKey(name))
                            {
                                variableInfo = campaignInfo.VariableInfoSet[name];
                            }
                            else
                            {
                                variableInfo = new VariableInfo(name);
                                campaignInfo.VariableInfoSet.Add(name, variableInfo);
                            }

                            variableInfo.VariableNameSet = IOHelper.UpdateAttributeList(groupId2, "name_set", variableInfo.VariableNameSet.ToArray());
                            variableInfo.VariableGroupSet = IOHelper.UpdateAttributeList(groupId2, "group_set", variableInfo.VariableGroupSet.ToArray());

                            if (formatVersion > 1)
                            {
                                // transfer function
                                variableInfo.UnitSet = IOHelper.UpdateAttributeList(groupId2, "unit_set", variableInfo.UnitSet.ToArray());
                                variableInfo.TransferFunctionSet = IOHelper.UpdateAttributeList(groupId2, "transfer_function_set", variableInfo.TransferFunctionSet.ToArray());
                            }
                        }

                        break;

                    case 5:

                        if (objectType == H5O.type_t.DATASET)
                        {
                            if (variableInfo.DatasetInfoSet.ContainsKey(name))
                            {
                                datasetInfo = variableInfo.DatasetInfoSet[name];
                            }
                            else
                            {
                                typeId_do_not_close = H5D.get_type(datasetId);
                                datasetInfo = new DatasetInfo(name, typeId_do_not_close);
                                variableInfo.DatasetInfoSet.Add(name, datasetInfo);
                            }

                            dataspaceId = H5D.get_space(datasetId);
                            H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);
                            datasetInfo.SourceFileInfoSet.Add((filePath.ToString(), actualDimenionSet.First(), dateTime));
                        }

                        break;
                }

                if (objectType == H5O.type_t.GROUP)
                {
                    H5L.iterate(groupId2, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(fullName));
                }

                // clean up
                if (H5I.is_valid(groupId2) > 0) { H5G.close(groupId2); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }

                return 0;
            }
        }

        public static List<(string Path, string Name)> GetPathSet(long locationId, int maxDepth, bool includeGroups, bool includeDatasets)
        {
            ulong idx;
            List<(string Path, string Name)> datasetPathSet;
            H5E.auto_t func;
            IntPtr clientData;

            idx = default;
            datasetPathSet = new List<(string Path, string Name)>();
            func = null;
            clientData = IntPtr.Zero;

            H5E.get_auto(H5E.DEFAULT, ref func, ref clientData);
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
            H5L.iterate(locationId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi("/"));
            H5E.set_auto(H5E.DEFAULT, func, clientData);

            return datasetPathSet;

            int Callback(long groupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                long datasetId = -1;

                ulong idx2 = default;
                int level;

                string name;
                string userData;

                H5O.type_t objectType;

                name = Marshal.PtrToStringAnsi(intPtrName);
                userData = Marshal.PtrToStringAnsi(intPtrUserData);
                level = userData.Split("/".ToArray()).Count();

                // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                // and H5G_loc_info is not directly accessible
                // only chance is to modify source code (H5Oget_info_by_name) or use V2 B-tree
                datasetId = H5D.open(groupId, name);

                if (datasetId > -1)
                {
                    objectType = H5O.type_t.DATASET;
                }
                else
                {
                    groupId = H5G.open(groupId, name);

                    if (groupId > -1)
                    {
                        objectType = H5O.type_t.GROUP;
                    }
                    else
                    {
                        objectType = H5O.type_t.UNKNOWN;
                    }
                }

                switch (objectType)
                {
                    case H5O.type_t.GROUP:

                        if (level <= maxDepth)
                        {
                            H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(GeneralHelper.CombinePath(userData, name)));

                            if (includeGroups)
                            {
                                datasetPathSet.Add((userData, name));
                            }

                            H5G.close(groupId);
                        }

                        break;

                    case H5O.type_t.DATASET:

                        if (includeDatasets)
                        {
                            datasetPathSet.Add((userData, name));
                        }

                        H5D.close(datasetId);
                        break;

                    default:
                        break;
                }

                return 0;
            }
        }

        public static string CombinePath(string path1, string path2)
        {
            return $"{path1}/{path2}".Replace("///", "/").Replace("//", "/");
        }

        public static double ToSampleRate(this string datasetName)
        {
            string[] datasetNamePartSet;

            datasetNamePartSet = datasetName.Split(' ', '_');

            switch (datasetNamePartSet[1])
            {
                case "Hz":
                    return Double.Parse(datasetNamePartSet[0]);

                case "s":
                    return 1 / Double.Parse(datasetNamePartSet[0]);

                default:
                    throw new NotImplementedException();
            }
        }

        public static object InvokeGenericMethod(Type methodParent, object instance, string methodName, BindingFlags bindingFlags, Type genericType, object[] parameters)
        {
            MethodInfo methodInfo;
            MethodInfo genericMethodInfo;

            methodInfo = methodParent.GetMethod(methodName, bindingFlags);
            genericMethodInfo = methodInfo.MakeGenericMethod(genericType);

            return genericMethodInfo.Invoke(instance, parameters);
        }
    }
}