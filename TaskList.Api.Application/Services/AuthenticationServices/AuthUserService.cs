using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Application.Services.UserServices;
using TaskList.Api.Domain.Users.DTOs.AuthModels;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Application.Services.AuthenticationServices
{
    public partial class AuthUserService : IRefreshUserService, ILoginAndRegisterUserService
    {
        private readonly ILogger<AuthUserService> _logger;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthUserService(ILogger<AuthUserService> logger, IConfiguration config, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<RegisterResponse> RegisterNewUserAsync(UserRegister userRegister)
        {
            RegisterResponse registerResponse = new RegisterResponse();
            await ValidateNewUserAsync(userRegister, registerResponse);
            if (registerResponse.Errors != null)
            {
                return registerResponse;
            }            

            ApplicationUser newUser = UserCreateService.CreateNewUser(userRegister);
            IdentityResult? createdUser = await _userManager.CreateAsync(newUser, userRegister.Password);
            if (createdUser is null)
            {
                AddErrors(registerResponse, "Error failed to create an account.");
                return registerResponse;
            }

            if (!createdUser.Succeeded)
            {
                foreach (IdentityError error in createdUser.Errors)
                {
                    AddErrors(registerResponse, error.Description);
                }
                return registerResponse;
            }

            await CheckRoleAndAddAsync(userRegister, newUser);

            registerResponse.IsSuccessful = true;
            return registerResponse;
        }

        public async Task<LoginResponse> LoginUserAsync(UserLogin userLogin)
        {
            LoginResponse response = new LoginResponse();

            if (userLogin is null || string.IsNullOrEmpty(userLogin.Email))
            {
                return response;
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(userLogin.Email);
            if (user == null)
            {
                return response;
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, userLogin.Password);
            if (!isValidPassword)
            {
                return response;
            }

            await GetUserTokenAndRefreshToken(response, user);
            return response;
        }

        public async Task<RefreshResponse> RefreshTokenAsync(Guid userId, RefreshTokenRequest refreshTokenRequest)
        {
            RefreshResponse response = new RefreshResponse();
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return response;
            }

            if (user.RefreshToken != refreshTokenRequest.RefreshToken)
            {
                return response;
            }

            await GetUserTokenAndRefreshToken(response, user);
            return response;
        }

        private async Task<RegisterResponse> ValidateNewUserAsync(UserRegister userRegister, RegisterResponse registerResponse)
        {
            if (userRegister == null)
            {
                _logger.LogWarning("Request for User failed - userRegister provided was null.  Request method: {Method}", nameof(this.ValidateNewUserAsync));
                AddErrors(registerResponse, "Important Information not present.");
                return registerResponse;
            }

            var existingUser = await _userManager.FindByEmailAsync(userRegister.Email);
            if (existingUser is not null)
            {
                _logger.LogWarning("Request for User failed - User: {UserId} already exists.  Request method: {Method}", userRegister.Email, nameof(this.ValidateNewUserAsync));
                AddErrors(registerResponse, "User already exists.");
                return registerResponse;
            }
            return registerResponse;
        }

        private async Task CheckRoleAndAddAsync(UserRegister userRegister, ApplicationUser newUser)
        {
            IdentityRole? checkUser = await _roleManager.FindByNameAsync(userRegister.Email);
            if (checkUser is null)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                await _userManager.AddToRoleAsync(newUser, "User");           
            }
        }

        private static void AddErrors(RegisterResponse registerResponse, string error)
        {
            registerResponse.Errors ??= new List<string>();
            registerResponse.Errors.Add(error);
        }

    }
}
