using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace OneDas.DataManagement.Explorer.Controllers
{
    [Route("v1/api/projects")]
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

        /// <summary>
        /// Gets a list of all accessible and visible projects.
        /// </summary>
        [HttpGet]
        public ActionResult<List<string>> Projects()
        {
            var projectIds = _databaseManager.Database.ProjectContainers
                .Select(container => container.Id);

            projectIds = projectIds.Where(projectId
                => Utilities.IsProjectAccessible(this.User, projectId, _databaseManager.Config.RestrictedProjects)
                && Utilities.IsProjectVisible(this.User, projectId, Constants.HiddenProjects));
    
            return projectIds.ToList();
        }

        /// <summary>
        /// Gets a description of the specified project.
        /// </summary>
        /// <param name="id">The project identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<ProjectInfo> Projects(string id)
        {
            id = WebUtility.UrlDecode(id);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests project '{id}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // security check
                if (!Utilities.IsProjectAccessible(this.User, id, _databaseManager.Config.RestrictedProjects))
                    return this.Unauthorized($"The current user is not authorized to access the project '{id}'.");

                var projectContainer = _databaseManager
                   .Database
                   .ProjectContainers
                   .FirstOrDefault(container => container.Id == id);

                _logger.LogInformation($"{message} Done.");

                if (projectContainer != null)
                    return projectContainer.Project;
                else
                    return this.NotFound(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Gets a description of the specified variable.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="variableId">The variable identifier.</param>
        /// <returns></returns>
        [HttpGet("{project-id}/variables/{variable-id}")]
        public ActionResult<VariableInfo> Variables(
            [FromRoute(Name = "project-id")] string projectId,
            [FromRoute(Name = "variable-id")] string variableId)
        {
            projectId = WebUtility.UrlDecode(projectId);
            variableId = WebUtility.UrlDecode(variableId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests variable '{projectId}/{variableId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // security check
                if (!_databaseManager.Database.TryFindVariableById(projectId, variableId, out var variable))
                    return this.NotFound($"{projectId}/{variableId}");

                var project = variable.Parent;

                if (!Utilities.IsProjectAccessible(this.User, project.Id, _databaseManager.Config.RestrictedProjects))
                    return this.Unauthorized($"The current user is not authorized to access the project '{projectId}'.");

                _logger.LogInformation($"{message} Done.");

                return variable;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }

        /// <summary>
        /// Gets a description of the specified dataset.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="variableId">The variable identifier.</param>
        /// <param name="datasetId">The dataset identifier.</param>
        /// <returns></returns>
        [HttpGet("{project-id}/variables/{variable-id}/datasets/{dataset-id}")]
        public ActionResult<DatasetInfo> Variables(
            [FromRoute(Name = "project-id")] string projectId,
            [FromRoute(Name = "variable-id")] string variableId,
            [FromRoute(Name = "dataset-id")] string datasetId)
        {
            projectId = WebUtility.UrlDecode(projectId);
            variableId = WebUtility.UrlDecode(variableId);
            datasetId = WebUtility.UrlDecode(datasetId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests dataset '{projectId}/{variableId}/{datasetId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // security check
                if (!_databaseManager.Database.TryFindDatasetById(projectId, variableId, datasetId, out var dataset))
                    return this.NotFound($"{projectId}/{variableId}/{datasetId}");

                var project = dataset.Parent.Parent;

                if (!Utilities.IsProjectAccessible(this.User, project.Id, _databaseManager.Config.RestrictedProjects))
                    return this.Unauthorized($"The current user is not authorized to access the project '{projectId}'.");

                _logger.LogInformation($"{message} Done.");

                return dataset;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }
    }
}
