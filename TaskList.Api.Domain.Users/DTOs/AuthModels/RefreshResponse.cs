namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class RefreshResponse : LoginResponse
    {
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
