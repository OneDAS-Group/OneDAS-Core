using Microsoft.Extensions.Logging;
using System;

namespace OneDas.WebServer.Logging
{
    public class ClientMessageLoggerProvider : ILoggerProvider
    {
        private Func<string, LogLevel, bool> _filter;

        public ClientMessageLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ClientMessageLogger(categoryName, _filter);
        }

        public void Dispose()
        {
            //
        }
    }
}
