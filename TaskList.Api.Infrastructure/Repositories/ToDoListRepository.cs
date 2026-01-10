using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskList.Api.Domain.Tasks.Enums;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Tasks.Models;
using TaskList.Api.Infrastructure.Data;

namespace TaskList.Api.Infrastructure.Repositories
{
    public class ToDoListRepository : IToDoListRepository
    {
        private readonly TaskListDbContext _context;
        private readonly ILogger<ToDoListRepository> _logger;

        public ToDoListRepository(ILogger<ToDoListRepository> logger, TaskListDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ToDoList>> GetAllByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching all ToDoLists for user {UserId}", userId);
         
            var result = await _context.ToDoLists
                .Include(t => t.ToDoItems)
                .Where(t => t.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
                
            _logger.LogInformation("Successfully retrieved {Count} ToDoLists for user {UserId}", result.Count(), userId);
            return result;   
        }

        public async Task<ToDoList?> GetByIdAsync(int id, string userId)
        {
            _logger.LogInformation("Fetching ToDoList {ToDoListId} for user {UserId}", id, userId);
           
            var result = await _context.ToDoLists
                .Include(t => t.ToDoItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
                
            if (result != null)
            {          
                _logger.LogInformation("Successfully retrieved ToDoList {ToDoListId} for user {UserId}", id, userId);
            }
                
            return result;  
        }

        public async Task<(ToDoList? ToDoList, int TotalItems)> GetByIdWithPaginationAsync(int id, string userId, int pageNumber, int pageSize, string sortBy, bool ascending)
        {
            _logger.LogInformation("Getting Paginated ToDoList {ToDoListId} for user {UserId} - (Page: {PageNumber}, Size: {PageSize}, Sort: {SortBy} {SortDirection})",
                id, userId, pageNumber, pageSize, sortBy, ascending ? "ASC" : "DESC");

            ToDoList? toDoList = await GetByIdAsync(id, userId);

            if (toDoList == null)
            {
                _logger.LogWarning("ToDoList {ToDoListId} not found for user {UserId}", id, userId);
                return (null, 0);
            }

            int totalItems = await _context.ToDoItems
                .Where(item => item.ToDoListId == id)
                .CountAsync();

            if(totalItems == 0)
            {
                _logger.LogInformation("ToDoList {ToDoListId} does not have any tasks to paginate", id);
                return (toDoList, totalItems);
            }

            IQueryable<ToDoItem> itemsQuery = _context.ToDoItems
                .Where(item => item.ToDoListId == id);

            itemsQuery = sortBy.ToLower() switch
            {
                "priority" => ascending
                    ? itemsQuery.OrderBy(item => item.Priority)
                    : itemsQuery.OrderByDescending(item => item.Priority),
                "status" => ascending
                    ? itemsQuery.OrderBy(item => item.Status)
                    : itemsQuery.OrderByDescending(item => item.Status),                
                _ => ascending
                    ? itemsQuery.OrderBy(item => item.Id)
                    : itemsQuery.OrderByDescending(item => item.Id)
            };

            toDoList.ToDoItems = await itemsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();            

            _logger.LogInformation("Successfully retrieved ToDoList {ToDoListId} with {ItemCount} items on page {PageNumber} (Total: {TotalItems})", id, toDoList.ToDoItems.Count, pageNumber, totalItems);

            return (toDoList, totalItems);
        }

        public async Task<ToDoList> CreateAsync(ToDoList toDoList)
        {
            _logger.LogInformation("Creating new ToDoList for user {UserId} with title '{Title}'", toDoList.UserId, toDoList.Title);
           
            await _context.ToDoLists.AddAsync(toDoList);
            await _context.SaveChangesAsync();
                
            _logger.LogInformation("Successfully created ToDoList {ToDoListId} for user {UserId}", toDoList.Id, toDoList.UserId);
            return toDoList;         
        }

        public async Task<ToDoList?> UpdateAsync(ToDoList toDoList)
        {
            _logger.LogInformation("Updating ToDoList {ToDoListId} for user {UserId}", toDoList.Id, toDoList.UserId);
            
            ToDoList? existing = await _context.ToDoLists.FirstOrDefaultAsync(t => t.Id == toDoList.Id && t.UserId == toDoList.UserId);

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

        public async Task<bool> DeleteListAnddItemsAsync(int id, string userId)
        {
            _logger.LogInformation("Deleting ToDoList {ToDoListId} for user {UserId}", id, userId);
           
            var toDoList = await _context.ToDoLists.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

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

        public async Task<bool> ExistsAsync(int id, string userId)
        {
            _logger.LogDebug("Checking existence of ToDoList {ToDoListId} for user {UserId}", id, userId);
            try
            {
                bool exists = await _context.ToDoLists.AnyAsync(t => t.Id == id && t.UserId == userId);                
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
