using Microsoft.AspNetCore.Http;
using TaskList.Api.Domain.Users.DTOs.FileUploadModels;
using TaskList.Api.Domain.Users.DTOs.UserProfileModels;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse?> GetUserProfileAsync(Guid userId);
        Task<UploadResult> UpdateProfileImageAsync(IFormFile file, Guid userId);
    }
}
