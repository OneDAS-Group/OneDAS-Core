using Microsoft.Extensions.Logging;
using System;

namespace OneDas.WebServer.Logging
{
    public class WebClientLoggerProvider : ILoggerProvider
    {
        private Func<string, LogLevel, bool> _filter;

        public WebClientLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new WebClientLogger(categoryName, _filter);
        }

        public void Dispose()
        {
            //
        }
    }
}
