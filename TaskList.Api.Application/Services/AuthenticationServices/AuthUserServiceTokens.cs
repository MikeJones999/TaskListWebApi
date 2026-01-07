using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;   
using System.Text;
using TaskList.Api.Domain.Users.DTOs.AuthModels;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;
using TaskList.Api.Utility.StaticHelpers;

namespace TaskList.Api.Application.Services.AuthenticationServices
{
    public partial class AuthUserService
    {
        private async Task GetUserTokenAndRefreshToken(LoginResponse response, ApplicationUser user)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            response.Token = CreateTokenAndAddClaims(user, userRoles.ToList());
            response.IsAuthorised = true;
            if (string.IsNullOrWhiteSpace(user.RefreshToken) || user.RefreshTokenExpiry < DateTime.UtcNow.AddDays(1))
            {
                await CreateNewRefreshTokenAndAssignToUserAsync(response, user);
            }
            else
            {
                response.RefreshToken = user.RefreshToken;
            }
        }

        private string CreateTokenAndAddClaims(ApplicationUser user, List<string> roles)
        {
            List<Claim> claims = CreateNonRoleClaims(user);
            AddRolesClaimsToClaims(roles, claims);
            return CreateSecurityTokenFromTokenKey(claims);
        }

        private static List<Claim> CreateNonRoleClaims(ApplicationUser user)
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.GivenName, user.DisplayName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
        }

        private static void AddRolesClaimsToClaims(List<string> roles, List<Claim> claims)
        {
            if (roles.Any())
            {
                foreach (string role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
        }

        private string CreateSecurityTokenFromTokenKey(List<Claim> claims)
        {
            string tokenKey = _config.GetSection("JWT:Key").Value!;
            string issuer = _config.GetSection("JWT:Issuer").Value!;
            string audience = _config.GetSection("JWT:Audience").Value!;

            if (string.IsNullOrWhiteSpace(tokenKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                _logger.LogCritical(message: "Failed to extract JWT information from appsettings.json. Request {Method}", nameof(this.CreateSecurityTokenFromTokenKey));
                throw new Exception("Internal server Error.");
            }

            //add section to connect to key vault and get the secret
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                    );
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private async Task CreateNewRefreshTokenAndAssignToUserAsync(LoginResponse response, ApplicationUser user)
        {
            response.RefreshToken = RefreshTokenHelper.GenerateRefreshToken();
            user.RefreshToken = response.RefreshToken;
            string days = _config.GetSection("JWT:RefreshLifetimeInDays").Value ?? "7";
            int.TryParse(days, out int intParse);
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(intParse == 0 ? 7 : intParse);
            await _userManager.UpdateAsync(user);
        }
    }
}
