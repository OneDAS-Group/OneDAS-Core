using HDF.PInvoke;
using OneDas.Database;
using OneDas.DataManagement;
using OneDas.DataManagement.Extensibility;
using OneDas.DataManagement.Hdf;
using OneDas.DataStorage;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer.DataReader
{
    public class HdfDataReader : DataReaderExtensionBase
    {
        #region Fields

        private string _filePath;
        private long _fileId = -1;

        #endregion

        #region Constructors

        public HdfDataReader(string databasePath)
        {
            _filePath = Path.Combine(databasePath, "VDS.h5");
        }

        #endregion

        #region Methods

        public override List<CampaignInfo> GetCampaigns()
        {
            this.EnsureOpened();

            return GeneralHelper.GetCampaigns(_fileId);
        }

        public override bool IsDataOfDayAvailable(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public override ISimpleDataStorage LoadDataset(string datasetPath, ulong start, ulong block)
        {
            this.EnsureOpened();

            long datasetId = -1;
            long typeId = -1;

            var dataset = IOHelper.ReadDataset(_fileId, datasetPath, start, 1, block, 1);

            // apply status (only if native dataset)
            if (H5L.exists(_fileId, datasetPath + "_status") > 0)
            {
                try
                {
                    datasetId = H5D.open(_fileId, datasetPath);
                    typeId = H5D.get_type(datasetId);

                    var dataset_status = IOHelper.ReadDataset(_fileId, datasetPath + "_status", start, 1, block, 1).Cast<byte>().ToArray();

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

        public override DataAvailabilityStatistics GetDataAvailabilityStatistics(string campaignName, DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            this.EnsureOpened();

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
            var data = IOHelper.ReadDataset<byte>(_fileId, $"{ campaignName }/is_chunk_completed_set", start, 1UL, block, 1UL).Select(value => (int)value).ToArray();

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

        public override void Open()
        {
            this.EnsureOpened();
        }

        public override void Close()
        {
            this.Dispose();
        }

        public override void Dispose()
        {
            if (H5I.is_valid(_fileId) > 0) { H5F.close(_fileId); }
        }

        private void EnsureOpened()
        {
            if (!(H5I.is_valid(_fileId) > 0))
                _fileId = H5F.open(_filePath, H5F.ACC_RDONLY);
        }

        protected override (T[] dataset, byte[] statusSet) ReadPartial<T>(DatasetInfo dataset, ulong start, ulong length)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
