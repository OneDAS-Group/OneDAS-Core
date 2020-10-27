using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Infrastructure;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace OneDas.DataManagement.Explorer.Controllers
{
    [Route("api/v1/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerStateManager _stateManager;

        #endregion

        public ProjectsController(OneDasExplorerStateManager stateManager,
                                  OneDasDatabaseManager databaseManager,
                                  ILoggerFactory loggerFactory)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #region Methods

        /// <summary>
        /// Gets a list of all accessible projects.
        /// </summary>
        [HttpGet]
        public ActionResult<List<Project>> Projects()
        {
            var projectContainers = _databaseManager.Database.ProjectContainers;

            projectContainers = projectContainers.Where(projectContainer
                => Utilities.IsProjectAccessible(this.User, projectContainer.Id, _databaseManager.Config.RestrictedProjects)
                && Utilities.IsProjectVisible(this.User, projectContainer.Id, Constants.HiddenProjects))
                .ToList();

            return projectContainers.Select(projectContainer
                => this.CreateProjectResponse(projectContainer.Project, projectContainer.ProjectMeta))
                .ToList();
        }

        /// <summary>
        /// Gets the specified project.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public ActionResult<Project> Projects(string projectId)
        {
            projectId = WebUtility.UrlDecode(projectId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests project '{projectId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                return this.ProcessProjectId<Project>(projectId, message,
                    (project, projectMeta) =>
                    {
                        _logger.LogInformation($"{message} Done.");
                        return this.CreateProjectResponse(project, projectMeta);
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Gets a list of all channels in the specified project.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        [HttpGet("{projectId}/channels")]
        public ActionResult<List<Channel>> Channels(
            string projectId)
        {
            projectId = WebUtility.UrlDecode(projectId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests channels for project '{projectId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                return this.ProcessProjectId<List<Channel>>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channels = project.Channels.Select(channel =>
                        {
                            var channelMeta = projectMeta.Channels.First(
                                current => current.Id == channel.Id);

                            return this.CreateChannelResponse(channel, channelMeta);
                        }).ToList();

                        _logger.LogInformation($"{message} Done.");

                        return channels;
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Gets the specified channel.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns></returns>
        [HttpGet("{projectId}/channels/{channelId}")]
        public ActionResult<Channel> Channels(
            string projectId,
            string channelId)
        {
            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests channel '{projectId}/{channelId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                return this.ProcessProjectId<Channel>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channel = project.Channels.FirstOrDefault(
                            current => current.Id == channelId);

                        if (channel == null)
                            return this.NotFound($"{projectId}/{channelId}");

                        var channelMeta = projectMeta.Channels.First(
                            current => current.Id == channel.Id);

                        _logger.LogInformation($"{message} Done.");

                        return this.CreateChannelResponse(channel, channelMeta);
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Gets a list of all datasets in the specified project and channel.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns></returns>
        [HttpGet("{projectId}/channels/{channelId}/datasets")]
        public ActionResult<List<Dataset>> Datasets(
            string projectId,
            string channelId)
        {
            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests datasets for channel '{projectId}/{channelId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                return this.ProcessProjectId<List<Dataset>>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channel = project.Channels.FirstOrDefault(
                            current => current.Id == channelId);

                        if (channel == null)
                            return this.NotFound($"{projectId}/{channelId}");

                        _logger.LogInformation($"{message} Done.");

                        return channel.Datasets.Select(dataset 
                            => this.CreateDatasetResponse(dataset))
                            .ToList();
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Gets the specified dataset.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="datasetId">The dataset identifier.</param>
        /// <returns></returns>
        [HttpGet("{projectId}/channels/{channelId}/datasets/{datasetId}")]
        public ActionResult<Dataset> Datasets(
            string projectId,
            string channelId,
            string datasetId)
        {
            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);
            datasetId = WebUtility.UrlDecode(datasetId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests dataset '{projectId}/{channelId}/{datasetId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                return this.ProcessProjectId<Dataset>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channel = project.Channels.FirstOrDefault(
                            current => current.Id == channelId);

                        if (channel == null)
                            return this.NotFound($"{projectId}/{channelId}");

                        var dataset = channel.Datasets.FirstOrDefault(
                           current => current.Id == datasetId);

                        if (dataset == null)
                            return this.NotFound($"{projectId}/{channelId}/{dataset}");

                        _logger.LogInformation($"{message} Done.");

                        return this.CreateDatasetResponse(dataset);
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        private Project CreateProjectResponse(ProjectInfo project, ProjectMetaInfo projectMeta)
        {
            return new Project()
            {
                Id = project.Id,
                ProjectStart = project.ProjectStart,
                ProjectEnd = project.ProjectEnd,
                ShortDescription = projectMeta.ShortDescription,
                LongDescription = projectMeta.LongDescription,
                ResponsiblePerson = projectMeta.ResponsiblePerson,
            };
        }

        private Channel CreateChannelResponse(ChannelInfo channel, ChannelMetaInfo channelMeta)
        {
            return new Channel()
            {
                Id = channel.Id,
                Name = channel.Name,
                Group = channel.Group,
                Unit = !string.IsNullOrWhiteSpace(channelMeta.Unit)
                        ? channelMeta.Unit
                        : channel.Unit,
                Description = channelMeta.Description,
                SpecialInfo = channelMeta.SpecialInfo,
                TransferFunctions = channelMeta.TransferFunctions.Any() 
                        ? channelMeta.TransferFunctions
                        : channel.TransferFunctions
            };
        }

        private Dataset CreateDatasetResponse(DatasetInfo dataset)
        {
            return new Dataset()
            {
                Id = dataset.Id,
                DataType = dataset.DataType,
                IsNative = dataset.IsNative
            };
        }

        private ActionResult<T> ProcessProjectId<T>(
            string projectId,
            string message,
            Func<ProjectInfo, ProjectMetaInfo, ActionResult<T>> action)
        {
            if (!Utilities.IsProjectAccessible(this.User, projectId, _databaseManager.Config.RestrictedProjects))
                return this.Unauthorized($"The current user is not authorized to access the project '{projectId}'.");

            var projectContainer = _databaseManager
               .Database
               .ProjectContainers
               .FirstOrDefault(container => container.Id == projectId);

            if (projectContainer != null)
            {
                var project = projectContainer.Project;
                var projectMeta = projectContainer.ProjectMeta;

                return action.Invoke(project, projectMeta);
            }
            else
            {
                _logger.LogInformation($"{message} Not found.");
                return this.NotFound(projectId);
            }
        }

        #endregion

        #region Records

        public record Project()
        {
            public string Id { get; set; }
            public DateTime ProjectStart { get; set; }
            public DateTime ProjectEnd { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }
            public string ResponsiblePerson { get; set; }
        }

        public record Channel()
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Group { get; set; }
            public string Unit { get; set; }
            public string Description { get; set; }
            public string SpecialInfo { get; set; }
            public List<TransferFunction> TransferFunctions { get; set; }
        }

        public record Dataset()
        {
            public string Id { get; set; }
            public OneDasDataType DataType { get; set; }
            public bool IsNative { get; set; }
        }

        #endregion
    }
}
