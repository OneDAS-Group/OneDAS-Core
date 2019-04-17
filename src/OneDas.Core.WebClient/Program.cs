using Microsoft.AspNetCore.Blazor.Hosting;

namespace OneDas.Core.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args)
                   .Build()
                   .Run();

            // see also: https://github.com/aspnet/AspNetCore/issues/8117#issuecomment-475872863
            //JSRuntime.Current.InvokeAsync<object>("OnLoaded");
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
