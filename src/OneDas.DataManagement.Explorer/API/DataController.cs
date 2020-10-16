using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneDas.Buffers;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace OneDas.DataManagement.Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerStateManager _stateManager;

        #endregion

        public DataController(OneDasExplorerStateManager stateManager,
                              OneDasDatabaseManager databaseManager,
                              ILoggerFactory loggerFactory)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        [HttpGet("campaigninfo")]
        public ActionResult<string> CampaignInfo(string id)
        {
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
                    return JsonSerializer.Serialize(campaignContainer);
                else
                    return this.NotFound(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                return this.StatusCode(500);
            }
        }

        [HttpGet("stream")]
        public ActionResult<string> StreamData(string channelName, CancellationToken cancellationToken)
        {
            var begin = DateTime.UtcNow.Date.AddDays(-1).AddHours(-1);
            var end = DateTime.UtcNow.Date.AddDays(-1);
            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            DateTime.SpecifyKind(begin, DateTimeKind.Utc);
            DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var message = $"User '{userName}' ({remoteIpAddress}) streams data: {begin.ToString("yyyy-MM-ddTHH:mm:ssZ")} to {end.ToString("yyyy-MM-ddTHH:mm:ssZ")} ...";
            _logger.LogInformation(message);

            try
            {
                _stateManager.CheckState();

                // dataset
                if (!_databaseManager.Database.TryFindDataset(channelName, out var dataset))
                    return this.NotFound($"Could not find channel with name '{channelName}'.");

                var campaign = (CampaignInfo)dataset.Parent.Parent;

                // security check
                if (!Utilities.IsCampaignAccessible(this.User, campaign.Id, _databaseManager.Config.RestrictedCampaigns))
                    return this.Unauthorized($"The current user is not authorized to access the campaign '{campaign.Id}'.");

                // dataReader
                using var dataReader = dataset.IsNative ? _databaseManager.GetNativeDataReader(campaign.Id) : _databaseManager.GetAggregationDataReader();

                // read and stream data
                var data = (IEnumerable<double>)new double[0];

                dataReader.Read(dataset, begin, end, 5 * 1000 * 1000UL, progressRecord =>
                {
                    double[] doubleData = default;
                    var dataRecord = progressRecord.DatasetToRecordMap.First().Value;

                    if (dataset.IsNative)
                        doubleData = BufferUtilities.ApplyDatasetStatus2(dataRecord.Dataset, dataRecord.Status);
                    else
                        doubleData = (double[])dataRecord.Dataset;

                    // avoid throwing an uncatched exception here because this would crash the app
                    // the task is cancelled anyway
                    try
                    {
                        data = data.Concat(doubleData);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning($"{message} Cancelled.");
                    }
                }, cancellationToken);

                _logger.LogInformation($"{message} Done.");

#warning only for testing
                var options = new JsonSerializerOptions() 
                { 
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals 
                };

                return JsonSerializer.Serialize(data.ToArray(), options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                return this.StatusCode(500);
            }
        }
    }
}
