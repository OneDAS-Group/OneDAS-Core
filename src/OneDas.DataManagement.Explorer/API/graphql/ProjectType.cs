using GraphQL;
using GraphQL.Types;
using OneDas.DataManagement.Database;
using System.Collections.Generic;
using System.Linq;
using static OneDas.DataManagement.Explorer.Controllers.ProjectsController;

namespace OneDas.DataManagement.Explorer.API
{
    public class ProjectType : ObjectGraphType<(ProjectInfo Project, ProjectMeta Meta)>
    {
        public ProjectType()
        {
            this.Name = "Project";

            this.Field(x => x.Project.Id, type: typeof(IdGraphType))
                .Description("The project ID.");

            this.Field(x => x.Project.ProjectStart)
                .Description("The datetime of the oldest data available.");

            this.Field(x => x.Project.ProjectEnd)
                .Description("The datetime of the newest data available.");

            this.Field<ChannelType>(
                "Channel",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "The ID of the channel." }),
                resolve: context =>
            {
                var id = context.GetArgument<string>("id");
                var project = context.Source.Project;
                var projectMeta = context.Source.Meta;

                var channel = project.Channels.First(current => current.Id == id);
                var channelMeta = projectMeta.Channels.First(current => current.Id == id);

#warning taken from ProjectsController, unify this
                var channel2 = new Channel()
                {
                    Id = channel.Id,
                    Name = channel.Name,
                    Group = channel.Group,
                    Unit = !string.IsNullOrWhiteSpace(channelMeta.Unit)
                            ? channelMeta.Unit
                            : channel.Unit,
                    Description = !string.IsNullOrWhiteSpace(channelMeta.Description)
                            ? channelMeta.Description
                            : channel.Description,
                    SpecialInfo = channelMeta.SpecialInfo
                };

                return channel2;
            });
            //.Description("A list of channels defined in the project.");

            this.Field<ListGraphType<ChannelType>>("Channels", resolve: context =>
            {
                var result = new List<Channel>();
                var project = context.Source.Project;
                var projectMeta = context.Source.Meta;

                foreach (var channel in project.Channels)
                {
                    var channelMeta = projectMeta.Channels.First(current => current.Id == channel.Id);

#warning taken from ProjectsController, unify this
                    var channel2 = new Channel()
                    {
                        Id = channel.Id,
                        Name = channel.Name,
                        Group = channel.Group,
                        Unit = !string.IsNullOrWhiteSpace(channelMeta.Unit)
                                ? channelMeta.Unit
                                : channel.Unit,
                        Description = channelMeta.Description,
                        SpecialInfo = channelMeta.SpecialInfo
                    };

                    result.Add(channel2);
                }

                return result;
            });
              //.Description("A list of channels defined in the project.");

            this.Field(x => x.Meta.ShortDescription)
                .Description("A short project description.");

            this.Field(x => x.Meta.LongDescription)
                .Description("A long project description.");

            this.Field(x => x.Meta.Contact)
                .Description("A project contact.");
        }
    }
}
