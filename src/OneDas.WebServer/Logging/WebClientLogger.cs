using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace OneDas.WebServer.Logging
{
    public class WebClientLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;

        public WebClientLogger(string categoryName, Func<string, LogLevel, bool> filter)
        {
            _categoryName = categoryName;
            _filter = filter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (this.IsEnabled(logLevel))
            {
                AdvancedBootloader.ClientPushService.SendClientMessage($"{ _categoryName }: { formatter(state, exception) }");
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(_categoryName, logLevel);
        }
    }
}
