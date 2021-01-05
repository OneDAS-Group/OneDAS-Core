using OneDas.DataManagement.Explorer.Filters;
using System.Runtime.Loader;

namespace OneDas.DataManagement.Extensions
{
    public class FilterDataReaderLoadContext : AssemblyLoadContext
    {
        //
    }

    public record FilterDataReaderCacheEntry(
        FilterDataReaderLoadContext LoadContext,
        FilterProviderBase FilterProvider);
}
