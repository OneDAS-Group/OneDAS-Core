using OneDas.Buffers;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class SparseProjectInfo : ProjectInfo
    {
        #region "Constructors"

        public SparseProjectInfo(string id, ProjectLicense license) : base(id)
        {
            this.License = license;
        }

        #endregion

        #region Properties

        public ProjectLicense License { get; }

        #endregion

        #region "Methods"

        public List<ChannelDescription> ToChannelDescriptions()
        {
            return this.Channels.SelectMany(channel =>
            {
                return channel.Datasets.Select(dataset =>
                {
                    var guid = new Guid(channel.Id);
                    var displayName = channel.Name;
                    var datasetName = dataset.Id;
                    var groupName = channel.Group;
                    var dataType = dataset.DataType;
                    var sampleRate = dataset.GetSampleRate();
                    var unit = channel.Unit;

                    return new ChannelDescription(guid,
                                                   displayName,
                                                   datasetName,
                                                   groupName,
                                                   dataType,
                                                   sampleRate,
                                                   unit,
                                                   BufferType.Simple);
                });
            }).ToList();
        }

        #endregion
    }
}
