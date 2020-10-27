using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    internal static class Constants
    {
        public static List<string> HiddenProjects
            => new List<string>() { "/IN_MEMORY/ALLOWED/TEST", "/IN_MEMORY/RESTRICTED/TEST" };
    }
}
