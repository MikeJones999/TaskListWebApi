using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Domain.Tasks.Interfaces.Repositories
{
    public interface IToDoListRepository
    {
        Task<IEnumerable<ToDoList>> GetAllByUserIdAsync(string userId);
        Task<ToDoList?> GetByIdAsync(int id, string userId);
        Task<(ToDoList? ToDoList, int TotalItems)> GetByIdWithPaginationAsync(int id, string userId, int pageNumber, int pageSize, string sortBy, bool ascending);
        Task<ToDoList> CreateAsync(ToDoList toDoList);
        Task<ToDoList?> UpdateAsync(ToDoList toDoList);
        Task<bool> DeleteListAnddItemsAsync(int id, string userId);
        Task<bool> ExistsAsync(int id, string userId);
    }
}