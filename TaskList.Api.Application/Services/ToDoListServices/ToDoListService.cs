using Microsoft.Extensions.Logging;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Application.Services.ToDoListServices
{
    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _repository;
        private readonly ILogger<ToDoListService> _logger;

        public ToDoListService(IToDoListRepository repository, ILogger<ToDoListService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ToDoListResponse>> GetAllToDoListsAsync(Guid userId)
        {
            _logger.LogInformation("Service: Retrieving all ToDoLists for user {UserId}", userId);
            try
            {
                var toDoLists = await _repository.GetAllByUserIdAsync(userId.ToString());
                var response = toDoLists.Select(MapToResponse).ToList();
                
                _logger.LogInformation("Successfully retrieved {Count} ToDoLists for user {UserId}", response.Count, userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoLists for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ToDoListResponse?> GetToDoListByIdAsync(int id, Guid userId)
        {
            _logger.LogInformation("Retrieving ToDoList {ToDoListId} for user {UserId}", id, userId);
            try
            {
                var toDoList = await _repository.GetByIdAsync(id, userId.ToString());
                if (toDoList == null)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for user {UserId}", id, userId);
                    return null;
                }
                
                var response = MapToResponse(toDoList);
                _logger.LogInformation("Successfully retrieved ToDoList {ToDoListId} for user {UserId}", id, userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ToDoListResponse> CreateToDoListAsync(CreateToDoListRequest request, Guid userId)
        {
            _logger.LogInformation("Creating ToDoList with title '{Title}' for user {UserId}", request.Title, userId);
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("Attempted to create ToDoList with empty title for user {UserId}", userId);
                throw new ArgumentException("Title is required", nameof(request.Title));
            }

            try
            {
                var toDoList = new ToDoList
                {
                    Title = request.Title.Trim(),
                    Description = request.Description?.Trim() ?? string.Empty,
                    UserId = userId.ToString()
                };

                var created = await _repository.CreateAsync(toDoList);
                var response = MapToResponse(created);
                
                _logger.LogInformation("Successfully created ToDoList {ToDoListId} for user {UserId}", created.Id, userId);
                return response;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDoList for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ToDoListResponse?> UpdateToDoListAsync(UpdateToDoListRequest request, Guid userId)
        {
            _logger.LogInformation("Updating ToDoList {ToDoListId} for user {UserId}", request.Id, userId);
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("Attempted to update ToDoList {ToDoListId} with empty title for user {UserId}", request.Id, userId);
                throw new ArgumentException("Title is required", nameof(request.Title));
            }

            try
            {
                //var exists = await _repository.ExistsAsync(request.Id, userId.ToString());
                //if (!exists)
                //{
                //    _logger.LogWarning("ToDoList {ToDoListId} not found for update for user {UserId}", request.Id, userId);
                //    return null;
                //}

                var toDoList = new ToDoList
                {
                    Id = request.Id,
                    Title = request.Title.Trim(),
                    Description = request.Description?.Trim() ?? string.Empty,
                    UserId = userId.ToString()
                };

                var updated = await _repository.UpdateAsync(toDoList);
                if (updated == null)
                {
                    _logger.LogWarning("Failed to update ToDoList {ToDoListId} for user {UserId}", request.Id, userId);
                    return null;
                }
                
                var response = MapToResponse(updated);
                _logger.LogInformation("Successfully updated ToDoList {ToDoListId} for user {UserId}", request.Id, userId);
                return response;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDoList {ToDoListId} for user {UserId}", request.Id, userId);
                throw;
            }
        }

        public async Task<bool> DeleteToDoListAsync(int id, Guid userId)
        {
            _logger.LogInformation("Deleting ToDoList {ToDoListId} for user {UserId}", id, userId);
            try
            {
                var deleted = await _repository.DeleteAsync(id, userId.ToString());
                if (!deleted)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for deletion for user {UserId}", id, userId);
                    return false;
                }
                
                _logger.LogInformation("Successfully deleted ToDoList {ToDoListId} for user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }

        private static ToDoListResponse MapToResponse(ToDoList toDoList)
        {
            return new ToDoListResponse
            {
                Id = toDoList.Id,
                Title = toDoList.Title,
                Description = toDoList.Description,
                UserId = toDoList.UserId,
                TaskCount = toDoList.ToDoItems.Count,
                Tasks = toDoList.ToDoItems.Select(t => new ToDoItemSummary
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = (int)t.Status,
                    Priority = (int)t.Priority
                }).ToList()
            };
        }
    }
}
