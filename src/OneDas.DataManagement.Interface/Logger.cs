using System;

namespace OneDas.DataManagement.Interface
{
    public class Logger
    {
        Action<string> _logAction;

        public Logger(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void Log(string message)
        {
            _logAction?.Invoke(message);
        }
    }
}