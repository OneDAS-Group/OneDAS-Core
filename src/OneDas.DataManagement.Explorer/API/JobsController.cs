using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Explorer.Core;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Controllers
{
    [Route("api/v1/data")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        #region Fields

        private ILogger _logger;
        private OneDasDatabaseManager _databaseManager;
        private StateManager _stateManager;
        private DataService _dataService;
        private JobService _jobService;

        #endregion

        #region Constructors

        public JobsController(
            StateManager stateManager,
            OneDasDatabaseManager databaseManager,
            DataService dataService,
            JobService jobService,
            ILoggerFactory loggerFactory)
        {
            _stateManager = stateManager;
            _databaseManager = databaseManager;
            _dataService = dataService;
            _jobService = jobService;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new export job.
        /// </summary>
        /// <param name="parameters">Export parameters.</param>
        /// <returns></returns>
        [HttpPost("/jobs/export")]
        public ActionResult<ExportJob> CreateExportJob(ExportParameters parameters)
        {
            parameters.Begin = parameters.Begin.ToUniversalTime();
            parameters.End = parameters.End.ToUniversalTime();

            // check state
            _stateManager.CheckState();

            // translate channel paths to datasets
            var datasets = parameters.ChannelPaths.Select(channelPath =>
            {
                if (!_databaseManager.Database.TryFindDataset(channelPath, out var dataset))
                    throw new Exception($"Could not find the channel with path '{channelPath}'.");

                return dataset;
            }).ToList();

            // check that there is anything to export
            if (!datasets.Any())
                return this.BadRequest("The list of channel paths is empty.");

            // security check
            var projectIds = datasets.Select(dataset => dataset.Parent.Parent.Id).Distinct();

            foreach (var projectId in projectIds)
            {
                if (!Utilities.IsProjectAccessible(this.HttpContext.User, projectId, _databaseManager.Config.RestrictedProjects))
                    return this.Unauthorized($"The current user is not authorized to access project '{projectId}'.");
            }

            //
            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;
            var cancellationTokenSource = new CancellationTokenSource();

            var jobControl = new JobControl<ExportJob>()
            {
                Start = DateTime.UtcNow,
                Job = new ExportJob()
                {
                    Owner = this.User.Identity.Name,
                    Parameters = parameters
                },
                CancellationTokenSource = cancellationTokenSource,
            };

            var handler = (EventHandler<ProgressUpdatedEventArgs>)((sender, e) =>
            {
                jobControl.Progress = e.Progress;
                jobControl.ProgressMessage = e.Message;
            });

            _dataService.Progress.ProgressChanged += handler; // add handler before task starts
            jobControl.Task = _dataService.ExportDataAsync(remoteIpAddress, parameters, datasets, cancellationTokenSource.Token);

            Task.Run(async () =>
            {
                try
                {
                    await jobControl.Task;
                }
                finally
                {
                    _dataService.Progress.ProgressChanged -= handler;
                }
            });

            if (_jobService.TryAddExportJob(jobControl))
                return this.Accepted($"{this.GetBasePath()}/jobs/export/{jobControl.Job.Id}/status", jobControl.Job);
            else
                return this.Conflict();
        }

        /// <summary>
        /// Gets a list of all export jobs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/jobs/export")]
        public ActionResult<List<ExportJob>> GetJobs()
        {
            return _jobService
                .GetExportJobs()
                .Select(jobControl => jobControl.Job)
                .ToList();
        }

        /// <summary>
        /// Gets the specified export job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("/jobs/export/{jobId}")]
        public ActionResult<ExportJob> GetJob(Guid jobId)
        {
            if (_jobService.TryGetExportJob(jobId, out var jobControl))
                return jobControl.Job;
            else
                return this.NotFound(jobId);
        }

        /// <summary>
        /// Gets the status of the specified export job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("/jobs/export/{jobId}/status")]
        public ActionResult<JobStatus> GetJobStatus(Guid jobId)
        {
            if (_jobService.TryGetExportJob(jobId, out var jobControl))
            {
                if (this.User.Identity.Name == jobControl.Job.Owner ||
                    jobControl.Job.Owner == null ||
                    this.User.HasClaim("IsAdmin", "true"))
                {
                    return new JobStatus()
                    {
                        Start = jobControl.Start,
                        Progress = jobControl.Progress,
                        ProgressMessage = jobControl.ProgressMessage,
                        Status = jobControl.Task.Status,
                        ExceptionMessage = jobControl.Task.Exception != null
                            ? jobControl.Task.Exception.GetFullMessage()
                            : string.Empty,
                        Result = jobControl.Task.Status == TaskStatus.RanToCompletion 
                            ? $"{this.GetBasePath()}/{jobControl.Task.Result}"
                            : null
                    };
                }
                else
                {
                    return this.Unauthorized($"The current user is not authorized to cancel the job '{jobControl.Job.Id}'.");
                }
            }
            else
            {
                return this.NotFound(jobId);
            }
        }

        /// <summary>
        /// Cancels the specified job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete("/jobs/export/{jobId}")]
        public ActionResult DeleteJob(Guid jobId)
        {
            if (_jobService.TryGetExportJob(jobId, out var jobControl))
            {
                if (this.User.Identity.Name == jobControl.Job.Owner || 
                    jobControl.Job.Owner == null ||
                    this.User.HasClaim("IsAdmin", "true"))
                {
                    jobControl.CancellationTokenSource.Cancel();
                    return this.Accepted();
                }
                else
                {
                    return this.Unauthorized($"The current user is not authorized to cancel the job '{jobControl.Job.Id}'.");
                }
            }
            else
            {
                return this.NotFound(jobId);
            }
        }

        private string GetBasePath()
        {
            return $"{this.Request.Scheme}://{this.Request.Host}";
        }

        #endregion
    }
}
