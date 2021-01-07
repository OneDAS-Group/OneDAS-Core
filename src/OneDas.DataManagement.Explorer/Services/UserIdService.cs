using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Services
{
    public class UserIdService
    {
        #region Fields

        private IHttpContextAccessor _httpContextAccessor;
        private AuthenticationStateProvider _authenticationStateProvider;

        #endregion

        #region Constructors

        public UserIdService(IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationStateProvider = authenticationStateProvider;
        }

        #endregion

        #region Properties

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;

        #endregion

        #region Methods

        public async Task<ClaimsPrincipal> GetUserAsync()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

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
