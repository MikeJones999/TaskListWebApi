using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IToDoItemService
    {
        Task<IEnumerable<ToDoItemResponse>> GetAllToDoItemsAsync(Guid userId);
        Task<IEnumerable<ToDoItemResponse>> GetAllToDoItemsByListIdAsync(int toDoListId, Guid userId);
        Task<ToDoItemResponse?> GetToDoItemByIdAsync(int id, Guid userId);
        Task<ToDoItemResponse> CreateToDoItemAsync(CreateToDoItemRequest request, Guid userId);
        Task<ToDoItemResponse?> UpdateToDoItemAsync(UpdateToDoItemRequest request, Guid userId);
        Task<ToDoItemResponse?> UpdateToDoItemStatusAsync(UpdateToDoItemStatusRequest request, Guid userId);
        Task<ToDoItemResponse?> UpdateToDoItemPriorityAsync(UpdateToDoItemPriorityRequest request, Guid userId);
        Task<bool> DeleteToDoItemAsync(int id, Guid userId);
    }
}