using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Domain.Tasks.Interfaces.Repositories
{
    public interface IToDoListRepository
    {
        Task<IEnumerable<ToDoList>> GetAllByUserIdAsync(string userId);
        Task<ToDoList?> GetByIdAsync(int id, string userId);
        Task<ToDoList> CreateAsync(ToDoList toDoList);
        Task<ToDoList?> UpdateAsync(ToDoList toDoList);
        Task<bool> DeleteAsync(int id, string userId);
        Task<bool> ExistsAsync(int id, string userId);
    }
}