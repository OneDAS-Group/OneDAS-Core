using System;

namespace OneDas.DataManagement.Interface
{
    public class Logger
    {
        #region Fields

        private Action<string> _logAction;

        #endregion

        #region Constrcutors

        public Logger(Action<string> logAction)
        {
            _logAction = logAction;
        }

        #endregion

        #region Methods

        public void Log(string message)
        {
            _logAction?.Invoke(message);
        }

        #endregion
    }
}