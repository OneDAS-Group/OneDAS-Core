using HDF.PInvoke;
using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OneDas.DataManagement.Hdf
{
    public static class GeneralHelper
    {
        public static (ulong, ulong) CalculateChunkParameters(ulong periodInSeconds, decimal samplesPerSecond)
        {
            var maxChunkLength = 4096UL;
            var length = (ulong)(periodInSeconds * samplesPerSecond);
            var chunkLength = GeneralHelper.FindLargestDivisor(length, maxChunkLength);
            var chunkCount = length / chunkLength;

            return (chunkLength, chunkCount);
        }

        public static (ulong start, ulong block) GetStartAndBlock(DateTime begin, DateTime end, ulong samplesPerDay)
        {
            var epochStart = new DateTime(2000, 01, 01);
            var epochEnd = new DateTime(2030, 01, 01);

            if (!(epochStart <= begin && begin <= end && end <= epochEnd))
                throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

            var start = (ulong)Math.Floor((begin - epochStart).TotalDays * samplesPerDay);
            var block = (ulong)Math.Ceiling((end - begin).TotalDays * samplesPerDay);

            return (start, block);
        }

        public static void UpdateCampaigns(long fileId, List<HdfCampaignInfo> campaigns, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            GeneralHelper.InternalUpdateCampaigns(fileId, campaigns, null, updateSourceFileMap);
        }

        public static HdfCampaignInfo GetCampaign(long fileId, string campaignGroupPath)
        {
            return GeneralHelper.InternalUpdateCampaigns(fileId, null, campaignGroupPath, null).FirstOrDefault();
        }

        public static List<HdfCampaignInfo> GetCampaigns(long fileId)
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

        public static List<HdfCampaignInfo> InternalUpdateCampaigns(long fileId,
                                                                     List<HdfCampaignInfo> campaigns = null,
                                                                     string campaignGroupPath = "",
                                                                     UpdateSourceFileMapDelegate updateSourceFileMap = null)
        {
            var idx = 0UL;

            if (campaigns == null)
                campaigns = new List<HdfCampaignInfo>();

            GeneralHelper.SuppressErrors(() => H5L.iterate(fileId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, Marshal.StringToHGlobalAnsi("/")));

            return campaigns;

            int Callback(long campaignGroupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                ulong idx2 = 0;

                long groupId = -1;
                long datasetId = -1;

                int formatVersion;

                H5O.type_t objectType;

                try
                {
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

                                var currentCampaign = campaigns.FirstOrDefault(campaign => campaign.Id == fullName);

                                if (currentCampaign == null)
                                {
                                    currentCampaign = new HdfCampaignInfo(fullName);
                                    campaigns.Add(currentCampaign);
                                }

                                currentCampaign.Update(groupId, new FileContext(formatVersion, dateTime, filePath.ToString()), updateSourceFileMap);
                            }

                            break;
                    }

                    if (objectType == H5O.type_t.GROUP && level < 3)
                        H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback, Marshal.StringToHGlobalAnsi(fullName));
                }
                finally
                {
                    if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }              

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

        public static void UpdateCampaignStartAndEnd(long fileId, CampaignInfo campaign, int maxProbingCount)
        {
            DateTime minDate = DateTime.MaxValue;
            DateTime maxDate = DateTime.MinValue;

            var probingCount = Math.Min(campaign.Variables.Count, maxProbingCount);

            foreach (var variable in campaign.Variables.Take(probingCount))
            {
                if (GeneralHelper.TryGetMappingDate(fileId, variable, first: true, out var firstDate))
                {
                    if (firstDate < minDate)
                        minDate = firstDate;
                }

                if (GeneralHelper.TryGetMappingDate(fileId, variable, first: false, out var lastDate))
                {
                    if (lastDate > maxDate)
                        maxDate = lastDate;
                }
            }

            campaign.CampaignStart = minDate;
            campaign.CampaignEnd = maxDate;
        }

        private static bool TryGetMappingDate(long fileId, VariableInfo variable, bool first, out DateTime mappingDate)
        {
            long groupId1 = -1;
            long datasetId1 = -1;
            long dcpl1 = -1;

            long fileId2 = -1;
            long groupId2 = -1;
            long datasetId2 = -1;
            long dcpl2 = -1;

            var length = 255;
            var campaignName = variable.Parent.Id;

            mappingDate = DateTime.MinValue;

            // ensure variable
            if (variable == null)
                return false;

            var dataset = variable.Datasets.First();

            // ensure dataset
            if (dataset == null)
                return false;

            // go and find mapping date
            try
            {
                var index1 = IntPtr.Zero;
                var stringBuilder1 = new StringBuilder(length);

                groupId1 = H5G.open(fileId, GeneralHelper.CombinePath(campaignName, variable.Id));
                datasetId1 = H5D.open(groupId1, dataset.Id);
                dcpl1 = H5D.get_create_plist(datasetId1);

                if (!first)
                {
                    H5P.get_virtual_count(dcpl1, ref index1);
                    index1 -= 1;
                }

                H5P.get_virtual_filename(dcpl1, index1, stringBuilder1, new IntPtr(length));

                if (File.Exists(stringBuilder1.ToString()))
                {
                    var index2 = IntPtr.Zero;
                    var stringBuilder2 = new StringBuilder(length);

                    fileId2 = H5F.open(stringBuilder1.ToString(), H5F.ACC_RDONLY);
                    groupId2 = H5G.open(fileId2, GeneralHelper.CombinePath(campaignName, variable.Id));
                    datasetId2 = H5D.open(groupId2, dataset.Id);
                    dcpl2 = H5D.get_create_plist(datasetId2);

                    if (!first)
                    {
                        H5P.get_virtual_count(dcpl2, ref index2);
                        index2 -= 1;
                    }

                    H5P.get_virtual_filename(dcpl2, index2, stringBuilder2, new IntPtr(length));

                    var fileName = stringBuilder2.ToString();
                    var datePart = fileName.Substring(fileName.Length - 23, 20);

                    mappingDate = DateTime.ParseExact(datePart, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                    return true;
                }
            }
            finally
            {
                if (H5I.is_valid(dcpl2) > 0) { H5P.close(dcpl2); }
                if (H5I.is_valid(datasetId2) > 0) { H5D.close(datasetId2); }
                if (H5I.is_valid(groupId2) > 0) { H5G.close(groupId2); }
                if (H5I.is_valid(fileId2) > 0) { H5F.close(fileId2); }

                if (H5I.is_valid(dcpl1) > 0) { H5P.close(dcpl1); }
                if (H5I.is_valid(datasetId1) > 0) { H5D.close(datasetId1); }
                if (H5I.is_valid(groupId1) > 0) { H5G.close(groupId1); }
            }

            return false;
        }

        private static ulong FindLargestDivisor(ulong length, ulong limit)
        {
            if (length < limit)
                return length;

            var primes = GeneralHelper.Factorize(length)
                .GroupBy(value => value)
                .Select(group => group.Aggregate(1UL, (previous, next) => previous * next))
                .Where(prime => prime <= limit)
                .ToList();

            var products = GeneralHelper.Powerset(primes)
                .Select(combo => combo.Aggregate(1UL, (previous, next) => previous * next))
                .ToList();

            products.Sort();

            var result = products.LastOrDefault(value => value <= limit);

            return result;
        }

        private static List<ulong> Factorize(ulong number)
        {
            var primes = new List<ulong>();

            for (ulong div = 2; number > 1; div++)
            {
                if (number % div == 0)
                {
                    while (number % div == 0)
                    {
                        number /= div;
                        primes.Add(div);
                    }
                }
            }

            return primes;
        }

        private static List<List<T>> Powerset<T>(List<T> list)
        {
            var comboCount = (int)Math.Pow(2, list.Count) - 1;
            var result = new List<List<T>>();

            for (int i = 1; i < comboCount + 1; i++)
            {
                result.Add(new List<T>());

                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }

            return result;
        }
    }
}