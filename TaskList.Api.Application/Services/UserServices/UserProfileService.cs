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
        private readonly IBlobServices _blobServices;

        public UserProfileService(ILogger<UserProfileService> logger,
            IUserRepository context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IBlobServices blobServices)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _blobServices = blobServices;
        }

        public async Task<UserProfileResponse?> GetUserProfileAsync(Guid userId)
        {
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("WFH - Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userId, nameof(this.GetUserProfileAsync));
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

            uploadResult.StoredFileNamed =  GetBlobNameForProfileImage(user);
            await DeleteBlobAndUploadFileAsync(file, user, uploadResult);
            return uploadResult;
        }   

        private async Task DeleteBlobAndUploadFileAsync(IFormFile file, ApplicationUser user, UploadResult uploadResult)
        {
            if (user.HasProfilePicture)
            {
                string blobName = GetBlobNameForProfileImage(user);
                await _blobServices.DeleteFileAsync(blobName);  //TODO move first then delete after successful upload
            }

            await _blobServices.UploadAsync(file, uploadResult.StoredFileNamed!);
            user.HasProfilePicture = true;
            if (await _context.SaveAsync())
            {
                uploadResult.IsSuccessful = true;
            }
        }         

        private string GetBlobNameForProfileImage(ApplicationUser user)
        {
            return $"{user.Id}.db4.png";
            //TODO requires config setting or based upon the user uniqueness (createdAtDate?
            //return $"{user.Id}.{user.CreatedDateUtc}.png";
            //CreatedAtDateUtc needs to be fedd up to UI
        }
    }
}
