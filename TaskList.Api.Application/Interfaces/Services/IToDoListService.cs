using TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IToDoListService
    {
        Task<IEnumerable<ToDoListResponse>> GetAllToDoListsAsync(Guid userId);
        Task<ToDoListResponse?> GetToDoListByIdAsync(int id, Guid userId);
        Task<ToDoListResponse> CreateToDoListAsync(CreateToDoListRequest request, Guid userId);
        Task<ToDoListResponse?> UpdateToDoListAsync(UpdateToDoListRequest request, Guid userId);
        Task<bool> DeleteToDoListAsync(int id, Guid userId);
    }
}