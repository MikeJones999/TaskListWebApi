using TaskList.Api.Domain.Users.DTOs.AuthModels;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Application.Services.UserServices
{
    public static class UserCreateService
    {
        public static ApplicationUser CreateNewUser(UserRegister userRegister)
        {
            ApplicationUser newUser = new ApplicationUser();
            newUser.DisplayName = string.IsNullOrWhiteSpace(userRegister.DisplayName) ? userRegister.UserName : userRegister.DisplayName;
            newUser.Email = userRegister.Email;
            newUser.UserName = userRegister.UserName;
            return newUser;
        }
    }
}
