using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using OneDas.Infrastructure;

namespace OneDas.Plugin
{
    public class DataWriterContext
    {
        private string errorDescription;

        public DataWriterContext(string systemName, string dataDirectoryPath, OneDasProjectDescription projectDescription, IList<CustomMetadataEntry> customMetadataEntrySet)
        {
            Contract.Requires(customMetadataEntrySet != null);

            customMetadataEntrySet.ToList().ForEach(customMetaDataEntry =>
            {
                if (!InfrastructureHelper.CheckNamingConvention(customMetaDataEntry.Key, out errorDescription))
                {
                    throw new ArgumentException($"Argument '{ nameof(customMetadataEntrySet) }', value '{ customMetaDataEntry.Key }': { errorDescription }");
                }
            });

            this.SystemName = systemName;
            this.DataDirectoryPath = dataDirectoryPath;
            this.ProjectDescription = projectDescription;
            this.CustomMetadataEntrySet = customMetadataEntrySet;
        }

        public string SystemName { get; private set; }
        public string DataDirectoryPath { get; private set; }
        public OneDasProjectDescription ProjectDescription { get; private set; }
        public IList<CustomMetadataEntry> CustomMetadataEntrySet { get; private set; }
    }
}