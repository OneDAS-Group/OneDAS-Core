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
    [Route("v1/api/resources")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerStateManager _stateManager;

        #endregion

        public ResourceController(OneDasExplorerStateManager stateManager,
                                  OneDasDatabaseManager databaseManager,
                                  ILoggerFactory loggerFactory)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        /// <summary>
        /// Gets a list of all accessible and visible campaigns.
        /// </summary>
        [HttpGet]
        public ActionResult<List<string>> Campaigns()
        {
            var campaignIds = _databaseManager.Database.CampaignContainers
                .Select(container => container.Id);

            campaignIds = campaignIds.Where(campaignId
                => Utilities.IsCampaignAccessible(this.User, campaignId, _databaseManager.Config.RestrictedCampaigns)
                && Utilities.IsCampaignVisible(this.User, campaignId, Constants.HiddenCampaigns));
    
            return campaignIds.ToList();
        }

        /// <summary>
        /// Gets a description of the specified campaign.
        /// </summary>
        /// <param name="id">The campaign identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<CampaignInfo> Campaigns(string id)
        {
            id = WebUtility.UrlDecode(id);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests campaign '{id}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // security check
                if (!Utilities.IsCampaignAccessible(this.User, id, _databaseManager.Config.RestrictedCampaigns))
                    return this.Unauthorized($"The current user is not authorized to access the campaign '{id}'.");

                var campaignContainer = _databaseManager
                   .Database
                   .CampaignContainers
                   .FirstOrDefault(container => container.Id == id);

                _logger.LogInformation($"{message} Done.");

                if (campaignContainer != null)
                    return campaignContainer.Campaign;
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
        /// <param name="campaignId">The campaign identifier.</param>
        /// <param name="variableId">The variable identifier.</param>
        /// <returns></returns>
        [HttpGet("{campaign-id}/variables/{variable-id}")]
        public ActionResult<VariableInfo> Variables(
            [FromRoute(Name = "campaign-id")] string campaignId,
            [FromRoute(Name = "variable-id")] string variableId)
        {
            campaignId = WebUtility.UrlDecode(campaignId);
            variableId = WebUtility.UrlDecode(variableId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests variable '{campaignId}/{variableId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // security check
                if (!_databaseManager.Database.TryFindVariableById(campaignId, variableId, out var variable))
                    return this.NotFound($"{campaignId}/{variableId}");

                var campaign = variable.Parent;

                if (!Utilities.IsCampaignAccessible(this.User, campaign.Id, _databaseManager.Config.RestrictedCampaigns))
                    return this.Unauthorized($"The current user is not authorized to access the campaign '{campaignId}'.");

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
        /// <param name="campaignId">The campaign identifier.</param>
        /// <param name="variableId">The variable identifier.</param>
        /// <param name="datasetId">The dataset identifier.</param>
        /// <returns></returns>
        [HttpGet("{campaign-id}/variables/{variable-id}/datasets/{dataset-id}")]
        public ActionResult<DatasetInfo> Variables(
            [FromRoute(Name = "campaign-id")] string campaignId,
            [FromRoute(Name = "variable-id")] string variableId,
            [FromRoute(Name = "dataset-id")] string datasetId)
        {
            campaignId = WebUtility.UrlDecode(campaignId);
            variableId = WebUtility.UrlDecode(variableId);
            datasetId = WebUtility.UrlDecode(datasetId);

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            var message = $"User '{userName}' ({remoteIpAddress}) requests dataset '{campaignId}/{variableId}/{datasetId}' ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // security check
                if (!_databaseManager.Database.TryFindDatasetById(campaignId, variableId, datasetId, out var dataset))
                    return this.NotFound($"{campaignId}/{variableId}/{datasetId}");

                var campaign = dataset.Parent.Parent;

                if (!Utilities.IsCampaignAccessible(this.User, campaign.Id, _databaseManager.Config.RestrictedCampaigns))
                    return this.Unauthorized($"The current user is not authorized to access the campaign '{campaignId}'.");

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
