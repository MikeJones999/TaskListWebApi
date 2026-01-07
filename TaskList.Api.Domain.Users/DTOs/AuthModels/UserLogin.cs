using System.ComponentModel.DataAnnotations;

namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class UserLogin
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
