﻿using HDF.PInvoke;
using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OneDas.DataManagement.Hdf
{
    public static class GeneralHelper
    {
        public static void UpdateCampaigns(long fileId, List<CampaignInfo> campaigns, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            GeneralHelper.InternalUpdateCampaigns(fileId, campaigns, null, updateSourceFileMap);
        }

        public static CampaignInfo GetCampaign(long fileId, string campaignGroupPath)
        {
            return GeneralHelper.InternalUpdateCampaigns(fileId, null, campaignGroupPath, null).FirstOrDefault();
        }

        public static List<CampaignInfo> GetCampaigns(long fileId)
        {
            return GeneralHelper.InternalUpdateCampaigns(fileId, null, null, null);
        }

        public static void SuppressErrors(Action action)
        {
            H5E.auto_t func = null;

            var clientData = IntPtr.Zero;

            H5E.get_auto(H5E.DEFAULT, ref func, ref clientData);
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);

            action.Invoke();

            H5E.set_auto(H5E.DEFAULT, func, clientData);
        }

        public static List<CampaignInfo> InternalUpdateCampaigns(long fileId,
                                                                     List<CampaignInfo> campaigns = null,
                                                                     string campaignGroupPath = "",
                                                                     UpdateSourceFileMapDelegate updateSourceFileMap = null)
        {
            var idx = 0UL;

            if (campaigns == null)
                campaigns = new List<CampaignInfo>();

            GeneralHelper.SuppressErrors(() => H5L.iterate(fileId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi("/")));

            return campaigns;

            int Callback(long campaignGroupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                ulong idx2 = 0;

                long groupId = -1;
                long datasetId = -1;

                int formatVersion;

                H5O.type_t objectType;

                //
                if (H5A.exists(fileId, "format_version") > 0) // raw file
                    formatVersion = IOHelper.ReadAttribute<int>(fileId, "format_version").First();
                else // virtual file
                    formatVersion = -1;

                var filePath = new StringBuilder(260);
                H5F.get_name(fileId, filePath, new IntPtr(260));
                var dateTime = DateTime.ParseExact(IOHelper.ReadAttribute<string>(fileId, "date_time").First(), "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                var name = Marshal.PtrToStringAnsi(intPtrName);
                var userData = Marshal.PtrToStringAnsi(intPtrUserData);
                var fullName = GeneralHelper.CombinePath(userData, name);
                var level = userData.Split("/".ToArray()).Count();

                // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                // and H5G_loc_info is not directly accessible
                // only chance is to modify source code (H5Oget_info_by_name)
                datasetId = H5D.open(campaignGroupId, name);

                if (H5I.is_valid(datasetId) > 0)
                {
                    objectType = H5O.type_t.DATASET;
                }
                else
                {
                    groupId = H5G.open(campaignGroupId, name);

                    if (H5I.is_valid(groupId) > 0)
                        objectType = H5O.type_t.GROUP;
                    else
                        objectType = H5O.type_t.UNKNOWN;
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
                                return 0;

                            var currentCampaign = campaigns.FirstOrDefault(campaignInfo => campaignInfo.Name == fullName);

                            if (currentCampaign == null)
                            {
                                currentCampaign = new CampaignInfo(fullName);
                                campaigns.Add(currentCampaign);
                            }

                            currentCampaign.Update(groupId, new FileContext(formatVersion, dateTime, filePath.ToString()), updateSourceFileMap);
                        }
                        
                        break;
                }

                if (objectType == H5O.type_t.GROUP && level < 3)
                    H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(fullName));

                // clean up
                if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }

                return 0;
            }
        }

        public static List<(string Path, string Name)> GetPathSet(long locationId, int maxDepth, bool includeGroups, bool includeDatasets)
        {
            var idx = 0UL;
            H5E.auto_t func = null;
            var datasetPathSet = new List<(string Path, string Name)>();
            var clientData = IntPtr.Zero;

            H5E.get_auto(H5E.DEFAULT, ref func, ref clientData);
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
            H5L.iterate(locationId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi("/"));
            H5E.set_auto(H5E.DEFAULT, func, clientData);

            return datasetPathSet;

            int Callback(long groupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                long datasetId = -1;
                H5O.type_t objectType;

                var idx2 = 0UL;
                var name = Marshal.PtrToStringAnsi(intPtrName);
                var userData = Marshal.PtrToStringAnsi(intPtrUserData);
                var level = userData.Split("/".ToArray()).Count();

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
                        objectType = H5O.type_t.GROUP;
                    else
                        objectType = H5O.type_t.UNKNOWN;
                }

                switch (objectType)
                {
                    case H5O.type_t.GROUP:

                        if (level <= maxDepth)
                        {
                            H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(GeneralHelper.CombinePath(userData, name)));

                            if (includeGroups)
                                datasetPathSet.Add((userData, name));

                            H5G.close(groupId);
                        }

                        break;

                    case H5O.type_t.DATASET:

                        if (includeDatasets)
                            datasetPathSet.Add((userData, name));

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
    }
}