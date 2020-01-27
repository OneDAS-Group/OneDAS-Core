using System;

namespace OneDas.Hdf.Explorer.Core
{
    public class ProgressUpdatedEventArgs : EventArgs
    {
        public ProgressUpdatedEventArgs(double progress, string message)
        {
            this.Progress = progress;
            this.Message = message;
        }

        public double Progress { get; private set; }
        public string Message { get; private set; }
    }
}
