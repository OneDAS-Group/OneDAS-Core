using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class ProjectContainer
    {
        #region "Constructors"

        public ProjectContainer(string id, string rootPath)
        {
            this.Id = id;
            this.RootPath = rootPath;
            this.Project = new ProjectInfo(id);
        }

        private ProjectContainer()
        {
            //
        }

        #endregion

        #region "Properties"

        public string Id { get; set; }

        public string PhysicalName => this.Id.TrimStart('/').Replace('/', '_');

        public string RootPath { get; set; }

        public ProjectInfo Project { get; set; }

        public ProjectMetaInfo ProjectMeta { get; set; }

        #endregion

        #region Methods

        public void Initialize()
        {
            this.Project.Initialize();
        }

        public SparseProjectInfo ToSparseProject(List<DatasetInfo> datasets)
        {
            var project = new SparseProjectInfo(this.Id, this.ProjectMeta.License);
            var channels = datasets.Select(dataset => (ChannelInfo)dataset.Parent).Distinct().ToList();

            project.Channels = channels.Select(reference =>
            {
                var channelMeta = this.ProjectMeta.Channels.First(channelMeta => channelMeta.Id == reference.Id);

                var channel = new ChannelInfo(reference.Id, project)
                {
                    Name = reference.Name,
                    Group = reference.Group,

                    Unit = !string.IsNullOrWhiteSpace(channelMeta.Unit) 
                        ? channelMeta.Unit
                        : reference.Unit,

                    TransferFunctions = channelMeta.TransferFunctions.Any()
                        ? channelMeta.TransferFunctions
                        : reference.TransferFunctions
                };

                var referenceDatasets = datasets.Where(dataset => (ChannelInfo)dataset.Parent == reference);

                channel.Datasets = referenceDatasets.Select(referenceDataset =>
                {
                    return new DatasetInfo(referenceDataset.Id, channel)
                    {
                        DataType = referenceDataset.DataType,
                        IsNative = referenceDataset.IsNative
                    };
                }).ToList();

                return channel;
            }).ToList();

            return project;
        }

        #endregion
    }
}