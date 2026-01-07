using System.ComponentModel.DataAnnotations;

namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
