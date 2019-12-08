using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Extension.Famos;
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
            var dataWriter = provider.GetRequiredService<FamosWriter>();

            var campaignGuid = Guid.NewGuid();
            var dataDirectoryPath = Path.Combine(Path.GetTempPath(), campaignGuid.ToString());

            Directory.CreateDirectory(dataDirectoryPath);

            var campaignDescription = new OneDasCampaignDescription(campaignGuid, 1, "a", "b", "c");
            var customMetadataEntrySet = new List<CustomMetadataEntry>();
            var dataWriterContext = new DataWriterContext("OneDAS", dataDirectoryPath, campaignDescription, customMetadataEntrySet);

            var variableDescriptionSet = new List<VariableDescription>()
            {
                this.CreateVariableDescription("Var1", "Group1", OneDasDataType.FLOAT64, 8640000, "Unit1"),
                this.CreateVariableDescription("Var2", "Group2", OneDasDataType.FLOAT64, 8640000, "Unit2"),
                this.CreateVariableDescription("Var3", "Group1", OneDasDataType.FLOAT64, 86400, "Unit2"),
            };

            var currentDate = new DateTime(2019, 1, 1, 15, 0, 0);
            var period = TimeSpan.FromMinutes(1);

            // Act
            dataWriter.Configure(dataWriterContext, variableDescriptionSet);

            for (int i = 0; i < 9; i++)
            {
                var dataStorageSet = variableDescriptionSet.Select(current =>
                {
                    var length = (int)current.SamplesPerDay / 1440;
                    var offset = length * i;
                    var data = Enumerable.Range(offset, length).Select(value => value * 0 + (double)i + 1).ToArray();

                    return new SimpleDataStorage(data);
                }).ToList();

                dataWriter.Write(currentDate, period, dataStorageSet.Cast<IDataStorage>().ToList());
                currentDate += period;
            }

            // Assert
        }

        private VariableDescription CreateVariableDescription(string variableName, string group, OneDasDataType dataType, ulong samplesPerDay, string unit)
        {
            var guid = Guid.NewGuid();
            var sampleRate = samplesPerDay / 86400;
            var datasetName = $"{ 100 / (int)sampleRate } Hz";
            var transferFunctionSet = new List<TransferFunction>();
            var dataStorageType = typeof(SimpleDataStorage);

            return new VariableDescription(guid, variableName, datasetName, group, dataType, samplesPerDay, unit, transferFunctionSet, dataStorageType);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(current => new FamosWriter(new FamosSettings() { FileGranularity = FileGranularity.Minute_10 }, current.GetRequiredService<ILoggerFactory>()));

            services.AddLogging(loggingBuilder =>
            {
                //
            });
        }
    }
}