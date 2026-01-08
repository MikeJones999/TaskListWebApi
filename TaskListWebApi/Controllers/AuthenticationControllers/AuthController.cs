using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Users.DTOs.AuthModels;

namespace TaskListWebApi.Controllers.AuthenticationControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseAuthController
    {
        private readonly IRefreshUserService _authUserService;

        public AuthController(ILogger<AuthController> logger, IRefreshUserService authUserService) : base(logger)
        {
            _authUserService = authUserService;
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<RefreshResponse>> RefreshLoggedInUserTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            RefreshResponse response = await _authUserService.RefreshTokenAsync(UserId, refreshTokenRequest);
            if (!response.IsAuthorised)
            {
                _logger.LogWarning(message: "Failed to refresh token for {Email}. Request {Method}", UserId, nameof(this.RefreshLoggedInUserTokenAsync));
                return Unauthorized(response);
            }
            return Ok(response);
        }
    }
}
