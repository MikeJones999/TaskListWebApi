using Microsoft.EntityFrameworkCore;
using TaskList.Api.Domain.Users.Interfaces.Repositories;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;
using TaskList.Api.Infrastructure.Data;

namespace TaskList.Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskListDbContext _dbContext;

        public UserRepository(TaskListDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ApplicationUser?> GetUserByUserIdAsync(Guid userId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
        }

        public async Task<ApplicationUser?> GetUserByUserIdReadonlyAsync(Guid userId)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }


}
