using System.Collections.Generic;

namespace OneDas.DataManagement.Explorer.Core
{
    internal static class Constants
    {
        public static List<string> HiddenCampaigns
            => new List<string>() { "/IN_MEMORY/ALLOWED/TEST", "/IN_MEMORY/RESTRICTED/TEST" };
    }
}
