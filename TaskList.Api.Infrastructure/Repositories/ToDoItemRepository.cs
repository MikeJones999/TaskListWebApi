using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskList.Api.Domain.Tasks.Enums;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Tasks.Models;
using TaskList.Api.Infrastructure.Data;

namespace TaskList.Api.Infrastructure.Repositories
{
    public class ToDoItemRepository : IToDoItemRepository
    {
        private readonly TaskListDbContext _context;
        private readonly ILogger<ToDoItemRepository> _logger;

        public ToDoItemRepository(TaskListDbContext context, ILogger<ToDoItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ToDoItem>> GetAllByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching all ToDoItems for user {UserId}", userId);
            try
            {
                IEnumerable<ToDoItem> result = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .Where(t => t.ToDoList.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} ToDoItems for user {UserId}", result.Count(), userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDoItems for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ToDoItem>> GetAllByToDoListIdAsync(int toDoListId, string userId)
        {
            _logger.LogInformation("Fetching all ToDoItems for ToDoList {ToDoListId} and user {UserId}", toDoListId, userId);
            try
            {
                IEnumerable<ToDoItem> result = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .Where(t => t.ToDoListId == toDoListId && t.ToDoList.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} ToDoItems for ToDoList {ToDoListId}", result.Count(), toDoListId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDoItems for ToDoList {ToDoListId}", toDoListId);
                throw;
            }
        }

        public async Task<ToDoItem?> GetByIdAsync(int id, string userId)
        {
            _logger.LogInformation("Fetching ToDoItem {ToDoItemId} for user {UserId}", id, userId);
            try
            {
                ToDoItem? result = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id && t.ToDoList.UserId == userId);

                if (result == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for user {UserId}", id, userId);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ToDoItem> CreateToDoItemAsync(ToDoItem toDoItem)
        {
            _logger.LogInformation("Creating new ToDoItem for ToDoList {ToDoListId} with title '{Title}'", toDoItem.ToDoListId, toDoItem.Title);
            try
            {
                //TODO maybe worth checking if the user has an existing task with the same title before creating a new one
                //Additionally maybe worth stopping user from creating more than 100 tasks or add this to a config...
                toDoItem.CreatedAt = DateTime.UtcNow;
                await _context.ToDoItems.AddAsync(toDoItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created ToDoItem {ToDoItemId} for ToDoList {ToDoListId}", toDoItem.Id, toDoItem.ToDoListId);
                return toDoItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDoItem for ToDoList {ToDoListId}", toDoItem.ToDoListId);
                throw;
            }
        }

        public async Task<ToDoItem?> UpdateAsync(ToDoItem toDoItem)
        {
            _logger.LogInformation("Updating ToDoItem {ToDoItemId}", toDoItem.Id);
            try
            {
                ToDoItem? existing = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .FirstOrDefaultAsync(t => t.Id == toDoItem.Id && t.ToDoList.UserId == toDoItem.ToDoList.UserId);

                if (existing == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for update", toDoItem.Id);
                    return null;
                }

                existing.Title = toDoItem.Title;
                existing.Description = toDoItem.Description;
                existing.Type = toDoItem.Type;
                existing.Status = toDoItem.Status;
                existing.Priority = toDoItem.Priority;
                existing.ToDoListId = toDoItem.ToDoListId;

                if (toDoItem.Status == ProgressStatus.Done && existing.CompletedAt == null)
                {
                    existing.CompletedAt = DateTime.UtcNow;
                }
                else if (toDoItem.Status != ProgressStatus.Done)
                {
                    existing.CompletedAt = null;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated ToDoItem {ToDoItemId}", toDoItem.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDoItem {ToDoItemId}", toDoItem.Id);
                throw;
            }
        }

        public async Task<ToDoItem?> UpdateStatusAsync(int id, int status, string userId)
        {
            _logger.LogInformation("Updating status of ToDoItem {ToDoItemId} to {Status} for user {UserId}", id, status, userId);
            try
            {
                ToDoItem? existing = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .FirstOrDefaultAsync(t => t.Id == id && t.ToDoList.UserId == userId);

                if (existing == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for status update for user {UserId}", id, userId);
                    return null;
                }

                existing.Status = (ProgressStatus)status;

                if (status == (int)ProgressStatus.Done && existing.CompletedAt == null)
                {
                    existing.CompletedAt = DateTime.UtcNow;
                    _logger.LogInformation("ToDoItem {ToDoItemId} marked as completed at {CompletedAt}", id, existing.CompletedAt);
                }
                else if (status != (int)ProgressStatus.Done)
                {
                    existing.CompletedAt = null;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated status of ToDoItem {ToDoItemId} to {Status}", id, status);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status of ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ToDoItem?> UpdatePriorityAsync(int id, int priority, string userId)
        {
            _logger.LogInformation("Updating priority of ToDoItem {ToDoItemId} to {Priority} for user {UserId}", id, priority, userId);
            try
            {
                ToDoItem? existing = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .FirstOrDefaultAsync(t => t.Id == id && t.ToDoList.UserId == userId);

                if (existing == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for priority update for user {UserId}", id, userId);
                    return null;
                }

                existing.Priority = (PriorityStatus)priority;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated priority of ToDoItem {ToDoItemId} to {Priority}", id, priority);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating priority of ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            _logger.LogInformation("Deleting ToDoItem {ToDoItemId} for user {UserId}", id, userId);
            try
            {
                ToDoItem? toDoItem = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .FirstOrDefaultAsync(t => t.Id == id && t.ToDoList.UserId == userId);

                if (toDoItem == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for deletion for user {UserId}", id, userId);
                    return false;
                }

                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id, string userId)
        {
            _logger.LogDebug("Checking existence of ToDoItem {ToDoItemId} for user {UserId}", id, userId);
            try
            {
                bool exists = await _context.ToDoItems
                    .Include(t => t.ToDoList)
                    .AnyAsync(t => t.Id == id && t.ToDoList.UserId == userId);

                _logger.LogDebug("ToDoItem {ToDoItemId} exists for user {UserId}: {Exists}", id, userId, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> ToDoListBelongsToUserAsync(int toDoListId, string userId)
        {
            _logger.LogDebug("Checking if ToDoList {ToDoListId} belongs to user {UserId}", toDoListId, userId);
            try
            {
                bool belongs = await _context.ToDoLists
                    .AnyAsync(t => t.Id == toDoListId && t.UserId == userId);

                _logger.LogDebug("ToDoList {ToDoListId} belongs to user {UserId}: {Belongs}", toDoListId, userId, belongs);
                return belongs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if ToDoList {ToDoListId} belongs to user {UserId}", toDoListId, userId);
                throw;
            }
        }
    }
}