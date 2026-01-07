namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsAuthorised { get; set; } = false;
    }
}
