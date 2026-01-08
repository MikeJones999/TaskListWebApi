using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskList.Api.Application.ExceptionHandling.CustomHandler;
using TaskList.Api.Domain.Users.Interfaces.Repositories;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskListWebApi.Middleware
{
    public class JwtMiddlewareUserExtraction
    {
        private readonly RequestDelegate _next;

        public JwtMiddlewareUserExtraction(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration, ILogger<JwtMiddlewareUserExtraction> logger, IUserRepository dbContext)
        {
            string? token = context.Request.Headers[JwtMiddlewareRoutes.Authorisation].ToString().Replace(JwtMiddlewareRoutes.Bearer, "");
            string? path = context.Request.Path.Value;
            if (!string.IsNullOrEmpty(token) && !JwtMiddlewareRoutes.GetListOfPathsToIgnore().Contains(path))
            {
                try
                {
                    ClaimsPrincipal? claimsPrincipal = ValidateJwtToken(token, configuration);
                    if (claimsPrincipal == null)
                    {
                        logger.LogWarning(message: "Failed to extract claimsPrincipals from JWT. Request {Method}", nameof(this.InvokeAsync));
                        throw new JwtTokenValidationException();
                    }
                    
                    // Set the validated claims principal to context.User
                    context.User = claimsPrincipal;
                    
                    string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    await ExtractUserAndUserIdAndAddToContextAsync(context, logger, userId, dbContext);
                }
                catch (JwtTokenValidationException ex)
                {
                    logger.LogWarning("{Message}. Request method: {Method}", ex.Message, nameof(this.InvokeAsync));
                    throw new JwtTokenValidationException($"Token is not valid.");
                }
                catch (Exception)
                {
                    throw new JwtTokenValidationException($"Token is not valid.");
                }
            }

            await _next(context);
        }

        private async Task ExtractUserAndUserIdAndAddToContextAsync(HttpContext context, ILogger<JwtMiddlewareUserExtraction> logger, string? userId, IUserRepository dbContext)
        {
            Guid.TryParse(userId, out Guid userIdGuid);
            if (string.IsNullOrEmpty(userId) && userIdGuid != Guid.Empty)
            {
                throw new JwtTokenValidationException("No user Token Found");
            }

            //Redis Cache call for read only use.
            ApplicationUser? user = await dbContext.GetUserByUserIdReadonlyAsync((Guid)userIdGuid);
            if (user == null)
            {
                throw new JwtTokenValidationException("User doesnt exist");
            }

            context.Items[JwtMiddlewareRoutes.UserId] = userId;
            context.Items[JwtMiddlewareRoutes.UserName] = user.UserName;
        }


        public ClaimsPrincipal ValidateJwtToken(string token, IConfiguration configuration)
        {
            try
            {
                string key = configuration.GetSection("JWT:Key").Value!;
                string issuer = configuration.GetSection("JWT:Issuer").Value!;
                string audience = configuration.GetSection("JWT:Audience").Value!;

                JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal? claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateActor = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience

                }, out SecurityToken validatedToken);

                return claimsPrincipal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new JwtTokenValidationException("Token has expired.");
            }
        }
    }

    public static class CustomJwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomJwtMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddlewareUserExtraction>();
        }
    }
}
