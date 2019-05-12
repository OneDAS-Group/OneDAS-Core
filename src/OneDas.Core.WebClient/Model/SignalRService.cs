using Microsoft.AspNetCore.SignalR.Client;
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

            this.Connection.Closed += e =>
            {
                return Task.Run(async () =>
                {
                    state.IsConnected = false;

                    while (true)
                    {
                        try
                        {
                            await this.Connect(state);
                            break;
                        }
                        catch
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3));
                        }
                    }
                });
            };

            try
            {
                _ = this.Connect(state);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exceptionne");
                Console.WriteLine(ex.ToString());
                //
            }           
        }

        #endregion

        #region Properties

        public HubConnection Connection { get; }

        #endregion

        #region Methods

        private async Task Connect(AppStateViewModel state)
        {
            Console.WriteLine("A.");
            await this.Connection.StartAsync();
            Console.WriteLine("Connected.");
            try
            {
                var a = await this.Connection.InvokeAsync<AppModel>("GetAppModel");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ohh Ohhhh: " + ex.ToString());
                throw;
            }

            Console.WriteLine("B.");
            Console.WriteLine("App model received.");
            state.IsConnected = true;
            Console.WriteLine("C.");
        }

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
