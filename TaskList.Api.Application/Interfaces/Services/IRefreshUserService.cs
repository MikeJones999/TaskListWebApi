using TaskList.Api.Domain.Users.DTOs.AuthModels;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IRefreshUserService
    {
        Task<RefreshResponse> RefreshTokenAsync(Guid userId, RefreshTokenRequest refreshTokenRequest);
    }
}
