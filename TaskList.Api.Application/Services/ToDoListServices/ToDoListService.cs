using AutoMapper;
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
        private readonly IMapper _mapper;

        public ToDoListService(ILogger<ToDoListService> logger, IToDoListRepository repository, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ToDoListResponse>> GetAllToDoListsAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving all ToDoLists for user {UserId}", userId);
            try
            {
                IEnumerable<ToDoList> toDoLists = await _repository.GetAllByUserIdAsync(userId.ToString());
                IEnumerable<ToDoListResponse> response = _mapper.Map<IEnumerable<ToDoListResponse>>(toDoLists);
                
                _logger.LogInformation("Successfully retrieved {Count} ToDoLists for user {UserId}", response.Count(), userId);
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
                ToDoList? toDoList = await _repository.GetByIdAsync(id, userId.ToString());
                if (toDoList == null)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for user {UserId}", id, userId);
                    return null;
                }

                ToDoListResponse response = _mapper.Map<ToDoListResponse>(toDoList);
                _logger.LogInformation("Successfully retrieved ToDoList {ToDoListId} for user {UserId}", id, userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<PaginatedToDoListResponse?> GetToDoListByIdWithPaginationAsync(int id, Guid userId, int pageNumber = 1, int pageSize = 10, string sortBy = "priority", bool ascending = true)
        {
            _logger.LogInformation("Retrieving ToDoList {ToDoListId} for user {UserId} with pagination (Page: {PageNumber}, Size: {PageSize}, Sort: {SortBy} {SortDirection})", 
                id, userId, pageNumber, pageSize, sortBy, ascending ? "ASC" : "DESC");
            
            try
            {
                var (toDoList, totalItems) = await _repository.GetByIdWithPaginationAsync(id, userId.ToString(), pageNumber, pageSize, sortBy, ascending);
                
                if (toDoList == null)
                {
                    _logger.LogWarning("ToDoList {ToDoListId} not found for user {UserId}", id, userId);
                    return null;
                }
          
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                var response = new PaginatedToDoListResponse
                {
                    Id = toDoList.Id,
                    Title = toDoList.Title,
                    Description = toDoList.Description,
                    UserId = toDoList.UserId,
                    TotalItemCount = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages,
                    Tasks = _mapper.Map<List<ToDoItemSummary>>(toDoList.ToDoItems)
                };

                _logger.LogInformation("Successfully retrieved ToDoList {ToDoListId} with {ItemCount} items on page {PageNumber} of {TotalPages}", id, toDoList.ToDoItems.Count, pageNumber, totalPages);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated ToDoList {ToDoListId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ToDoListResponse> CreateToDoListAsync(CreateToDoListRequest request, Guid userId)
        {
            _logger.LogInformation("Creating ToDoList with title '{Title}' for user {UserId}", request.Title, userId);

            try
            {
                ToDoList toDoList = _mapper.Map<ToDoList>(request);
                UpdateUserId(userId, toDoList);

                ToDoList created = await _repository.CreateAsync(toDoList);
                ToDoListResponse response = _mapper.Map<ToDoListResponse>(created);

                _logger.LogInformation("Successfully created ToDoList {ToDoListId} for user {UserId}", created.Id, userId);
                return response;
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

            try
            {
                ToDoList toDoList = _mapper.Map<ToDoList>(request);
                UpdateUserId(userId, toDoList);

                ToDoList? updated = await _repository.UpdateAsync(toDoList);
                if (updated == null)
                {
                    _logger.LogWarning("Failed to update ToDoList {ToDoListId} for user {UserId}", request.Id, userId);
                    return null;
                }

                ToDoListResponse response = _mapper.Map<ToDoListResponse>(updated);
                _logger.LogInformation("Successfully updated ToDoList {ToDoListId} for user {UserId}", request.Id, userId);
                return response;
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
                if (!await _repository.DeleteListAnddItemsAsync(id, userId.ToString()))
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

        private static void UpdateUserId(Guid userId, ToDoList toDoList)
        {
            toDoList.UserId = userId.ToString();
        }
    }
}
