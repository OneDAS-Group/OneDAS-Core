using Microsoft.Extensions.Logging;

namespace OneDas.Hdf.VdsTool
{
    public class PwshLogger
    {
#warning Why is this wrapper necessary for powershell?
        private ILogger _logger;

        public PwshLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void LogTrace(string message)
        {
            _logger.LogTrace(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
