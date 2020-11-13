using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public abstract record Job
    {
        /// <example>06f8eb30-5924-4a71-bdff-322f92343f5b</example>
        public Guid Id { get; init; } = Guid.NewGuid();
        /// <example>test@onedas.org</example>
        public string Owner { get; init; } = string.Empty;
    }

    public record ExportJob : Job
    {
        public ExportParameters Parameters { get; init; }
    }

    public record AggregationJob : Job
    {
        public AggregationParameters Parameters { get; init; }
    }

    public record JobStatus
    {
        public DateTime Start { get; init; }
        public TaskStatus Status { get; init; }
        public double Progress { get; init; }
        public string ProgressMessage { get; init; }
        public string ExceptionMessage { get; init; }
        public string Result { get; init; }
    }

    public record JobControl<T> where T : Job
    {
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        public event EventHandler Completed;

        public DateTime Start { get; init; }
        public double Progress { get; private set; }
        public string ProgressMessage { get; private set; }
        public T Job { get; init; }
        public Task<string> Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; init; }

        public void OnProgressUpdated(ProgressUpdatedEventArgs e)
        {
            this.Progress = e.Progress;
            this.ProgressMessage = e.Message;
            this.ProgressUpdated?.Invoke(this, e);
        }

        public void OnCompleted()
        {
            this.Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}
