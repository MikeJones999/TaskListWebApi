using System;
using System.Collections.Generic;
using System.Text;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Domain.Users.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByUserIdAsync(Guid userId);
        Task<bool> SaveAsync();
        Task<ApplicationUser?> GetUserByUserIdReadonlyAsync(Guid userId);
    }
}
