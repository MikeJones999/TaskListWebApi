using System.ComponentModel.DataAnnotations;
using TaskList.Api.Domain.Users.Validation;

namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class UserRegister
    {
        [Required, StringLength(15, MinimumLength = 5)]
        public string UserName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        [Required, StrictEmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(100, MinimumLength = 12)]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
