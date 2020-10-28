using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public record Job
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Owner { get; init; } = string.Empty;
    }

    public record ExportJob : Job
    {
        public ExportParameters Parameters { get; init; }
    }

    public record JobStatus
    {
        public DateTime Start { get; init; }
        public TaskStatus Status { get; init; }
        public double Progress { get; init; }
        public string ProgressMessage { get; init; } = string.Empty;
        public string ExceptionMessage { get; init; } = string.Empty;
    }

    public record JobControl<T> where T : Job
    {
        public DateTime Start { get; init; }
        public double Progress { get; set; }
        public string ProgressMessage { get; set; } = string.Empty;
        public T Job { get; init; }
        public Task Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; init; }
    }
}
