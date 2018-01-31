using System.Collections.Generic;
using System.Linq;

namespace OneDas.Common
{
    public static class LinqHelper
    {
        public static IEnumerable<T> TrueDistinct<T>(this IEnumerable<T> inputSet)
        {
            return inputSet.GroupBy(x => x).Where(x => x.Count() == 1).SelectMany(x => x);
        }
    }
}
