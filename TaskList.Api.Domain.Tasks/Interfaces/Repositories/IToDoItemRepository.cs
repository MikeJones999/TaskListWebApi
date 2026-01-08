using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Domain.Tasks.Interfaces.Repositories
{
    public interface IToDoItemRepository
    {
        Task<IEnumerable<ToDoItem>> GetAllByUserIdAsync(string userId);
        Task<IEnumerable<ToDoItem>> GetAllByToDoListIdAsync(int toDoListId, string userId);
        Task<ToDoItem?> GetByIdAsync(int id, string userId);
        Task<ToDoItem> CreateToDoItemAsync(ToDoItem toDoItem);
        Task<ToDoItem?> UpdateAsync(ToDoItem toDoItem);
        Task<ToDoItem?> UpdateStatusAsync(int id, int status, string userId);
        Task<ToDoItem?> UpdatePriorityAsync(int id, int priority, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<bool> ExistsAsync(int id, string userId);
        Task<bool> ToDoListBelongsToUserAsync(int toDoListId, string userId);
    }
}