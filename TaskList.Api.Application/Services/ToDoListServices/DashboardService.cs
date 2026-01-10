using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;
using TaskList.Api.Domain.Tasks.Enums;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Application.Services.ToDoListServices
{
    public class DashboardService : IDashboardService
    {
        private readonly ILogger<DashboardService> _logger;
        private readonly IToDoListRepository _repository;
        private readonly IMapper _mapper;

        public DashboardService(ILogger<DashboardService> logger, IToDoListRepository repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<DashboardResponse?> GetAllToDoListsForDashboardAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving all ToDoLists for user {UserId}", userId);
            try
            {
                IEnumerable<ToDoList> toDoLists = await _repository.GetAllByUserIdAsync(userId.ToString());
     
                List<ToDoItem> allItems = toDoLists.SelectMany(list => list.ToDoItems).ToList();
                if(!allItems.Any())
                {
                    _logger.LogInformation("No ToDoItems found for user {UserId}", userId);
                    return null;
                }
                DashboardResponse response = new DashboardResponse
                {
                    TaskListCount = toDoLists.Count(),
                    TotalTasksCount = allItems.Count,
                    
                    TasksNotStartedCount = CountProgressStatusTasks(allItems, ProgressStatus.NotStarted),
                    TasksInProgressCount = CountProgressStatusTasks(allItems, ProgressStatus.InProgress),
                    TasksDoneCount = CountProgressStatusTasks(allItems, ProgressStatus.Done),

                    TasksPriorityLow = CountPriorityTasks(allItems, PriorityStatus.Low),
                    TasksPriorityMedium = CountPriorityTasks(allItems, PriorityStatus.Medium),
                    TasksPriorityHigh = CountPriorityTasks(allItems, PriorityStatus.High)
                };


                _logger.LogInformation("Successfully retrieved dashboard data for user {UserId}: {TaskListCount} lists, {TotalTasksCount} tasks",
                    userId, response.TaskListCount, response.TotalTasksCount);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoLists for user {UserId}", userId);
                throw;
            }
        }

        private int CountProgressStatusTasks(IEnumerable<ToDoItem> items, ProgressStatus status)
        {
            return items.Count(item => item.Status == status);
        }

        private int CountPriorityTasks(IEnumerable<ToDoItem> items, PriorityStatus priority)
        {
            return items.Count(item => item.Priority == priority);
        }

       
    }
}
