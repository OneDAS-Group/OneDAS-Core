using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OneDas.DataManagement.Explorer.Core
{
    public static class Utilities
    {
        public static bool IsCampaignAccessible(ClaimsPrincipal principal, CampaignInfo campaign, List<string> restrictedCampaigns)
        {
            var identitiy = principal.Identity;

            if (!restrictedCampaigns.Contains(campaign.Id))
            {
                return true;
            }
            else if (identitiy.IsAuthenticated)
            {
                var isAdmin = principal.HasClaim(claim => claim.Type == "IsAdmin" && claim.Value == "true");
                var canAccessCampaignContainer = principal.HasClaim(claim => claim.Type == "CanAccessCampaign"
                                                                 && claim.Value.Split(";").Any(campaignName => campaignName == campaign.Id));

                return isAdmin || canAccessCampaignContainer;
            }

            return false;
        }

        public static string GetEnumLocalization(Enum enumValue)
        {
            return EnumerationDescription.ResourceManager.GetString(enumValue.GetType().Name + "_" + enumValue.ToString());
        }

        public static List<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static string FormatByteCount(long byteCount)
        {
            if (byteCount >= 1000 * 1000 * 1000)
                return $"{(double)byteCount / 1000 / 1000 / 1000:G3} GB";
            else if (byteCount >= 1000 * 1000)
                return $"{(double)byteCount / 1000 / 1000:G3} MB";
            else if (byteCount >= 1000)
                return $"{(double)byteCount / 1000:G3} kB";
            else
                return $"{(double)byteCount:F0} B";
        }

        internal static bool IsCampaignAccessible(ClaimsPrincipal principal, object campaign, List<string> restrictedCampaigns)
        {
            throw new NotImplementedException();
        }
    }
}
