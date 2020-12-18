using System.Collections.Generic;
using System.Reflection;

namespace OneDas.DataManagement.Extensions
{
    public record FilterDataReaderCacheEntry
    {
        public FilterDataReaderLoadContext LoadContext { get; } = new FilterDataReaderLoadContext();
        public Dictionary<string, MethodInfo> FilterIdToMethodInfoMap { get; } = new Dictionary<string, MethodInfo>();
    }
}
