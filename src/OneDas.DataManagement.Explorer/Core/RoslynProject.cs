using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Text;
using OneDasDatabase = OneDas.DataManagement.Database.OneDasDatabase;

namespace OneDas.DataManagement.Explorer.Core
{
    public class RoslynProject : IDisposable
    {
        #region Fields

        private OneDasDatabaseManager _databaseManager;
        private EventHandler<OneDasDatabase> _handler;
        private DocumentId _databaseCodeId;

        #endregion

        #region Constructors

        public RoslynProject(OneDasDatabaseManager databaseManager, string defaultCode)
        {
            _databaseManager = databaseManager;
            _handler = (sender, e) => this.UpdateDatabaseCode();
            _databaseManager.OnDatabaseUpdated += _handler;

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
            this.UpdateDatabaseCode();

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

        private void UpdateDatabaseCode()
        {
            // generate code
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}");
            stringBuilder.AppendLine($"{{");

            stringBuilder.AppendLine($"public class CodeGenerationDatabase");
            stringBuilder.AppendLine($"{{");

            foreach (var projectContainer in _databaseManager.Database.ProjectContainers)
            {
                // project class definition
                stringBuilder.AppendLine($"public class {projectContainer.PhysicalName}_TYPE");
                stringBuilder.AppendLine($"{{");

                foreach (var channel in projectContainer.Project.Channels)
                {
                    // channel class definition
                    stringBuilder.AppendLine($"public class {channel.Name}_TYPE");
                    stringBuilder.AppendLine($"{{");

                    foreach (var dataset in channel.Datasets.Where(dataset => dataset.Id.Contains("600 s")))
                    {
                        // dataset property
                        stringBuilder.AppendLine($"public double[] {OneDasUtilities.EnforceNamingConvention(dataset.Id, prefix: "DATASET")} {{ get; set; }}");
                    }

                    stringBuilder.AppendLine($"}}");

                    // channel property
                    stringBuilder.AppendLine($"public {channel.Name}_TYPE {channel.Name} {{ get; }}");
                }

                stringBuilder.AppendLine($"}}");

                // project property
                stringBuilder.AppendLine($"public {projectContainer.PhysicalName}_TYPE {projectContainer.PhysicalName} {{ get; }}");
            }

            stringBuilder.AppendLine($"}}");

            stringBuilder.AppendLine($"}}");
            var code = stringBuilder.ToString();

            // update code
            Solution updatedSolution;

            do
            {
                updatedSolution = this.Workspace.CurrentSolution.WithDocumentText(_databaseCodeId, SourceText.From(code));
            } while (!this.Workspace.TryApplyChanges(updatedSolution));
        }

        public void Dispose()
        {
            _databaseManager.OnDatabaseUpdated -= _handler;
        }

        #endregion
    }
}