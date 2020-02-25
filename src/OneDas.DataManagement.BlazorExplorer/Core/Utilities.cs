using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public static class Utilities
    {
        public static string GetEnumLocalization(Enum enumValue)
        {
            return EnumerationDescription.ResourceManager.GetString(enumValue.GetType().Name + "_" + enumValue.ToString());
        }

        public static List<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
