using OneDas.Engine.Core;
using OneDas.Engine.Serialization;
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

            serviceCollection.AddSingleton(typeof(IOneDasSerializer), typeof(OneDasSerializer));
            serviceCollection.AddSingleton(typeof(IOneDasProjectSerializer), typeof(OneDasProjectSerializer));
            serviceCollection.AddSingleton(typeof(IPluginProvider), typeof(PluginProvider));
            serviceCollection.AddSingleton<OneDasEngine>();
        }
    }
}