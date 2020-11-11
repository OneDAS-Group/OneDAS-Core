using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OneDas.DataManagement.Explorer.Services
{
    public class UserIdService
    {
        #region Fields

        private IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructors

        public UserIdService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Properties

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;

        #endregion

        #region Methods

        public string GetUserId()
        {
            string username;

            if (this.User.Identity.IsAuthenticated)
                username = this.User.Identity.Name;
            else
                username = "anonymous";

            var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            return $"{username} ({remoteIpAddress})";
        }

        #endregion
    }
}
