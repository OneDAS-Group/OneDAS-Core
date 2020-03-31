using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using OneDas.Buffers;
using OneDas.Extensibility;
using OneDas.Extension.Famos;
using OneDas.Extension.Hdf;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace OneDas.Core.Tests
{
    public class DataWriterFamosTests
    {
        [Fact]
        public void FamosWriterCreatesDatFile()
        {
            // Arrange
            var services = new ServiceCollection();

            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            var dataWriter = provider.GetRequiredService<HdfWriter>();

            var campaignGuid = Guid.NewGuid();
            var dataDirectoryPath = Path.Combine(Path.GetTempPath(), campaignGuid.ToString());

            Directory.CreateDirectory(dataDirectoryPath);

            var campaignDescription = new OneDasCampaignDescription(campaignGuid, 1, "a", "b", "c");
            var customMetadataEntrySet = new List<CustomMetadataEntry>();
            var dataWriterContext = new DataWriterContext("OneDAS", dataDirectoryPath, campaignDescription, customMetadataEntrySet);

            var variableDescriptionSet = new List<VariableDescription>()
            {
                this.CreateVariableDescription("Var3", "Group1", OneDasDataType.FLOAT64, new SampleRateContainer(144), "Unit2"),
            };

            var currentDate = new DateTime(2019, 1, 1, 15, 0, 0);
            var period = TimeSpan.FromMinutes(12);

            // Act
            dataWriter.Configure(dataWriterContext, variableDescriptionSet);

            var buffers = variableDescriptionSet.Select(current =>
            {
                var length = (int)current.SampleRate.SamplesPerDay;
                var data = Enumerable.Range(0, length).Select(value => (double)value + 1).ToArray();

                return BufferUtilities.CreateSimpleBuffer(data);
            }).ToList();

            dataWriter.Write(currentDate, period, buffers.Cast<IBuffer>().ToList());
            dataWriter.Dispose();

            // Assert
        }

        private VariableDescription CreateVariableDescription(string variableName, string group, OneDasDataType dataType, SampleRateContainer sampleRate, string unit)
        {
            var guid = Guid.NewGuid();
            var datasetName = sampleRate.ToUnitString();
            var transferFunctionSet = new List<TransferFunction>();

            return new VariableDescription(guid, variableName, datasetName, group, dataType, sampleRate, unit, transferFunctionSet, BufferType.Simple);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(current => new HdfWriter(new HdfSettings() { FileGranularity = FileGranularity.Minute_10 }, NullLogger.Instance));
        }
    }
}