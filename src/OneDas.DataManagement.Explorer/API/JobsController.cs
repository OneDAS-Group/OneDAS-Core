using GraphQL.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Services;
using OneDas.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Controllers
{
    [Route("api/v1/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        #region Fields

        private ILogger _logger;
        private IServiceProvider _serviceProvider;
        private OneDasDatabaseManager _databaseManager;
        private OneDasExplorerOptions _options;
        private JobService<ExportJob> _exportJobService;
        private JobService<AggregationJob> _aggregationJobService;

        #endregion

        #region Constructors

        public JobsController(
            OneDasDatabaseManager databaseManager,
            OneDasExplorerOptions options,
            JobService<ExportJob> exportJobService,
            JobService<AggregationJob> aggregationJobService,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            _databaseManager = databaseManager;
            _options = options;
            _serviceProvider = serviceProvider;
            _exportJobService = exportJobService;
            _aggregationJobService = aggregationJobService;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        #endregion

        #region Export Jobs

        /// <summary>
        /// Creates a new export job.
        /// </summary>
        /// <param name="parameters">Export parameters.</param>
        /// <returns></returns>
        [HttpPost("export")]
        public ActionResult<ExportJob> CreateExportJob(ExportParameters parameters)
        {
            parameters.Begin = parameters.Begin.ToUniversalTime();
            parameters.End = parameters.End.ToUniversalTime();

            // translate channel paths to datasets
            List<DatasetInfo> datasets;

            try
            {
                datasets = parameters.ChannelPaths.Select(channelPath =>
                {
                    if (!_databaseManager.Database.TryFindDataset(channelPath, out var dataset))
                        throw new ValidationException($"Could not find the channel with path '{channelPath}'.");

                    return dataset;
                }).ToList();
            }
            catch (ValidationException ex)
            {
                return this.UnprocessableEntity(ex.GetFullMessage());
            }

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

            var userIdService = _serviceProvider.GetRequiredService<UserIdService>();
            var dataService = _serviceProvider.GetRequiredService<DataService>();
            dataService.Progress.ProgressChanged += handler; // add handler before task starts
            jobControl.Task = dataService.ExportDataAsync(userIdService.GetUserId(), parameters, datasets, cancellationTokenSource.Token);

            Task.Run(async () =>
            {
                try
                {
                    await jobControl.Task;
                }
                finally
                {
                    dataService.Progress.ProgressChanged -= handler;
                }
            });

            if (_exportJobService.TryAddJob(jobControl))
                return this.Accepted($"{this.GetBasePath()}{this.Request.Path}/{jobControl.Job.Id}/status", jobControl.Job);
            else
                return this.Conflict();
        }

        /// <summary>
        /// Gets a list of all export jobs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        public ActionResult<List<ExportJob>> GetExportJobs()
        {
            return _exportJobService
                .GetJobs()
                .Select(jobControl => jobControl.Job)
                .ToList();
        }

        /// <summary>
        /// Gets the specified export job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("export/{jobId}")]
        public ActionResult<ExportJob> GetExportJob(Guid jobId)
        {
            if (_exportJobService.TryGetJob(jobId, out var jobControl))
                return jobControl.Job;
            else
                return this.NotFound(jobId);
        }

        /// <summary>
        /// Gets the status of the specified export job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("export/{jobId}/status")]
        public ActionResult<JobStatus> GetExportJobStatus(Guid jobId)
        {
            if (_exportJobService.TryGetJob(jobId, out var jobControl))
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
        [HttpDelete("export/{jobId}")]
        public ActionResult DeleteExportJob(Guid jobId)
        {
            if (_exportJobService.TryGetJob(jobId, out var jobControl))
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

        #endregion

        #region Aggregation Jobs

        /// <summary>
        /// Creates a new aggregation job.
        /// </summary>
        /// <param name="parameters">Aggregation parameters.</param>
        /// <returns></returns>
        [HttpPost("aggregation")]
        public ActionResult<AggregationJob> CreateAggregationJob(AggregationParameters parameters)
        {
            var remoteIpAddress = this.HttpContext.Connection.RemoteIpAddress;
            var cancellationTokenSource = new CancellationTokenSource();

            var jobControl = new JobControl<AggregationJob>()
            {
                Start = DateTime.UtcNow,
                Job = new AggregationJob()
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

            var userIdService = _serviceProvider.GetRequiredService<UserIdService>();
            var aggregationService = _serviceProvider.GetRequiredService<AggregationService>();
            aggregationService.Progress.ProgressChanged += handler; // add handler before task starts

            jobControl.Task = aggregationService.AggregateDataAsync(
                userIdService.GetUserId(), 
                _options.DataBaseFolderPath,
                parameters, 
                cancellationTokenSource.Token);

            Task.Run(async () =>
            {
                try
                {
                    await jobControl.Task;
                }
                finally
                {
                    aggregationService.Progress.ProgressChanged -= handler;
                }
            });

            if (_aggregationJobService.TryAddJob(jobControl))
                return this.Accepted($"{this.GetBasePath()}{this.Request.Path}/{jobControl.Job.Id}/status", jobControl.Job);
            else
                return this.Conflict();
        }

        /// <summary>
        /// Gets a list of all aggregation jobs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("aggregation")]
        public ActionResult<List<AggregationJob>> GetAggregationJobs()
        {
            return _aggregationJobService
                .GetJobs()
                .Select(jobControl => jobControl.Job)
                .ToList();
        }

        /// <summary>
        /// Gets the specified aggregation job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("aggregation/{jobId}")]
        public ActionResult<AggregationJob> GetAggregationJob(Guid jobId)
        {
            if (_aggregationJobService.TryGetJob(jobId, out var jobControl))
                return jobControl.Job;
            else
                return this.NotFound(jobId);
        }

        /// <summary>
        /// Gets the status of the specified export job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("aggregation/{jobId}/status")]
        public ActionResult<JobStatus> GetAggregationJobStatus(Guid jobId)
        {
            if (_exportJobService.TryGetJob(jobId, out var jobControl))
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
        [HttpDelete("aggregation/{jobId}")]
        public ActionResult DeleteAggregationJob(Guid jobId)
        {
            if (_exportJobService.TryGetJob(jobId, out var jobControl))
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

        #endregion

        #region Methods

        private string GetBasePath()
        {
            return $"{this.Request.Scheme}://{this.Request.Host}";
        }

        #endregion
    }
}
