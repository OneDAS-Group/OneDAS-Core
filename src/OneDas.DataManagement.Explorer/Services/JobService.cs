using OneDas.DataManagement.Explorer.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Explorer.Services
{
    public class JobService<T> where T : Job
    {
        #region Fields

        private ConcurrentDictionary<Guid, JobControl<T>> _jobs;

        #endregion

        #region Constructors

        public JobService()
        {
            _jobs = new ConcurrentDictionary<Guid, JobControl<T>>();
        }

        #endregion

        #region Methods

        public bool TryAddJob(JobControl<T> jobControl)
        {
            return _jobs.TryAdd(jobControl.Job.Id, jobControl);
        }

        public bool TryGetJob(Guid key, out JobControl<T> jobControl)
        {
            return _jobs.TryGetValue(key, out jobControl);
        }

        public List<JobControl<T>> GetJobs()
        {
            // http://blog.i3arnon.com/2018/01/16/concurrent-dictionary-tolist/
            // https://stackoverflow.com/questions/41038514/calling-tolist-on-concurrentdictionarytkey-tvalue-while-adding-items
            return _jobs
                .ToArray()
                .Select(entry => entry.Value)
                .ToList();
        }

        public void Reset()
        {
            this.GetJobs()
                .ForEach(job => job.CancellationTokenSource.Cancel());

            _jobs.Clear();
        }

        #endregion
    }
}
