using System;

namespace OneDas.Main.Core
{
    public class SystemMessageReceivedEventArgs : EventArgs
    {
        public SystemMessageReceivedEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; set; }
    }
}