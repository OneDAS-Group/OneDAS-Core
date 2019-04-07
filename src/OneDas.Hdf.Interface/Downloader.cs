using Microsoft.AspNetCore.SignalR.Client;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OneDas.Hdf.Interface
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


    public class Downloader
    {
        HubConnection _connection;
        Logger _logger;

        static Downloader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.Load(args.Name.Split(',').First());
        }

        public Downloader(string hostName, int port, Action<string> logAction)
        {
            _connection = this.BuildHubConnection(hostName, port, "broadcaster");

            _connection.On("SendProgress", (double percent, string message) =>
            {
                _logger?.Log($"{percent,3:0}%: {message}\n");
            });

            _logger = new Logger(logAction);
        }

        public void Connect()
        {
            try
            {
                _connection.StartAsync().Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<string> Download(DateTime dateTimeBegin, DateTime dateTimeEnd, FileGranularity fileGranularity, string sampleRateDescription, string campaignPath, List<string> variableNameSet)
        {
            try
            {
                return await _connection.InvokeAsync<string>("Download", dateTimeBegin, dateTimeEnd, "MAT73", fileGranularity, sampleRateDescription, campaignPath, variableNameSet);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public void Disconnect()
        {
            _connection.StopAsync().Wait();
        }

        private HubConnection BuildHubConnection(string hostName, int port, string path)
        {
            UriBuilder uriBuilder;

            uriBuilder = new UriBuilder()
            {
                Host = hostName,
                Port = port,
                Path = path
            };

            return new HubConnectionBuilder()
                 .WithUrl(uriBuilder.ToString())
                 .Build();
        }
    }
}
