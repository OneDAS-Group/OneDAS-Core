using Microsoft.Extensions.Logging;
using OneDas.WebServer.Web;
using System;
using System.Threading.Tasks;

namespace OneDas.WebServer.Logging
{
    public class ClientMessageLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Task.Run(() => HomeController.SendClientMessage(formatter(state, exception)));
        }
    }
}
