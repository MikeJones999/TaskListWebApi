using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Users.DTOs.FileUploadModels;
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

        [HttpGet("Profile/Image")]
        public async Task<IActionResult> GetProfileImage(Guid userId)
        {
            ValidateUserPassedInAgainstAuth(userId, nameof(this.GetProfileImage));

            try
            {
                _logger.LogInformation("Request for user {UserId} profile image", userId);
                byte[]? imageBytes = await _userProfileService.GetProfileImageAsync(userId);

                if (imageBytes == null)
                {
                    _logger.LogWarning("Profile image not found for user {UserId}", userId);
                    return NotFound(new ResponseDto<string> 
                    { 
                        Success = false, 
                        Message = "Profile image not found" 
                    });
                }

                _logger.LogInformation("Successfully returned profile image for user {UserId}", userId);
                return File(imageBytes, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile image for user {UserId}", userId);
                return StatusCode(500, new ResponseDto<string> 
                { 
                    Success = false, 
                    Message = "Failed to retrieve profile image" 
                });
            }
        }

        [HttpPost("image")]
        public async Task<ActionResult<ResponseDto<UploadResult>>> UpdateProfileImage(List<IFormFile> files)
        {
            IFormFile? file = files.FirstOrDefault();
            ResponseDto<UploadResult> extResponse = new();

            if (file is null)
            {
                UpdateResponse(extResponse, message: "Unable to process request.");
                _logger.LogWarning("Request for User profile failed - userId Guid was empty. Request {Method}", nameof(this.UpdateProfileImage));
                return BadRequest(extResponse);
            }

            try
            {
                extResponse.ResponseData = await _userProfileService.UpdateProfileImageAsync(file, UserId);
                return Ok(extResponse);
            }
            catch (Exception ex)
            {
                UploadResult uploadResult = new UploadResult();
                uploadResult.IsSuccessful = false;
                UpdateResponse(extResponse, "Failed to upload Profile image file to storage.", false, uploadResult);
                _logger.LogWarning("{errorMessage}. Request {Method}", ex.Message, nameof(this.UpdateProfileImage));
                return BadRequest(extResponse);
            }
        }
    }
}
