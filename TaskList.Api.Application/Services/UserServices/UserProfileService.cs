using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Users.DTOs.FileUploadModels;
using TaskList.Api.Domain.Users.DTOs.UserProfileModels;
using TaskList.Api.Domain.Users.Interfaces.Repositories;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Application.Services.UserServices
{
    public class UserProfileService : IUserProfileService   
    {
        private readonly ILogger<UserProfileService> _logger;
        private readonly IUserRepository _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileServices _fileService;

        public UserProfileService(ILogger<UserProfileService> logger,
            IUserRepository context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IFileServices blobServices)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _fileService = blobServices;
        }

        public async Task<UserProfileResponse?> GetUserProfileAsync(Guid userId)
        {
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userId, nameof(this.GetUserProfileAsync));
                return null;
            }

            UserProfileResponse response = _mapper.Map<UserProfileResponse>(user);
            response.HasProfileImage = user.HasProfilePicture;
            return response;
        } 

        public async Task<UploadResult> UpdateProfileImageAsync(IFormFile file, Guid userId)
        {
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userId);
            UploadResult uploadResult = new UploadResult();
            string untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;

            if (user == null)
            {
                _logger.LogWarning("Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userId, nameof(this.UpdateProfileImageAsync));
                uploadResult.ErrorMessage = "Failed to verify user - please log out and try again";
                return uploadResult;
            }        

            uploadResult.StoredFileNamed = GetStorageNameForProfileImage(user);
            await DeleteFileIfExistsAndSaveFileAsync(file, user, uploadResult);
            return uploadResult;
        }

        public async Task<byte[]?> GetProfileImageAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving profile image for user {UserId}", userId);
            
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userId, nameof(this.GetProfileImageAsync));
                return null;
            }

            if (!user.HasProfilePicture)
            {
                _logger.LogInformation("User {UserId} does not have a profile picture", userId);
                return null;
            }

            try
            {
                string storageName = GetStorageNameForProfileImage(user);
                byte[]? fileBytes = await _fileService.DownloadAsync(storageName);

                if (fileBytes == null)
                {
                    _logger.LogWarning("Profile image file not found for user {UserId} with storage name {StorageName}", userId, storageName);
                    user.HasProfilePicture = false;
                    await _context.SaveAsync();
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved profile image for user {UserId}", userId);
                }

                return fileBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile image for user {UserId}", userId);
                throw;
            }
        }

        private async Task DeleteFileIfExistsAndSaveFileAsync(IFormFile file, ApplicationUser user, UploadResult uploadResult)
        {
            if (user.HasProfilePicture)
            {
                string storageName = GetStorageNameForProfileImage(user);
                await _fileService.DeleteFileAsync(storageName);  //TODO move first then delete after successful upload
            }

            await _fileService.UploadAsync(file, uploadResult.StoredFileNamed!);
            user.HasProfilePicture = true;
            if (await _context.SaveAsync())
            {
                uploadResult.IsSuccessful = true;
            }
        }         

        private string GetStorageNameForProfileImage(ApplicationUser user)
        {
            return $"{user.Id}.profile.png";
        }
    }
}
