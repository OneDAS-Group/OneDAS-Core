using Microsoft.Extensions.Logging;

namespace OneDas.WebServer.Logging
{
    public class ClientMessageLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ClientMessageLogger();
        }

        public void Dispose()
        {
            //
        }
    }
}
