using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Services
{
    public class UserIdService
    {
        #region Fields

        private AuthenticationStateProvider _authenticationStateProvider;

        #endregion

        #region Constructors

        public UserIdService(IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationStateProvider)
        {
            // https://github.com/dotnet/aspnetcore/issues/17585
            // https://github.com/dotnet/aspnetcore/issues/18183
            // https://github.com/dotnet/aspnetcore/issues/14090
            this.User = httpContextAccessor.HttpContext?.User;
            this.RemoteIpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress;

            _authenticationStateProvider = authenticationStateProvider;
        }

        #endregion

        #region Properties

        public ClaimsPrincipal User { get; }

        public IPAddress RemoteIpAddress { get; }

        #endregion

        #region Methods

        // Use this method only in conjunction with blazor invoked actions
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

            return $"{username} ({this.RemoteIpAddress})";
        }

        #endregion
    }
}
