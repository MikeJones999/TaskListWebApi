using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Tasks.Models;
using TaskList.Api.Infrastructure.Data;

namespace TaskList.Api.Infrastructure.Repositories
{
    public class ToDoListRepository : IToDoListRepository
    {
        private readonly TaskListDbContext _context;
        private readonly ILogger<ToDoListRepository> _logger;

        public ToDoListRepository(TaskListDbContext context, ILogger<ToDoListRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ToDoList>> GetAllByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching all ToDoLists for user {UserId}", userId);
            try
            {
                var result = await _context.ToDoLists
                    .Include(t => t.ToDoItems)
                    .Where(t => t.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
                
                _logger.LogInformation("Successfully retrieved {Count} ToDoLists for user {UserId}", result.Count(), userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDoLists for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ToDoList?> GetByIdAsync(int id, string userId)
        {
            _logger.LogInformation("Fetching ToDoList {ToDoListId} for user {UserId}", id, userId);
            try
            {
                var result = await _context.ToDoLists
                    .Include(t => t.ToDoItems)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
                
                if (result == null)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for user {UserId}", id, userId);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved ToDoList {ToDoListId} for user {UserId}", id, userId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ToDoList> CreateAsync(ToDoList toDoList)
        {
            _logger.LogInformation("Creating new ToDoList for user {UserId} with title '{Title}'", toDoList.UserId, toDoList.Title);
            try
            {
                await _context.ToDoLists.AddAsync(toDoList);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully created ToDoList {ToDoListId} for user {UserId}", toDoList.Id, toDoList.UserId);
                return toDoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDoList for user {UserId}", toDoList.UserId);
                throw;
            }
        }

        public async Task<ToDoList?> UpdateAsync(ToDoList toDoList)
        {
            _logger.LogInformation("Updating ToDoList {ToDoListId} for user {UserId}", toDoList.Id, toDoList.UserId);
            try
            {
                var existing = await _context.ToDoLists
                    .FirstOrDefaultAsync(t => t.Id == toDoList.Id && t.UserId == toDoList.UserId);

                if (existing == null)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for update for user {UserId}", toDoList.Id, toDoList.UserId);
                    return null;
                }

                existing.Title = toDoList.Title;
                existing.Description = toDoList.Description;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated ToDoList {ToDoListId} for user {UserId}", toDoList.Id, toDoList.UserId);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDoList {ToDoListId} for user {UserId}", toDoList.Id, toDoList.UserId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            _logger.LogInformation("Deleting ToDoList {ToDoListId} for user {UserId}", id, userId);
            try
            {
                var toDoList = await _context.ToDoLists
                    .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (toDoList == null)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for deletion for user {UserId}", id, userId);
                    return false;
                }

                _context.ToDoLists.Remove(toDoList);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully deleted ToDoList {ToDoListId} for user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id, string userId)
        {
            _logger.LogDebug("Checking existence of ToDoList {ToDoListId} for user {UserId}", id, userId);
            try
            {
                var exists = await _context.ToDoLists
                    .AnyAsync(t => t.Id == id && t.UserId == userId);
                
                _logger.LogDebug("ToDoList {ToDoListId} exists for user {UserId}: {Exists}", id, userId, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }
    }
}
