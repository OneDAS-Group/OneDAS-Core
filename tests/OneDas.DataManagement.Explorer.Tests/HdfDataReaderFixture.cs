using Microsoft.Extensions.Logging.Abstractions;
using OneDas.Buffers;
using OneDas.Extensibility;
using OneDas.Extension.Hdf;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace OneDas.DataManagement.Extensions.Tests
{
    public class HdfDataReaderFixture : IDisposable
    {
        public HdfDataReaderFixture()
        {
            this.Root = this.InitializeDatabase();
        }

        public string Root { get; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(this.Root, true);
            }
            catch
            {
                //
            }
        }

        private string InitializeDatabase()
        {
            // create dirs
            var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(root);

            var dataFolderPathEmpty = Path.Combine(root, "DATA", "2020-06");
            Directory.CreateDirectory(dataFolderPathEmpty);

            var dataFolderPath = Path.Combine(root, "DATA", "2020-07");
            Directory.CreateDirectory(dataFolderPath);

            // create files
            var dayOffset = 86400 * 100;
            var hourOffset = 360000;
            var halfHourOffset = hourOffset / 2;

            var settings = new HdfSettings()
            {
                FilePeriod = TimeSpan.FromDays(1)
            };

            using var writer1 = new HdfWriter(settings, NullLogger.Instance);
            var projectDescription = new OneDasProjectDescription(Guid.NewGuid(), 1, "A", "B", "C");
            var context = new DataWriterContext("HdfTestDatabase", dataFolderPath, projectDescription, new List<CustomMetadataEntry>());
            var varDesc = new List<ChannelDescription>();
            varDesc.Add(new ChannelDescription(Guid.NewGuid(), "A", "100 Hz", "Group A", OneDasDataType.FLOAT64, new SampleRateContainer(SampleRate.SampleRate_100), "Hz", new List<TransferFunction>(), BufferType.Extended));

            writer1.Configure(context, varDesc);

            // day 1 V1
            var buffer1 = (ExtendedBuffer<double>)BufferUtilities.CreateExtendedBuffer(OneDasDataType.FLOAT64, 86400 * 100);

            buffer1.StatusBuffer[0] = 1;
            buffer1.StatusBuffer[2] = 1;
            buffer1.StatusBuffer[dayOffset - 1] = 1;

            var buffers1 = new List<IBuffer>();
            buffers1.Add(buffer1);

            buffer1.Buffer[0] = 99.27636;
            buffer1.Buffer[2] = 99.27626;
            buffer1.Buffer[dayOffset - 1] = 2323e-3;

            writer1.Write(new DateTime(2020, 07, 08), TimeSpan.FromDays(1), buffers1);

            // day 2 V1
            var buffer2 = (ExtendedBuffer<double>)BufferUtilities.CreateExtendedBuffer(OneDasDataType.FLOAT64, 86400 * 100 - 360000);

            buffer2.StatusBuffer[0] = 1;
            buffer2.StatusBuffer[2] = 1;
            buffer2.StatusBuffer[dayOffset - hourOffset - 1] = 1;

            var buffers2 = new List<IBuffer>();
            buffers2.Add(buffer2);

            buffer2.Buffer[0] = 98.27636;
            buffer2.Buffer[2] = 97.27626;
            buffer2.Buffer[dayOffset - hourOffset - 1] = 2323e-6;
            writer1.Write(new DateTime(2020, 07, 09), TimeSpan.FromHours(23), buffers2);

            // day 2 V2
            projectDescription.Version = 2;
            using var writer2 = new HdfWriter(settings, NullLogger.Instance);

            var buffer3 = (ExtendedBuffer<double>)BufferUtilities.CreateExtendedBuffer(OneDasDataType.FLOAT64, 180000);

            buffer3.StatusBuffer[0] = 1;
            buffer3.StatusBuffer[2] = 1;
            buffer3.StatusBuffer[halfHourOffset - 1] = 1;

            var buffers3 = new List<IBuffer>();
            buffers3.Add(buffer3);

            buffer3.Buffer[0] = 90.27636;
            buffer3.Buffer[2] = 90.27626;
            buffer3.Buffer[halfHourOffset - 1] = 2323e-9;

            writer2.Configure(context, varDesc);
            writer2.Write(new DateTime(2020, 07, 09, 23, 30, 00), TimeSpan.FromMinutes(30), buffers3);

            // second project
            using var writer3 = new HdfWriter(settings, NullLogger.Instance);

            projectDescription.PrimaryGroupName = "A2";

            writer3.Configure(context, varDesc);
            writer3.Write(new DateTime(2020, 07, 08), TimeSpan.FromDays(1), buffers1);

            return root;
        }
    }
}
