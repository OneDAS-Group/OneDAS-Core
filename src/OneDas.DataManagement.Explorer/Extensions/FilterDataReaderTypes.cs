using OneDas.DataManagement.Explorer.Core;
using OneDas.DataManagement.Explorer.Filters;
using System.Collections.Generic;
using System.Runtime.Loader;

namespace OneDas.DataManagement.Extensions
{
    public class FilterDataReaderLoadContext : AssemblyLoadContext
    {
        public FilterDataReaderLoadContext() : base(isCollectible: true) 
        {
            //
        }
    }

    public record FilterDataReaderCacheEntry(
        CodeDefinition FilterCodeDefinition,
        FilterDataReaderLoadContext LoadContext,
        FilterProviderBase FilterProvider,
        List<string> SupportedChanneIds);
}
