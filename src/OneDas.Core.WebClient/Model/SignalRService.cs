using Blazor.Extensions;
using Microsoft.JSInterop;
using OneDas.Core.WebClient.ViewModel;
using System;
using System.Threading.Tasks;

namespace OneDas.Core.WebClient.Model
{
    public class SignalRService
    {
        #region Fields

        IJSRuntime _jsRuntime;

        #endregion

        #region Constructors

        public SignalRService(AppStateViewModel state, IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;

            this.Connection = this.BuildHubConnection();

            this.Connection.OnClose(e =>
            {
                return Task.Run(async () =>
                {
                    state.IsConnected = false;

                    while (true)
                    {
                        try
                        {
                            await this.Connection.StartAsync();
                            Console.WriteLine("Connected.");
                            var a = await this.Connection.InvokeAsync<AppModel>("GetAppModel");
                            Console.WriteLine("App model received.");

                            state.IsConnected = true;

                            break;
                        }
                        catch
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3));
                        }
                    }
                });
            });

            this.Connection.StartAsync();
        }

        #endregion

        #region Properties

        public HubConnection Connection { get; }

        #endregion

        #region Methods

        private HubConnection BuildHubConnection()
        {
            // TODO: replace magic string with options?
            return new HubConnectionBuilder(_jsRuntime)
                 .WithUrl("/webclienthub")
                 .Build();
        }

        #endregion
    }
}
