using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;

namespace OneDas.DataManagement.Explorer.Controllers
{
    [Route("api/v1/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;

        #endregion

        #region Constructors

        public DataController(OneDasDatabaseManager databaseManager,
                              ILoggerFactory loggerFactory)
        {
            _databaseManager = databaseManager;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        /// <summary>
        /// Gets the requested data.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="datasetId">The dataset identifier.</param>
        /// <param name="begin">Start date/time.</param>
        /// <param name="end">End date/time.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns></returns>

        [HttpGet]
        public IActionResult GetStream(
            [BindRequired] string projectId,
            [BindRequired] string channelId,
            [BindRequired] string datasetId,
            [BindRequired] DateTime begin,
            [BindRequired] DateTime end,
            CancellationToken cancellationToken)
        {
            projectId = WebUtility.UrlDecode(projectId);
            channelId = WebUtility.UrlDecode(channelId);
            datasetId = WebUtility.UrlDecode(datasetId);

            throw new Exception();

            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;

            // log
            string userName;

            if (this.User.Identity.IsAuthenticated)
                userName = this.User.Identity.Name;
            else
                userName = "anonymous";

            DateTime.SpecifyKind(begin, DateTimeKind.Utc);
            DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var message = $"User '{userName}' ({remoteIpAddress}) streams data: {begin.ToISO8601()} to {end.ToISO8601()} ...";
            _logger.LogInformation(message);

            try
            {
                // dataset
                var path = $"{projectId}/{channelId}/{datasetId}";

                if (!_databaseManager.Database.TryFindDataset(path, out var dataset))
                    return this.NotFound($"Could not find dataset with name '{path}'.");

                var project = (ProjectInfo)dataset.Parent.Parent;

                // security check
                if (!Utilities.IsProjectAccessible(this.User, project.Id, _databaseManager.Database))
                    return this.Unauthorized($"The current user is not authorized to access the project '{project.Id}'.");

                // dataReader
                using var dataReader = dataset.IsNative ? _databaseManager.GetNativeDataReader(project.Id) : _databaseManager.GetAggregationDataReader();

                // read data
                var stream = dataReader.ReadAsDoubleStream(dataset, begin, end, 1 * 1000 * 1000UL, cancellationToken);

                _logger.LogInformation($"{message} Done.");

                this.Response.Headers.ContentLength = stream.Length;
                return this.File(stream, "application/octet-stream", "data.bin");
            }
            catch (ValidationException ex)
            {
                return this.UnprocessableEntity(ex.GetFullMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message} {ex.GetFullMessage()}");
                throw;
            }
        }
    }
}
