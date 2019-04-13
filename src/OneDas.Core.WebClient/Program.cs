using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.JSInterop;

namespace OneDas.Core.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args)
                   .Build()
                   .Run();

            //JSRuntime.Current.InvokeAsync<object>("OnLoaded");
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
