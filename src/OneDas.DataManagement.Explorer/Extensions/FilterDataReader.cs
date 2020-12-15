﻿using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.Filters", "OneDAS filters", "Dynamically loads and compiles user-defined filters.", "", "")]
    public class FilterDataReader : DataReaderExtensionBase
    {
        #region Constructors

        public FilterDataReader(string rootPath, ILogger logger) : base(rootPath, logger)
        {
            //
        }

        #endregion

        #region Methods

        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            throw new NotImplementedException();
        }

        protected override List<ProjectInfo> LoadProjects()
        {
#warning revert this
            var testRootPath = this.RootPath.Substring(0, this.RootPath.Length - 1);

            var filePath = Path.Combine(testRootPath, "filters.json");

            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                var filterSettings = JsonSerializer.Deserialize<FilterSettings>(jsonString);

                var project = new ProjectInfo("FILTER_CHANNELS")
                {
                    ProjectStart = DateTime.MinValue,
                    ProjectEnd = DateTime.MaxValue
                };

                var channels = filterSettings.FilterDescriptions
                    .Where(filterDescription => filterDescription.CodeType == CodeType.Channel)
                    .Select(filterDescription =>
                {
                    var id = Guid.NewGuid();

                    var channel = new ChannelInfo(id.ToString(), project)
                    {
                        Group = filterDescription.Group,
                        Name = filterDescription.Name,
                        Unit = filterDescription.Unit  
                    };

                    var datasets = new List<DatasetInfo>()
                    {
                        new DatasetInfo(filterDescription.SampleRate, channel)
                        {
                            DataType = OneDasDataType.FLOAT64,
                            IsNative = true
                        }
                    };

                    channel.Datasets.AddRange(datasets);

                    return channel;
                });

                project.Channels.AddRange(channels);

                return new List<ProjectInfo>() { project };
            }
            else
            {
                return new List<ProjectInfo>();
            }
        }

        protected override double GetDataAvailability(string projectId, DateTime Day)
        {
            return 1;
        }

        #endregion
    }
}