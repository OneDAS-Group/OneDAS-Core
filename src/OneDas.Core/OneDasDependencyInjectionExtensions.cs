using OneDas;
using OneDas.Core.Engine;
using OneDas.Core.Serialization;
using OneDas.Extensibility;
using OneDas.PackageManagement;
using OneDas.Infrastructure;
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
            // options
            serviceCollection.Configure(configure);

            // common
            serviceCollection.AddLogging();
            serviceCollection.AddOptions();

            // engine
            serviceCollection.AddSingleton(typeof(IOneDasSerializer), typeof(OneDasSerializer));
            serviceCollection.AddSingleton(typeof(IOneDasProjectSerializer), typeof(OneDasProjectSerializer));
            serviceCollection.AddSingleton(typeof(IExtensionFactory), typeof(ExtensionFactory));
            serviceCollection.AddSingleton<OneDasEngine>();

            // package manager
            serviceCollection.AddSingleton<OneDasPackageManager>();
        }
    }
}