using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneDas.Core.WebClient.Model;
using OneDas.Core.WebClient.ViewModel;

namespace OneDas.Core.WebClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AppStateViewModel>();
            services.AddSingleton<SignalRService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
