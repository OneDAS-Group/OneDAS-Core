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
    public class RoslynProject
    {
        #region Constructors

        static RoslynProject()
        {
            RoslynProject.AdditionalTypesCode =
$@"           
namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}.Filters
{{
    public record FilterChannel(string Name, string Group, string Unit);

    public static class FilterBase
    {{
        public abstract void Filter(FilterChannel filterChannel, DateTime begin, DateTime end, DataProvider data, double[] result);
        public abstract List<FilterChannel> GetFilters(Database database);
    }}
}}
";

            RoslynProject.DefaultFilterCode =
$@"using System;
                 
namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}.Filters
{{
    class Filter : FilterBase
    {{
        /// <summary>
        /// This method is used to do the calculations for a single filter channel that can be based on the channels 
        /// of one ore more available and accessible projects.
        /// </summary>
        /// <param name=""begin"">Enables the user to choose the right calibration factors for that time period.</param>
        /// <param name=""end"">Enables the user to choose the right calibration factors for that time period.</param>
        /// <param name=""data"">Contains the data of the preselected projects.</param>
        /// <param name=""result"">The resulting double array with length matching the time period and sample rate.</param>
        public void Filter(DateTime begin, DateTime end, DataProvider data, double[] result)
        {{
            /* This dataset has the same length as the result array. */
            var t1 = database.IN_MEMORY_TEST_ACCESSIBLE.T1.DATASET_1_s_mean;
            
            for (int i = 0; i < result.Length; i++)
            {{
                /* Example: Square each value. */
                result[i] = Math.Pow(t1[i], 2);
            }}
        }}
    }}
}}
";

            RoslynProject.DefaultSharedCode =
$@"using System;
                 
namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}.Filters
{{
    /// <summary>
    /// The purpose of this class is to provide shared code, i.e. predefined and 
    /// resuable functions. By default this class is static but you may change it
    /// to be instantiatable. Also, you may rename or create another class within
    /// this code file. All files of kind 'shared' get linked to other 'normal'
    /// code files to make their content available there.
    /// </summary>
    public static class Shared
    {{
        public static void MySharedFunction(string myParameter1)
        {{
            /* This is only an example function. You can define functions
             * with any signature. */
        }}
    }}
}};
";
        }

        public RoslynProject(FilterDescription filter, List<string> additionalCodeFiles, OneDasDatabase database = null)
        {
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
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release));

            var project = this.Workspace.AddProject(projectInfo);

            // code
            var document = this.Workspace.AddDocument(project.Id, "Code.cs", SourceText.From(filter.Code));
            this.DocumentId = document.Id;

            // additional types code
            this.Workspace.AddDocument(project.Id, Guid.NewGuid().ToString(), SourceText.From(RoslynProject.AdditionalTypesCode));

            // additional code
            foreach (var additionalCode in additionalCodeFiles)
            {
                this.Workspace.AddDocument(project.Id, Guid.NewGuid().ToString(), SourceText.From(additionalCode));
            }

            // database code
            if (database is not null)
            {
                var databaseCode = this.GenerateDatabaseCode(database, filter.SampleRate, filter.RequestedProjectIds);
                this.Workspace.AddDocument(project.Id, "DatabaseCode.cs", SourceText.From(databaseCode));
            }
        }

        #endregion

        #region Properties

        public static string AdditionalTypesCode { get; }

        public static string DefaultFilterCode { get; }

        public static string DefaultSharedCode { get; }

        public AdhocWorkspace Workspace { get; init; }

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

        private string GenerateDatabaseCode(OneDasDatabase database, string sampleRate, List<string> requestedProjectIds)
        {
            // generate code
            var classStringBuilder = new StringBuilder();

            classStringBuilder.AppendLine($"namespace {nameof(OneDas)}.{nameof(DataManagement)}.{nameof(Explorer)}.Filters");
            classStringBuilder.AppendLine($"{{");

            classStringBuilder.AppendLine($"public class DataProvider");
            classStringBuilder.AppendLine($"{{");

            // add Read() method
            classStringBuilder.AppendLine($"public double[] Read(string projectId, string channelId, string datasetId");

            if (sampleRate is not null)
            {
                var filteredProjectContainer = database.ProjectContainers
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

        #endregion
    }
}