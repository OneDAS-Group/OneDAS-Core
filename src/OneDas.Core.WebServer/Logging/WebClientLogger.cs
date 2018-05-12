using Microsoft.Extensions.Logging;
using System;

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
            //return _loggerExternalScopeProvider().Push(state);

            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (this.IsEnabled(logLevel))
            {
                switch (_categoryName)
                {
                    case "Nuget":
                        AdvancedBootloader.ClientPushService?.SendNugetMessage(formatter(state, exception));
                        break;

                    default:
                        AdvancedBootloader.ClientPushService?.SendClientMessage($"{ _categoryName }: { formatter(state, exception) }");
                        break;
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(_categoryName, logLevel);
        }
    }
}
