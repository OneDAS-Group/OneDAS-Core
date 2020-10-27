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

        public bool TryFindVariableById(string projectId, string variableId, out VariableInfo variable)
        {
            variable = default;

            if (this.TryFindProjectById(projectId, out var project))
            {
                variable = project.Variables.FirstOrDefault(variable => variable.Id == variableId);

                if (variable == null)
                    variable = project.Variables.FirstOrDefault(variable => variable.Name == variableId);
            }

            return variable != null;
        }

        public bool TryFindDatasetById(string projectId, string variableId, string datsetId, out DatasetInfo dataset)
        {
            dataset = default;

            if (this.TryFindVariableById(projectId, variableId, out var variable))
            {
                dataset = variable.Datasets.FirstOrDefault(dataset => dataset.Id == datsetId);

                if (dataset != null)
                    return true;
            }

            return false;
        }

        public bool TryFindDataset(string path, out DatasetInfo dataset)
        {
            var pathParts = path.Split("/");

            if (pathParts.Length != 6)
                throw new Exception($"The channel name '{path}' is invalid.");

            var projectName = $"/{pathParts[1]}/{pathParts[2]}/{pathParts[3]}";
            var variableName = pathParts[4];
            var datasetName = pathParts[5];

            return this.TryFindDatasetById(projectName, variableName, datasetName, out dataset);
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
                var variables = projectContainer.Project.Variables
                    .Where(variable => variable.Group.Split('\n')
                    .Contains(groupName))
                    .OrderBy(variable => variable.Name)
                    .ToList();

                datasets
                    .AddRange(variables
                    .SelectMany(variable => variable.Datasets
                    .Where(dataset => dataset.Id == datasetName)));

                if (datasets.Any())
                    return true;
            }

            return false;
        }

        #endregion
    }
}