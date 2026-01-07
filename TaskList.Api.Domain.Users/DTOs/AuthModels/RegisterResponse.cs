namespace TaskList.Api.Domain.Users.DTOs.AuthModels
{
    public class RegisterResponse
    {
        public bool IsSuccessful { get; set; }
        public List<string>? Errors { get; set; } = null;
    }
}
