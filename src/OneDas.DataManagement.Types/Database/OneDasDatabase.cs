using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class OneDasDatabase
    {
        #region Constructors

        public OneDasDatabase()
        {
            this.ProjectContainers = new List<ProjectContainer>();
        }

        #endregion

        #region Properties

        public List<ProjectContainer> ProjectContainers { get; set; }

        #endregion

        #region Methods

        public List<ProjectInfo> GetProjects()
        {
            return this.ProjectContainers.Select(container => container.Project).ToList();
        }

        public bool TryFindProjectById(string id, out ProjectInfo project)
        {
            var projectContainer = this.ProjectContainers.FirstOrDefault(projectContainer => projectContainer.Id == id);
            project = projectContainer?.Project;

            return project != null;
        }

        public bool TryFindChannelById(string projectId, string channelId, out ChannelInfo channel)
        {
            channel = default;

            if (this.TryFindProjectById(projectId, out var project))
            {
                channel = project.Channels.FirstOrDefault(current => current.Id == channelId);

                if (channel == null)
                    channel = project.Channels.FirstOrDefault(current => current.Name == channelId);
            }

            return channel != null;
        }

        public bool TryFindDatasetById(string projectId, string channelId, string datsetId, out DatasetInfo dataset)
        {
            dataset = default;

            if (this.TryFindChannelById(projectId, channelId, out var channel))
            {
                dataset = channel.Datasets.FirstOrDefault(dataset => dataset.Id == datsetId);

                if (dataset != null)
                    return true;
            }

            return false;
        }

        public bool TryFindDataset(string path, out DatasetInfo dataset)
        {
            var pathParts = path.Split("/");

            if (pathParts.Length != 6)
                throw new Exception($"The channel path '{path}' is invalid.");

            var projectName = $"/{pathParts[1]}/{pathParts[2]}/{pathParts[3]}";
            var channelName = pathParts[4];
            var datasetName = pathParts[5];

            return this.TryFindDatasetById(projectName, channelName, datasetName, out dataset);
        }

        public bool TryFindDatasetsByGroup(string groupPath, out List<DatasetInfo> datasets)
        {
            var groupPathParts = groupPath.Split("/");

            if (groupPathParts.Length != 6)
                throw new Exception($"The group path '{groupPath}' is invalid.");

            var projectName = $"/{groupPathParts[1]}/{groupPathParts[2]}/{groupPathParts[3]}";
            var groupName = groupPathParts[4];
            var datasetName = groupPathParts[5];

            return this.TryFindDatasetsByGroup(projectName, groupName, datasetName, out datasets);
        }

        private bool TryFindDatasetsByGroup(string projectName, string groupName, string datasetName, out List<DatasetInfo> datasets)
        {
            datasets = new List<DatasetInfo>();

            var projectContainer = this.ProjectContainers.FirstOrDefault(projectContainer => projectContainer.Id == projectName);

            if (projectContainer != null)
            {
                var channels = projectContainer.Project.Channels
                    .Where(channel => channel.Group.Split('\n')
                    .Contains(groupName))
                    .OrderBy(channel => channel.Name)
                    .ToList();

                datasets
                    .AddRange(channels
                    .SelectMany(channel => channel.Datasets
                    .Where(dataset => dataset.Id == datasetName)));

                if (datasets.Any())
                    return true;
            }

            return false;
        }

        #endregion
    }
}