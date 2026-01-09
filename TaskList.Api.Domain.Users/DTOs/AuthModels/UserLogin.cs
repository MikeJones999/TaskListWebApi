using System.ComponentModel.DataAnnotations;
using TaskList.Api.Domain.Users.Validation;

namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class UserLogin
    {
        [Required]
        [StrictEmailAddress]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
