using System;

namespace OneDas.Plugin
{
    public class SendReportEventArgs : EventArgs
    {
        public SendReportEventArgs(string id, string message)
        {
            this.Id = id;
            this.Message = message;
        }

        public string Id { get; private set; }
        public string Message { get; private set; }
    }
}
