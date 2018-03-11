using HDF.PInvoke;
using OneDas.Hdf.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OneDas.Hdf.Core
{
    public static class GeneralHelper
    {
        public static void UpdateCampaignInfoSet(long fileId, List<CampaignInfo> campaignInfoSet)
        {
            GeneralHelper.InternalUpdateCampaignInfoSet(fileId, false, campaignInfoSet, null);
        }

        public static CampaignInfo GetCampaignInfo(long fileId, bool isLazyLoading, string campaignGroupPath)
        {
            return GeneralHelper.InternalUpdateCampaignInfoSet(fileId, isLazyLoading, null, campaignGroupPath).FirstOrDefault();
        }

        public static List<CampaignInfo> GetCampaignInfoSet(long fileId, bool isLazyLoading)
        {
            return GeneralHelper.InternalUpdateCampaignInfoSet(fileId, isLazyLoading, null, null);
        }

        public static void SuppressErrors(Action action)
        {
            H5E.auto_t func;
            IntPtr clientData;

            func = null;
            clientData = IntPtr.Zero;

            H5E.get_auto(H5E.DEFAULT, ref func, ref clientData);
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);

            action.Invoke();

            H5E.set_auto(H5E.DEFAULT, func, clientData);
        }

        public static List<CampaignInfo> InternalUpdateCampaignInfoSet(long fileId, bool isLazyLoading, List<CampaignInfo> campaignInfoSet = null, string campaignGroupPath = "")
        {
            ulong idx;

            idx = 0;

            if (campaignInfoSet == null)
            {
                campaignInfoSet = new List<CampaignInfo>();
            }

            GeneralHelper.SuppressErrors(() => H5L.iterate(fileId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi("/")));

            return campaignInfoSet;

            int Callback(long campaignGroupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                long groupId = -1;
                long datasetId = -1;
                ulong idx2 = 0;
                int level;
                int formatVersion;

                string name;
                string fullName;
                string userData;

                H5O.type_t objectType;
                CampaignInfo currentCampaignInfo;
                StringBuilder filePath;
                DateTime dateTime;

                //
                if (H5A.exists(fileId, "format_version") > 0) // raw file
                {
                    formatVersion = IOHelper.ReadAttribute<int>(fileId, "format_version").First();
                }
                else // virtual file
                {
                    formatVersion = -1;
                }

                filePath = new StringBuilder(260);
                H5F.get_name(fileId, filePath, new IntPtr(260));
                dateTime = DateTime.ParseExact(IOHelper.ReadAttribute<string>(fileId, "date_time").First(), "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                name = Marshal.PtrToStringAnsi(intPtrName);
                userData = Marshal.PtrToStringAnsi(intPtrUserData);
                fullName = GeneralHelper.CombinePath(userData, name);
                level = userData.Split("/".ToArray()).Count();

                // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                // and H5G_loc_info is not directly accessible
                // only chance is to modify source code (H5Oget_info_by_name)
                datasetId = H5D.open(campaignGroupId, name);

                if (datasetId > -1)
                {
                    objectType = H5O.type_t.DATASET;
                }
                else
                {
                    groupId = H5G.open(campaignGroupId, name);

                    if (groupId > -1)
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

                            currentCampaignInfo = campaignInfoSet.FirstOrDefault(campaignInfo => campaignInfo.Name == fullName);

                            if (currentCampaignInfo == null)
                            {
                                currentCampaignInfo = new CampaignInfo(fullName, null, isLazyLoading);
                                campaignInfoSet.Add(currentCampaignInfo);
                            }

                            currentCampaignInfo.Update(new FileContext(fileId, formatVersion, dateTime, filePath.ToString()));
                        }

                        break;
                }

                if (objectType == H5O.type_t.GROUP && level < 3)
                {
                    H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(fullName));
                }

                // clean up
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }

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
            return $"{ path1 }/{ path2 }".Replace("///", "/").Replace("//", "/");
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