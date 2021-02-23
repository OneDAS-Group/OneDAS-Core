using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NJsonSchema.Annotations;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
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
        private UserIdService _userIdService;
        private DatabaseManager _databaseManager;

        #endregion

        #region Constructors

        public ProjectsController(DatabaseManager databaseManager,
                                    UserIdService userIdService,
                                    ILoggerFactory loggerFactory)
        {
            _userIdService = userIdService;
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a list of all accessible projects.
        /// </summary>
        [HttpGet]
        public ActionResult<List<Project>> GetProjects()
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

            var projectContainers = _databaseManager.Database.ProjectContainers;

            projectContainers = projectContainers.Where(projectContainer =>
            {
                var isProjectAccessible = Utilities.IsProjectAccessible(this.User, projectContainer.Id, _databaseManager.Database);
                var isProjectVisible = Utilities.IsProjectVisible(this.User, projectContainer.ProjectMeta, isProjectAccessible);

                return isProjectAccessible && isProjectVisible;
            }).ToList();

            return projectContainers.Select(projectContainer
                => this.CreateProjectResponse(projectContainer.Project, projectContainer.ProjectMeta))
                .ToList();
        }

        /// <summary>
        /// Gets the specified project.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        [HttpGet("{projectId}")]
        public ActionResult<Project> GetProject(string projectId)
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

            projectId = WebUtility.UrlDecode(projectId);

            // log
            var message = $"User '{_userIdService.GetUserId()}' requests project '{projectId}' ...";
            _logger.LogInformation(message);

            try
            {
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
        /// Gets the specified project availability.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="begin">Start date.</param>
        /// <param name="end">End date.</param>
        /// <param name="granularity">Granularity of the resulting array.</param>
        [HttpGet("{projectId}/availability")]
        public ActionResult<List<AvailabilityResult>> GetProjectAvailability(string projectId,
                                                                             [BindRequired][JsonSchemaDate] DateTime begin,
                                                                             [BindRequired][JsonSchemaDate] DateTime end,
                                                                             [BindRequired] AvailabilityGranularity granularity)
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

            projectId = WebUtility.UrlDecode(projectId);

            // log
            var message = $"User '{_userIdService.GetUserId()}' requests availability of project '{projectId}' ...";
            _logger.LogInformation(message);

            try
            {
                return this.ProcessProjectId<List<AvailabilityResult>>(projectId, message,
                    (project, projectMeta) =>
                    {
                        _logger.LogInformation($"{message} Done.");
                        return this.CreateAvailabilityResponse(project, begin, end, granularity);
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
        public ActionResult<List<Channel>> GetChannels(
            string projectId)
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

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
        public ActionResult<Channel> GetChannel(
            string projectId,
            string channelId)
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);

            // log
            var message = $"User '{_userIdService.GetUserId()}' requests channel '{projectId}/{channelId}' ...";
            _logger.LogInformation(message);

            try
            {
                return this.ProcessProjectId<Channel>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channel = project.Channels.FirstOrDefault(
                            current => current.Id == channelId);

                        if (channel == null)
                            channel = project.Channels.FirstOrDefault(
                                current => current.Name == channelId);

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
        public ActionResult<List<Dataset>> GetDatasets(
            string projectId,
            string channelId)
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);

            // log
            var message = $"User '{_userIdService.GetUserId()}' requests datasets for channel '{projectId}/{channelId}' ...";
            _logger.LogInformation(message);

            try
            {
                return this.ProcessProjectId<List<Dataset>>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channel = project.Channels.FirstOrDefault(
                            current => current.Id == channelId);

                        if (channel == null)
                            channel = project.Channels.FirstOrDefault(
                                current => current.Name == channelId);

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
        public ActionResult<Dataset> GetDataset(
            string projectId,
            string channelId,
            string datasetId)
        {
            if (_databaseManager.Database == null)
                return this.StatusCode(503, "The database has not been loaded yet.");

            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);
            datasetId = WebUtility.UrlDecode(datasetId);

            // log
            var message = $"User '{_userIdService.GetUserId()}' requests dataset '{projectId}/{channelId}/{datasetId}' ...";
            _logger.LogInformation(message);

            try
            {
                return this.ProcessProjectId<Dataset>(projectId, message,
                    (project, projectMeta) =>
                    {
                        var channel = project.Channels.FirstOrDefault(
                            current => current.Id == channelId);

                        if (channel == null)
                            channel = project.Channels.FirstOrDefault(
                                current => current.Name == channelId);

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

        private Project CreateProjectResponse(ProjectInfo project, ProjectMeta projectMeta)
        {
            return new Project()
            {
                Id = project.Id,
                Contact = projectMeta.Contact,
                ProjectStart = project.ProjectStart,
                ProjectEnd = project.ProjectEnd,
                ShortDescription = projectMeta.ShortDescription,
                LongDescription = projectMeta.LongDescription,
                IsQualityControlled = projectMeta.IsQualityControlled,
                License = projectMeta.License,
                LogBook = projectMeta.Logbook
            };
        }

        private List<AvailabilityResult> CreateAvailabilityResponse(ProjectInfo project, DateTime begin, DateTime end, AvailabilityGranularity granularity)
        {
            var dataReaders = _databaseManager.GetDataReaders(_userIdService.User, project.Id);

            return dataReaders.Select(dataReaderForUsing =>
            {
                using var dataReader = dataReaderForUsing;
                var availability = dataReader.GetAvailability(project.Id, begin, end, granularity);

                var registration = new DataReaderRegistration()
                {
                    RootPath = availability.DataReaderRegistration.RootPath,
                    DataReaderId = availability.DataReaderRegistration.DataReaderId,
                };

                return new AvailabilityResult()
                {
                    DataReaderRegistration = registration,
                    Data = availability.Data
                };
            }).ToList();
        }

        private Channel CreateChannelResponse(ChannelInfo channel, ChannelMeta channelMeta)
        {
            return new Channel()
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
        }

        private Dataset CreateDatasetResponse(DatasetInfo dataset)
        {
            return new Dataset()
            {
                Id = dataset.Id,
                DataType = dataset.DataType
            };
        }

        private ActionResult<T> ProcessProjectId<T>(
            string projectId,
            string message,
            Func<ProjectInfo, ProjectMeta, ActionResult<T>> action)
        {
            if (!Utilities.IsProjectAccessible(this.User, projectId, _databaseManager.Database))
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

        #region Types

        public record DataReaderRegistration
        {
            public string RootPath { get; set; }
            public string DataReaderId { get; set; }
        }

        public record AvailabilityResult
        {
            public DataReaderRegistration DataReaderRegistration { get; set; }
            public Dictionary<DateTime, double> Data { get; set; }
        }

        public record Project
        {
            public string Id { get; set; }
            public string Contact { get; set; }
            public DateTime ProjectStart { get; set; }
            public DateTime ProjectEnd { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }
            public bool IsQualityControlled { get; set; }
            public ProjectLicense License { get;set;}
            public List<string> LogBook { get; set; }
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
        }

        #endregion
    }
}
