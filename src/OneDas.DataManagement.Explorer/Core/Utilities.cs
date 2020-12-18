using OneDas.DataManagement.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace OneDas.DataManagement.Explorer.Core
{
    public static class Utilities
    {
        public static bool IsProjectAccessible(ClaimsPrincipal principal, string projectId, OneDasDatabase database)
        {
            var identity = principal.Identity;
            var projectContainer = database.ProjectContainers.First(current => current.Id == projectId);

            if (projectContainer.ProjectMeta.License.LicensingScheme == ProjectLicensingScheme.None)
            {
                return true;
            }
            else if (identity.IsAuthenticated)
            {
                var isAdmin = principal.HasClaim(claim => claim.Type == "IsAdmin" && claim.Value == "true");
                var canAccessProject = principal.HasClaim(claim => claim.Type == "CanAccessProject"
                                                       && claim.Value.Split(";").Any(current => current == projectId));

                return isAdmin || canAccessProject;
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

        public static IEnumerable<MethodInfo> GetMethodsBySignature(Type type, Type returnType, params Type[] parameterTypes)
        {
            return type.GetMethods().Where(method =>
            {
                if (method.ReturnType != returnType) return false;

                var parameters = method.GetParameters();

                if (parameterTypes == null || parameterTypes.Length == 0)
                    return parameters.Length == 0;

                else if (parameters.Length != parameterTypes.Length)
                    return false;

                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }

                return true;
            });
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
