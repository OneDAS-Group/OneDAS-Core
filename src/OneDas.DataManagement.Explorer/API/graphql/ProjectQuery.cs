using GraphQL;
using GraphQL.Types;
using System.Linq;
using static OneDas.DataManagement.Explorer.Controllers.ProjectsController;

namespace OneDas.DataManagement.Explorer.API
{
    public class ProjectQuery : ObjectGraphType
    {
        public ProjectQuery(OneDasDatabaseManager databaseManager)
        {
            this.Field<ProjectType>(
                "Project",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "The ID of the project." }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>("id");
                    var projectContainer = databaseManager.Database.ProjectContainers
                        .FirstOrDefault(projectContainer => projectContainer.Id == id);

                    if (projectContainer != null)
                        return (projectContainer.Project, projectContainer.ProjectMeta);
                    else
                        return null;
                });

            this.Field<ProjectType>(
                "Projects",
                resolve: context =>
                {
                    return databaseManager.Database.ProjectContainers
                        .Select(projectContainer => (projectContainer.Project, projectContainer.ProjectMeta));
                });
        }
    }
}
