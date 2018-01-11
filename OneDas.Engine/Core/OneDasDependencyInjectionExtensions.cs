using OneDas.Engine.Core;
using OneDas.Infrastructure;
using OneDas.Plugin;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OneDasDependencyInjectionExtensions
    {
        public static void AddOneDas(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddOneDas(oneDasOptions => { });
        }

        public static void AddOneDas(this IServiceCollection serviceCollection, Action<OneDasOptions> configure)
        {
            serviceCollection.Configure(configure);

            serviceCollection.AddSingleton<PluginManager>();
            serviceCollection.AddSingleton<OneDasEngine>();
        }
    }
}