
using Microsoft.AspNetCore.Identity;

namespace TaskList.Api.Domain.Users.Models.AuthenticationModels
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedDateUtc { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? DateModifiedUtc { get; set; }       
        public string DisplayName { get; set; } = string.Empty;
        public bool HasProfilePicture { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
