using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneDasDatabase = OneDas.DataManagement.Database.OneDasDatabase;

namespace OneDas.DataManagement.Explorer.Core
{
    public class RoslynProject : IDisposable
    {
        #region Fields

        private string _name;
        private string _sampleRate;
        private List<string> _requestedProjectIds;
        private OneDasDatabaseManager _databaseManager;
        private DocumentId _databaseCodeId;

        #endregion

        #region Constructors

        public RoslynProject(string name, OneDasDatabaseManager databaseManager, string defaultCode)
        {
            _name = name;
            _databaseManager = databaseManager;
            _databaseManager.OnDatabaseUpdated += this.OnDatabaseUpdated;
            _sampleRate = "NaN";

            var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);

            // workspace
            this.Workspace = new AdhocWorkspace(host);

            // project
            var filePath = typeof(object).Assembly.Location;
            var documentationProvider = XmlDocumentationProvider.CreateFromFile(@"./Resources/System.Runtime.xml");

            var projectInfo = ProjectInfo
                .Create(ProjectId.CreateNewId(), VersionStamp.Create(), "OneDas", "OneDas", LanguageNames.CSharp)
                .WithMetadataReferences(new[]
                {
                    MetadataReference.CreateFromFile(filePath, documentation: documentationProvider)
                })
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var project = this.Workspace.AddProject(projectInfo);

            // database code
            var databaseCode = this.Workspace.AddDocument(project.Id, "DatabaseCode.cs", SourceText.From(string.Empty));
            _databaseCodeId = databaseCode.Id;

            // code
            this.UseOnlyOnceDocument = this.Workspace.AddDocument(project.Id, "Code.cs", SourceText.From(defaultCode));
            this.DocumentId = this.UseOnlyOnceDocument.Id;
        }

        #endregion

        #region Properties

        public AdhocWorkspace Workspace { get; init; }

        public Document UseOnlyOnceDocument { get; init; }

        public DocumentId DocumentId { get; init; }

        #endregion

        #region Methods

        public void UpdateCode(DocumentId documentId, string code)
        {
            if (code == null)
                return;

            Solution updatedSolution;

            do
            {
                updatedSolution = this.Workspace.CurrentSolution.WithDocumentText(documentId, SourceText.From(code));
            } while (!this.Workspace.TryApplyChanges(updatedSolution));
        }

        public void SetValues(string code, string sampleRate, List<string> requestedProjectIds)
        {
            // update code
            this.UpdateCode(this.DocumentId, code);

            // database code
            _sampleRate = sampleRate;
            _requestedProjectIds = requestedProjectIds;

            var databaseCode = this.GenerateDatabaseCode(sampleRate, requestedProjectIds);
            this.UpdateCode(_databaseCodeId, databaseCode);
        }

        private string GenerateDatabaseCode(string sampleRate, List<string> requestedProjectIds)
        {
            // generate code
            var classStringBuilder = new StringBuilder();

            classStringBuilder.AppendLine($"namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}");
            classStringBuilder.AppendLine($"{{");

            classStringBuilder.AppendLine($"public class CodeGenerationDatabase");
            classStringBuilder.AppendLine($"{{");

            if (sampleRate is not null)
            {
                var filteredProjectContainer = _databaseManager.Database.ProjectContainers
                    .Where(projectContainer => requestedProjectIds.Contains(projectContainer.Id));

                foreach (var projectContainer in filteredProjectContainer)
                {
                    var addProject = false;
                    var projectStringBuilder = new StringBuilder();

                    // project class definition
                    projectStringBuilder.AppendLine($"public class {projectContainer.PhysicalName}_TYPE");
                    projectStringBuilder.AppendLine($"{{");

                    foreach (var channel in projectContainer.Project.Channels)
                    {
                        var addChannel = false;
                        var channelStringBuilder = new StringBuilder();

                        // channel class definition
                        channelStringBuilder.AppendLine($"public class {channel.Name}_TYPE");
                        channelStringBuilder.AppendLine($"{{");

                        foreach (var dataset in channel.Datasets.Where(dataset => dataset.Id.Contains(sampleRate)))
                        {
                            // dataset property
                            channelStringBuilder.AppendLine($"public double[] {OneDasUtilities.EnforceNamingConvention(dataset.Id, prefix: "DATASET")} {{ get; set; }}");

                            addChannel = true;
                            addProject = true;
                        }

                        channelStringBuilder.AppendLine($"}}");

                        // channel property
                        channelStringBuilder.AppendLine($"public {channel.Name}_TYPE {channel.Name} {{ get; }}");

                        if (addChannel)
                            projectStringBuilder.AppendLine(channelStringBuilder.ToString());
                    }

                    projectStringBuilder.AppendLine($"}}");

                    // project property
                    projectStringBuilder.AppendLine($"public {projectContainer.PhysicalName}_TYPE {projectContainer.PhysicalName} {{ get; }}");

                    if (addProject)
                        classStringBuilder.AppendLine(projectStringBuilder.ToString());
                }
            }

            classStringBuilder.AppendLine($"}}");

            classStringBuilder.AppendLine($"}}");
            return classStringBuilder.ToString();
        }

        public void Dispose()
        {
            _databaseManager.OnDatabaseUpdated -= this.OnDatabaseUpdated;
        }

        #endregion

        #region Callbacks

        private void OnDatabaseUpdated(object sender, OneDasDatabase database)
        {
            var databaseCode = this.GenerateDatabaseCode(_sampleRate, _requestedProjectIds);
            this.UpdateCode(_databaseCodeId, databaseCode);
        }

        #endregion
    }
}