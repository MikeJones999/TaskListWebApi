using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;

namespace TaskList.Api.Application.Services.ToDoListServices
{
    public interface IDashboardService
    {
        Task<DashboardResponse?> GetAllToDoListsForDashboardAsync(Guid userId);
    }
}