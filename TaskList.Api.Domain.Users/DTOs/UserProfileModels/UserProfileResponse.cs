namespace TaskList.Api.Domain.Users.DTOs.UserProfileModels
{
    public class UserProfileResponse
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool HasProfileImage { get; set; }
    }
}
