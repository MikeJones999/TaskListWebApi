using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Application.Services.ToDoItemServices
{
    public class ToDoItemService : IToDoItemService
    {
        private readonly ILogger<ToDoItemService> _logger;
        private readonly IToDoItemRepository _repository;
        private readonly IMapper _mapper;

        public ToDoItemService(ILogger<ToDoItemService> logger, IToDoItemRepository repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ToDoItemResponse>> GetAllToDoItemsAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving all ToDoItems for user {UserId}", userId);
            try
            {
                IEnumerable<ToDoItem> toDoItems = await _repository.GetAllByUserIdAsync(userId.ToString());
                IEnumerable<ToDoItemResponse> response = _mapper.Map<IEnumerable<ToDoItemResponse>>(toDoItems);

                _logger.LogInformation("Successfully retrieved {Count} ToDoItems for user {UserId}", response.Count(), userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoItems for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ToDoItemResponse>> GetAllToDoItemsByListIdAsync(int toDoListId, Guid userId)
        {
            _logger.LogInformation("Retrieving all ToDoItems for ToDoList {ToDoListId} and user {UserId}", toDoListId, userId);
            try
            {
                IEnumerable<ToDoItem> toDoItems = await _repository.GetAllByToDoListIdAsync(toDoListId, userId.ToString());
                IEnumerable<ToDoItemResponse> response = _mapper.Map<IEnumerable<ToDoItemResponse>>(toDoItems);

                _logger.LogInformation("Successfully retrieved {Count} ToDoItems for ToDoList {ToDoListId}", response.Count(), toDoListId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoItems for ToDoList {ToDoListId}", toDoListId);
                throw;
            }
        }

        public async Task<ToDoItemResponse?> GetToDoItemByIdAsync(int id, Guid userId)
        {
            _logger.LogInformation("Retrieving ToDoItem {ToDoItemId} for user {UserId}", id, userId);
            try
            {
                ToDoItem? toDoItem = await _repository.GetByIdAsync(id, userId.ToString());
                if (toDoItem == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for user {UserId}", id, userId);
                    return null;
                }

                ToDoItemResponse response = _mapper.Map<ToDoItemResponse>(toDoItem);
                _logger.LogInformation("Successfully retrieved ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ToDoItemResponse> CreateToDoItemAsync(CreateToDoItemRequest request, Guid userId)
        {
            _logger.LogInformation("Creating ToDoItem with title '{Title}' for user {UserId}", request.Title, userId);     

            try
            {
                if (!await _repository.ToDoListBelongsToUserAsync(request.ToDoListId, userId.ToString()))
                {
                    _logger.LogWarning("ToDoList {ToDoListId} does not belong to user {UserId}", request.ToDoListId, userId);
                    throw new UnauthorizedAccessException($"ToDoList {request.ToDoListId} does not belong to user");
                }

                ToDoItem toDoItem = _mapper.Map<ToDoItem>(request);
                ToDoItem created = await _repository.CreateToDoItemAsync(toDoItem);
                ToDoItemResponse response = _mapper.Map<ToDoItemResponse>(created);

                _logger.LogInformation("Successfully created ToDoItem {ToDoItemId} for user {UserId}", created.Id, userId);
                return response;
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDoItem for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ToDoItemResponse?> UpdateToDoItemAsync(UpdateToDoItemRequest request, Guid userId)
        {
            _logger.LogInformation("Updating ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);

            try
            {
                if (!await _repository.ToDoListBelongsToUserAsync(request.ToDoListId, userId.ToString()))
                {
                    _logger.LogWarning("ToDoList {ToDoListId} does not belong to user {UserId}", request.ToDoListId, userId);
                    throw new UnauthorizedAccessException($"ToDoList {request.ToDoListId} does not belong to user");
                }

                ToDoItem toDoItem = _mapper.Map<ToDoItem>(request);
                toDoItem.ToDoList = new ToDoList { UserId = userId.ToString() };

                ToDoItem? updated = await _repository.UpdateAsync(toDoItem);
                if (updated == null)
                {
                    _logger.LogWarning("Failed to update ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                    return null;
                }

                ToDoItemResponse response = _mapper.Map<ToDoItemResponse>(updated);
                _logger.LogInformation("Successfully updated ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                return response;
            }     
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                throw;
            }
        }

        public async Task<ToDoItemResponse?> UpdateToDoItemStatusAsync(UpdateToDoItemStatusRequest request, Guid userId)
        {
            _logger.LogInformation("Updating status of ToDoItem {ToDoItemId} to {Status} for user {UserId}", request.Id, request.Status, userId);

            try
            {
                ToDoItem? updated = await _repository.UpdateStatusAsync(request.Id, request.Status, userId.ToString());
                if (updated == null)
                {
                    _logger.LogWarning("Failed to update status of ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                    return null;
                }

                ToDoItemResponse response = _mapper.Map<ToDoItemResponse>(updated);
                _logger.LogInformation("Successfully updated status of ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status of ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                throw;
            }
        }

        public async Task<ToDoItemResponse?> UpdateToDoItemPriorityAsync(UpdateToDoItemPriorityRequest request, Guid userId)
        {
            _logger.LogInformation("Updating priority of ToDoItem {ToDoItemId} to {Priority} for user {UserId}", request.Id, request.Priority, userId);

            try
            {
                ToDoItem? updated = await _repository.UpdatePriorityAsync(request.Id, request.Priority, userId.ToString());
                if (updated == null)
                {
                    _logger.LogWarning("Failed to update priority of ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                    return null;
                }

                ToDoItemResponse response = _mapper.Map<ToDoItemResponse>(updated);
                _logger.LogInformation("Successfully updated priority of ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating priority of ToDoItem {ToDoItemId} for user {UserId}", request.Id, userId);
                throw;
            }
        }

        public async Task<bool> DeleteToDoItemAsync(int id, Guid userId)
        {
            _logger.LogInformation("Deleting ToDoItem {ToDoItemId} for user {UserId}", id, userId);
            try
            {
                bool deleted = await _repository.DeleteAsync(id, userId.ToString());
                if (!deleted)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for deletion for user {UserId}", id, userId);
                    return false;
                }

                _logger.LogInformation("Successfully deleted ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDoItem {ToDoItemId} for user {UserId}", id, userId);
                throw;
            }
        }
    }
}