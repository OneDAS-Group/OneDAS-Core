using Blazor.Extensions;
using OneDas.Core.WebClient.ViewModel;
using System;
using System.Threading.Tasks;

namespace OneDas.Core.WebClient.Model
{
    public class SignalRService
    {
        #region Constructors

        public SignalRService(AppStateViewModel state)
        {
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
                            var a = await this.Connection.InvokeAsync<AppModel>("GetAppModel");

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
            return new HubConnectionBuilder()
                 .WithUrl("/webclienthub")
                 .Build();
        }

        #endregion
    }
}
