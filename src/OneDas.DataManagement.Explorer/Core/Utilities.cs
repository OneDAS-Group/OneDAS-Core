using Microsoft.AspNetCore.Identity;
using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public static class Utilities
    {
        public static async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string username, UserManager<IdentityUser> userManager)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
                return null;

            var claims = await userManager.GetClaimsAsync(user);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            return principal;
        }

        public static bool IsProjectAccessible(ClaimsPrincipal principal, string projectId, OneDasDatabase database)
        {
            if (principal == null)
                return false;

            var identity = principal.Identity;
            var projectContainer = database.ProjectContainers.First(current => current.Id == projectId);
            var projectMeta = projectContainer.ProjectMeta;

            return Utilities.IsProjectAccessible(principal, projectMeta);
        }

        public static bool IsProjectAccessible(ClaimsPrincipal principal, ProjectMeta projectMeta)
        {
            if (principal == null)
                return false;

            var identity = principal.Identity;

            if (projectMeta.License.LicensingScheme == ProjectLicensingScheme.None)
            {
                return true;
            }
            else if (identity.IsAuthenticated)
            {
                var isAdmin = principal.HasClaim(claim => claim.Type == "IsAdmin" && claim.Value == "true");
                var canAccessProject = principal.HasClaim(claim => claim.Type == "CanAccessProject"
                                                       && claim.Value.Split(";").Any(current => current == projectMeta.Id));

                return isAdmin || canAccessProject;
            }

            return false;
        }

        public static bool IsProjectEditable(ClaimsPrincipal principal, ProjectMeta projectMeta)
        {
            if (principal == null)
                return false;

            var identity = principal.Identity;

            if (identity.IsAuthenticated)
            {
                var isAdmin = principal.HasClaim(claim => claim.Type == "IsAdmin" && claim.Value == "true");
                var canEditProject = principal.HasClaim(claim => claim.Type == "CanEditProject"
                                                       && claim.Value.Split(";").Any(current => current == projectMeta.Id));

                return isAdmin || canEditProject;
            }

            return false;
        }

        public static bool IsProjectVisible(ClaimsPrincipal principal, string projectId, List<string> hiddenProjects)
        {
            var identitiy = principal.Identity;

            if (!hiddenProjects.Contains(projectId))
            {
                return true;
            }
            else if (identitiy.IsAuthenticated)
            {
                var isAdmin = principal.HasClaim(claim => claim.Type == "IsAdmin" && claim.Value == "true");
                return isAdmin;
            }

            return false;
        }

        public static string GetEnumLocalization(Enum enumValue)
        {
            return EnumerationDescription.ResourceManager.GetString(enumValue.GetType().Name + "_" + enumValue.ToString());
        }

        public static string GetEnumIconName(Enum enumValue)
        {
            return EnumerationIconName.ResourceManager.GetString(enumValue.GetType().Name + "_" + enumValue.ToString());
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
    }
}
