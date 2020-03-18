using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Core
{
    public class JwtService<TUser> where TUser : class
    {
        #region Fields

        private static JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private UserManager<TUser> _userManager;
        private SignInManager<TUser> _signInManager;

        #endregion

        #region Constructors

        public JwtService(UserManager<TUser> userManager, SignInManager<TUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion

        #region Methods

        public async Task<string> GenerateTokenAsync(UserCredentials credentials)
        {
            var result = string.Empty;
            var user = await _userManager.FindByNameAsync(credentials.Username);

            if (user != null)
            {
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, credentials.Password, false);

                if (signInResult.Succeeded)
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    var signingCredentials = new SigningCredentials(Startup.SecurityKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(issuer: "OneDAS Explorer",
                                                     audience: signingCredentials.Algorithm,
                                                     claims: claims,
                                                     expires: DateTime.UtcNow.AddSeconds(30),
                                                     signingCredentials: signingCredentials);

                    result = _tokenHandler.WriteToken(token);
                }
            }

            return result;
        }

        #endregion
    }
}
