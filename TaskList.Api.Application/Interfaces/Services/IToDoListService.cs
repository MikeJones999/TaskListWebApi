using TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IToDoListService
    {
        Task<IEnumerable<ToDoListResponse>> GetAllToDoListsAsync(Guid userId);
        Task<ToDoListResponse?> GetToDoListByIdAsync(int id, Guid userId);
        Task<PaginatedToDoListResponse?> GetToDoListByIdWithPaginationAsync(int id, Guid userId, int pageNumber = 1, int pageSize = 10, string sortBy = "priority", bool ascending = true);
        Task<ToDoListResponse> CreateToDoListAsync(CreateToDoListRequest request, Guid userId);
        Task<ToDoListResponse?> UpdateToDoListAsync(UpdateToDoListRequest request, Guid userId);
        Task<bool> DeleteToDoListAsync(int id, Guid userId);
    }
}