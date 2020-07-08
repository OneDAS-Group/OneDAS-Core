using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OneDas.Buffers;
using OneDas.DataManagement.Core.Tests;
using OneDas.DataManagement.Extensions;
using OneDas.Extensibility;
using OneDas.Extension.Hdf;
using OneDas.Infrastructure;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace OneDas.Core.Tests
{
    public class HdfDataReaderTests
    {
        private ILogger _logger;

        public HdfDataReaderTests(ITestOutputHelper xunitLogger)
        {
            _logger = new XunitLoggerProvider(xunitLogger).CreateLogger(nameof(HdfDataReaderTests));
        }

        [Fact]
        public void ProvidesCampaignNames()
        {
            // arrange
            var root = this.InitializeDatabase();
            var dataReader = new HdfDataReader(root, _logger);

            // act
            var actual = dataReader.GetCampaignNames();

            // assert
            var expected = new List<string>() { "/A2/B/C", "/A/B/C" };
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void ProvidesCampaign()
        {
            // arrange
            var root = this.InitializeDatabase();
            var dataReader = new HdfDataReader(root, _logger);

            // act
            var actual = dataReader.GetCampaign("/A/B/C");
            var actualNames = actual.Variables.Select(variable => variable.Name).ToList();
            var actualGroups = actual.Variables.Select(variable => variable.Group).ToList();
            var actualUnits = actual.Variables.Select(variable => variable.Unit).ToList();

            // assert
            var expectedNames = new List<string>() { "A" };
            var expectedGroups = new List<string>() { "Group A" };
            var expectedUnits = new List<string>() { "Hz" };
            var expectedStartDate = new DateTime(2020, 07, 08, 00, 00, 00);
            var expectedEndDate = new DateTime(2020, 07, 10, 00, 00, 00);

            Assert.True(expectedNames.SequenceEqual(actualNames));
            Assert.True(expectedGroups.SequenceEqual(actualGroups));
            Assert.True(expectedUnits.SequenceEqual(actualUnits));
            Assert.Equal(expectedStartDate, actual.CampaignStart);
            Assert.Equal(expectedEndDate, actual.CampaignEnd);
        }

        [Fact]
        public void ProvidesDataAvailabilityStatistics()
        {
            // arrange
            var root = this.InitializeDatabase();
            var dataReader = new HdfDataReader(root, _logger);

            // act
            var actual = dataReader.GetDataAvailabilityStatistics("/A/B/C", new DateTime(2020, 07, 1), new DateTime(2020, 08, 01));

            // assert
            var expected = Enumerable.Range(0, 31).Select(number => 0).ToArray();
            expected[7] = 100;
            expected[8] = 100;

            Assert.True(actual.Data.SequenceEqual(expected));
        }

        [Fact]
        public void DetectsIfDataOfDayIsAvailable()
        {
            // arrange
            var root = this.InitializeDatabase();
            var dataReader = new HdfDataReader(root, _logger);

            // act
            var actual = dataReader.IsDataOfDayAvailable("/A/B/C", new DateTime(2020, 07, 08));

            // assert
            var expected = true;
            Assert.True(actual == expected);
        }

        [Fact]
        public void CanReadTwoDays()
        {
            // arrange
            var root = this.InitializeDatabase();
            var dataReader = new HdfDataReader(root, _logger);

            // act
            var campaign = dataReader.GetCampaign("/A/B/C");
            var dataset = campaign.Variables.First().Datasets.First();

            var begin = new DateTime(2020, 07, 08);
            var end = new DateTime(2020, 07, 10);

            var result = dataReader.ReadSingle<double>(dataset, begin, end);

            // assert
            Assert.True(result.Dataset[0] == 99.27636);
            Assert.True(result.Dataset[2] == 99.27626);
            Assert.True(result.Dataset[86400 * 100 - 1] == 2323e-3);
            Assert.True(result.Dataset[86400 * 100 + 0] == 98.27636);
            Assert.True(result.Dataset[86400 * 100 + 2] == 97.27626);
            Assert.True(result.Dataset[86400 * 100 + 86400 * 100 - 1] == 2323e-6);
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
            var settings = new HdfSettings()
            {
                FileGranularity = FileGranularity.Day
            };

            using var writer = new HdfWriter(settings, NullLogger.Instance);
            var campaignDescription = new OneDasCampaignDescription(Guid.NewGuid(), 1, "A", "B", "C");
            var context = new DataWriterContext("HdfTestDatabase", dataFolderPath, campaignDescription, new List<CustomMetadataEntry>());
            var varDesc = new List<VariableDescription>();
            varDesc.Add(new VariableDescription(Guid.NewGuid(), "A", "100 Hz", "Group A", OneDasDataType.FLOAT64, new SampleRateContainer(SampleRate.SampleRate_100), "Hz", new List<TransferFunction>(), BufferType.Extended));

            writer.Configure(context, varDesc);

            var buffer = (ExtendedBuffer<double>)BufferUtilities.CreateExtendedBuffer(OneDasDataType.FLOAT64, 86400 * 100);

            buffer.StatusBuffer[0] = 1;
            buffer.StatusBuffer[2] = 1;
            buffer.StatusBuffer[86400 * 100 - 1] = 1;

            var buffers = new List<IBuffer>();
            buffers.Add(buffer);

            // day 1 
            buffer.Buffer[0] = 99.27636;
            buffer.Buffer[2] = 99.27626;
            buffer.Buffer[86400 * 100 - 1] = 2323e-3;

            writer.Write(new DateTime(2020, 07, 08), TimeSpan.FromDays(1), buffers);

            // day 2
            buffer.Buffer[0] = 98.27636;
            buffer.Buffer[2] = 97.27626;
            buffer.Buffer[86400 * 100 - 1] = 2323e-6;
            writer.Write(new DateTime(2020, 07, 09), TimeSpan.FromDays(1), buffers);

            // second campaign
            using var writer2 = new HdfWriter(settings, NullLogger.Instance);

            campaignDescription.PrimaryGroupName = "A2";

            writer2.Configure(context, varDesc);
            writer2.Write(new DateTime(2020, 07, 08), TimeSpan.FromDays(1), buffers);

            return root;
        }
    }
}