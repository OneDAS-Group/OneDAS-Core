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
            var project = new SparseProjectInfo(this.Id);
            var variables = datasets.Select(dataset => (VariableInfo)dataset.Parent).Distinct().ToList();

            project.Variables = variables.Select(reference =>
            {
                var variableMeta = this.ProjectMeta.Variables.First(variableMeta => variableMeta.Id == reference.Id);

                var variable = new VariableInfo(reference.Id, project)
                {
                    Name = reference.Name,
                    Group = reference.Group,

                    Unit = !string.IsNullOrWhiteSpace(variableMeta.Unit) 
                        ? variableMeta.Unit
                        : reference.Unit,

                    TransferFunctions = variableMeta.TransferFunctions.Any()
                        ? variableMeta.TransferFunctions
                        : reference.TransferFunctions
                };

                var referenceDatasets = datasets.Where(dataset => (VariableInfo)dataset.Parent == reference);

                variable.Datasets = referenceDatasets.Select(referenceDataset =>
                {
                    return new DatasetInfo(referenceDataset.Id, variable)
                    {
                        DataType = referenceDataset.DataType,
                        IsNative = referenceDataset.IsNative
                    };
                }).ToList();

                return variable;
            }).ToList();

            return project;
        }

        #endregion
    }
}