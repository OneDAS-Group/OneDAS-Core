using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Explorer.Core
{
    public class JobService
    {
        #region Fields

        private ConcurrentDictionary<Guid, JobControl<ExportJob>> _exportJobs;

        #endregion

        #region Constructors

        public JobService()
        {
            _exportJobs = new ConcurrentDictionary<Guid, JobControl<ExportJob>>();
        }

        #endregion

        #region Methods

        public bool TryAddExportJob(JobControl<ExportJob> jobControl)
        {
            return _exportJobs.TryAdd(jobControl.Job.Id, jobControl);
        }

        public bool TryGetExportJob(Guid key, out JobControl<ExportJob> jobControl)
        {
            return _exportJobs.TryGetValue(key, out jobControl);
        }

        public List<JobControl<ExportJob>> GetExportJobs()
        {
            // http://blog.i3arnon.com/2018/01/16/concurrent-dictionary-tolist/
            // https://stackoverflow.com/questions/41038514/calling-tolist-on-concurrentdictionarytkey-tvalue-while-adding-items
            return _exportJobs
                .ToArray()
                .Select(entry => entry.Value)
                .ToList();
        }

        #endregion
    }
}
