using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Users.DTOs.UserProfileModels;
using TaskList.Api.Domian;

namespace TaskListWebApi.Controllers.UserControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : BaseAuthController
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(ILogger<BaseAuthController> logger, IUserProfileService userProfileService) : base(logger)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("Profile")]
        public async Task<ActionResult<ResponseDto<UserProfileResponse>>> GetUserProfile(Guid userId)
        {
            ValidateUserPassedInAgainstAuth(userId, nameof(this.GetUserProfile));

            ResponseDto<UserProfileResponse> extResponse = new ResponseDto<UserProfileResponse>();
            extResponse.ResponseData = await _userProfileService.GetUserProfileAsync(userId);
            if (extResponse.ResponseData is null)
            {
                UpdateResponse(extResponse);
                return Unauthorized(extResponse);
            }

            _logger.LogInformation("WHF - Request for User profile completed - userId {UserId} profile returned.", userId);
            return Ok(extResponse);
        }
    }
}
