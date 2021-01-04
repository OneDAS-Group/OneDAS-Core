using System.Reflection;
using System.Runtime.Loader;

namespace OneDas.DataManagement.Extensions
{
    public class FilterDataReaderLoadContext : AssemblyLoadContext
    {
        //
    }

    public record FilterDataReaderCacheEntry(
        FilterDataReaderLoadContext LoadContext, 
        MethodInfo MethodInfo);
}
