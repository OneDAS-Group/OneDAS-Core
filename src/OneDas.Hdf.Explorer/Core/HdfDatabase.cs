using HDF.PInvoke;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer.Core
{
    public class HdfDatabase : IDatabase
    {
        #region Fields

        private long _vdsFileId = -1;
        private long _vdsMetaFileId = -1;

        #endregion

        #region Constructors

        public HdfDatabase(string databasePath)
        {
            var filePath = Path.Combine(databasePath, "VDS.h5");
            var metaFilePath = Path.Combine(databasePath, "VDS_META.h5");

            _vdsFileId = H5F.open(filePath, H5F.ACC_RDONLY);

            if (File.Exists(metaFilePath))
                _vdsMetaFileId = H5F.open(metaFilePath, H5F.ACC_RDONLY);
        }

        #endregion

        #region Methods

        public ulong GetElementSize(string campaignName, string variableName, string datasetName)
        {
            long datasetId = -1;

            try
            {
                datasetId = H5D.open(_vdsFileId, $"{campaignName}/{variableName}/{datasetName}");
                return (ulong)OneDasUtilities.SizeOf(TypeConversionHelper.GetTypeFromHdfTypeId(H5D.get_type(datasetId)));
            }
            finally
            {
                if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
            }
        }

        public List<VariableDescription> GetVariableDescriptions(KeyValuePair<string, Dictionary<string, List<string>>> campaignInfo)
        {
            var variableDescriptions = new List<VariableDescription>();

            campaignInfo.Value.ToList().ForEach(variableInfo =>
            {
                variableInfo.Value.ForEach(datasetName =>
                {
                    long groupId = -1;
                    long typeId = -1;
                    long datasetId = -1;

                    try
                    {
                        groupId = H5G.open(_vdsFileId, $"{campaignInfo.Key}/{variableInfo.Key}");
                        datasetId = H5D.open(groupId, datasetName);
                        typeId = H5D.get_type(datasetId);

                        var displayName = IOHelper.ReadAttribute<string>(groupId, "name_set").Last();
                        var groupName = IOHelper.ReadAttribute<string>(groupId, "group_set").Last();
                        var unit = IOHelper.ReadAttribute<string>(groupId, "unit_set").LastOrDefault();
                        var hdf_transfer_function_t_set = IOHelper.ReadAttribute<hdf_transfer_function_t>(groupId, "transfer_function_set");
                        var transferFunctionSet = hdf_transfer_function_t_set.Select(tf => new TransferFunction(DateTime.ParseExact(tf.date_time, "yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture), tf.type, tf.option, tf.argument)).ToList();

                        var oneDasDataType = OneDasUtilities.GetOneDasDataTypeFromType(TypeConversionHelper.GetTypeFromHdfTypeId(typeId));
                        var sampleRate = new SampleRateContainer(datasetName);

                        variableDescriptions.Add(new VariableDescription(new Guid(variableInfo.Key), displayName, datasetName, groupName, oneDasDataType, sampleRate, unit, transferFunctionSet, DataStorageType.Simple));
                    }
                    finally
                    {
                        if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                        if (H5I.is_valid(groupId) > 0) { H5G.close(groupId); }
                        if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                    }
                });
            });

            return variableDescriptions;
        }

        public ISimpleDataStorage LoadDataset(string datasetPath, ulong start, ulong block)
        {
            long datasetId = -1;
            long typeId = -1;

            var dataset = IOHelper.ReadDataset(_vdsFileId, datasetPath, start, 1, block, 1);

            // apply status (only if native dataset)
            if (H5L.exists(_vdsFileId, datasetPath + "_status") > 0)
            {
                try
                {
                    datasetId = H5D.open(_vdsFileId, datasetPath);
                    typeId = H5D.get_type(datasetId);

                    var dataset_status = IOHelper.ReadDataset(_vdsFileId, datasetPath + "_status", start, 1, block, 1).Cast<byte>().ToArray();

                    var genericType = typeof(ExtendedDataStorage<>).MakeGenericType(TypeConversionHelper.GetTypeFromHdfTypeId(typeId));
                    var extendedDataStorage = (ExtendedDataStorageBase)Activator.CreateInstance(genericType, dataset, dataset_status);

                    dataset_status = null;

                    var simpleDataStorage = extendedDataStorage.ToSimpleDataStorage();
                    extendedDataStorage.Dispose();

                    return simpleDataStorage;
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                    if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                }
            }
            else
            {
                return new SimpleDataStorage(dataset.Cast<double>().ToArray());
            }
        }

        public (string name, string guid, string unit, List<hdf_transfer_function_t>) GetDocumentation(CampaignInfo campaignInfo, VariableInfo variableInfo)
        {
            long variable_groupId = -1;

            var name = variableInfo.VariableNameSet.Last();
            var guid = variableInfo.Name;
            var unit = string.Empty;
            var transferFunctionSet = new List<hdf_transfer_function_t>();

            try
            {
                var groupPath = GeneralHelper.CombinePath(campaignInfo.Name, variableInfo.Name);

                if (H5I.is_valid(_vdsMetaFileId) > 0 && IOHelper.CheckLinkExists(_vdsMetaFileId, groupPath))
                {
                    variable_groupId = H5G.open(_vdsMetaFileId, groupPath);

                    if (H5A.exists(variable_groupId, "unit") > 0)
                        unit = IOHelper.ReadAttribute<string>(variable_groupId, "unit").FirstOrDefault();

                    if (H5A.exists(variable_groupId, "transfer_function_set") > 0)
                        transferFunctionSet = IOHelper.ReadAttribute<hdf_transfer_function_t>(variable_groupId, "transfer_function_set").ToList();
                }
            }
            finally
            {
                if (H5I.is_valid(variable_groupId) > 0) { H5G.close(variable_groupId); }
            }

            return (name, guid, unit, transferFunctionSet);
        }

        public DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            ulong offset;
            int[] aggregatedData;
            DataAvailabilityGranularity granularity;

            // epoch & hyperslab
            var epochStart = new DateTime(2000, 01, 01);
            var epochEnd = new DateTime(2030, 01, 01);

            if (!(epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeEnd <= epochEnd))
                throw new Exception("requirement >> epochStart <= dateTimeBegin && dateTimeBegin <= dateTimeEnd && dateTimeBegin <= epochEnd << is not matched");

            var samplesPerDay = new SampleRateContainer("is_chunk_completed_set").SamplesPerDay;

            var start = (ulong)Math.Floor((dateTimeBegin - epochStart).TotalDays * samplesPerDay);
            var block = (ulong)Math.Ceiling((dateTimeEnd - dateTimeBegin).TotalDays * samplesPerDay);

            // get data
            var totalDays = (dateTimeEnd - dateTimeBegin).TotalDays;
            var data = IOHelper.ReadDataset<byte>(_vdsFileId, $"{ campaignName }/is_chunk_completed_set", start, 1UL, block, 1UL).Select(value => (int)value).ToArray();

            if (totalDays <= 7)
            {
                granularity = DataAvailabilityGranularity.ChunkLevel;
                aggregatedData = data;
            }
            else if (totalDays <= 365)
            {
                granularity = DataAvailabilityGranularity.DayLevel;
                offset = (ulong)dateTimeBegin.TimeOfDay.TotalMinutes;
                aggregatedData = new int[(int)Math.Ceiling(totalDays)];

                Parallel.For(0, (int)Math.Ceiling(totalDays), day =>
                {
                    var startIndex = (ulong)day * samplesPerDay; // inclusive
                    var endIndex = startIndex + samplesPerDay; // exclusive

                    if ((int)startIndex - (int)offset < 0)
                        startIndex = 0;
                    else
                        startIndex = startIndex - offset;

                    if (endIndex - offset >= (ulong)data.Length)
                        endIndex = (ulong)data.Length;
                    else
                        endIndex = endIndex - offset;

                    aggregatedData[day] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                });
            }
            else
            {
                var totalMonths = (dateTimeEnd.Month - dateTimeBegin.Month) + 1 + 12 * (dateTimeEnd.Year - dateTimeBegin.Year);
                var totalDateTimeBegin = new DateTime(dateTimeBegin.Year, dateTimeBegin.Month, 1);

                granularity = DataAvailabilityGranularity.MonthLevel;
                offset = (ulong)(dateTimeBegin - totalDateTimeBegin).TotalMinutes;
                aggregatedData = new int[totalMonths];

                Parallel.For(0, totalMonths, month =>
                {
                    ulong startIndex; // inclusive
                    ulong endIndex; // exclusive

                    var currentDateTimeBegin = totalDateTimeBegin.AddMonths(month);
                    var currentDateTimeEnd = currentDateTimeBegin.AddMonths(1);

                    if ((currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset < 0)
                        startIndex = 0;
                    else
                        startIndex = (ulong)(currentDateTimeBegin - totalDateTimeBegin).TotalMinutes - offset;

                    if ((currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset >= data.Length)
                        endIndex = (ulong)data.Length;
                    else
                        endIndex = (ulong)(currentDateTimeEnd - totalDateTimeBegin).TotalMinutes - offset;

                    aggregatedData[month] = (int)((double)data.Skip((int)startIndex).Take((int)(endIndex - startIndex)).Sum() / (endIndex - startIndex) * 100);
                });
            }

            return new DataAvailabilityStatistics(granularity, aggregatedData);
        }

        public void Dispose()
        {
            if (H5I.is_valid(_vdsFileId) > 0) { H5F.close(_vdsFileId); }
            if (H5I.is_valid(_vdsMetaFileId) > 0) { H5F.close(_vdsMetaFileId); }
        }

        #endregion
    }
}
