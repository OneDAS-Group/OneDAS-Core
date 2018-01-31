using System;
using System.Linq;

namespace OneDas.Common
{
    public static class ReflectionHelper
    {
        public static T GetFirstAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(false).OfType<T>().FirstOrDefault();
        }
    }
}
