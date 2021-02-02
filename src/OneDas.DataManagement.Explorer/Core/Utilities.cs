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
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Fake authentication type"));

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
                var isAdmin = principal.HasClaim(claim => claim.Type == Claims.IS_ADMIN && claim.Value == "true");

                var canAccessProject = principal.HasClaim(claim => claim.Type == Claims.CAN_ACCESS_PROJECT &&
                                                          claim.Value.Split(";").Any(current => current == projectMeta.Id));

                var canAccessGroup = principal.HasClaim(claim => claim.Type == Claims.CAN_ACCESS_GROUP &&
                                                        claim.Value.Split(";").Any(group => projectMeta.GroupMemberships.Contains(group)));

                return isAdmin || canAccessProject || canAccessGroup;
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
                var isAdmin = principal.HasClaim(claim => claim.Type == Claims.IS_ADMIN && claim.Value == "true");

                var canEditProject = principal.HasClaim(claim => claim.Type == Claims.CAN_EDIT_PROJECT
                                                       && claim.Value.Split(";").Any(current => current == projectMeta.Id));

                return isAdmin || canEditProject;
            }

            return false;
        }

        public static bool IsProjectVisible(ClaimsPrincipal principal, ProjectMeta projectMeta, bool isProjectAccessible)
        {
            var identity = principal.Identity;

            // 1. project is visible if user is admin (this check must come before 2.)
            if (identity.IsAuthenticated)
            {
                if (principal.HasClaim(claim => claim.Type == Claims.IS_ADMIN && claim.Value == "true"))
                    return true;
            }

            // 2. test projects are hidden by default
            if (Constants.HiddenProjects.Contains(projectMeta.Id))
                return false;

            // 3. other projects

            // project is hidden, addtional checks required
            if (projectMeta.IsHidden)
                // ignore hidden property in case user has access to project
                return isProjectAccessible;

            // project is visible
            else
                return true;
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
