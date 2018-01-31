using Microsoft.Extensions.Logging;
using OneDas.WebServer.Web;
using System;
using System.Threading.Tasks;

namespace OneDas.WebServer.Logging
{
    public class ClientMessageLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;

        public ClientMessageLogger(string categoryName, Func<string, LogLevel, bool> filter)
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
                Task.Run(() =>
                {
                    HomeController.SendMessage($"{ _categoryName }: { formatter(state, exception) }");
                });
            }  
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(_categoryName, logLevel);
        }
    }
}
