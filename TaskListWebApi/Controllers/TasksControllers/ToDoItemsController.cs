using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;
using TaskList.Api.Domain.Tasks.Enums;
using TaskList.Api.Domian;
using TaskList.Api.Utility.StaticHelpers;

namespace TaskListWebApi.Controllers.TasksControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : BaseAuthController
    {
        private readonly IToDoItemService _toDoItemService;

        public ToDoItemsController(ILogger<BaseAuthController> logger, IToDoItemService toDoItemService) : base(logger)
        {
            _toDoItemService = toDoItemService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<ToDoItemResponse>>>> GetAllToDoItems()
        {
            ResponseDto<IEnumerable<ToDoItemResponse>> response = new();

            try
            {
                _logger.LogInformation("User {UserId} requesting all ToDoItems", UserId);
                IEnumerable<ToDoItemResponse> toDoItems = await _toDoItemService.GetAllToDoItemsAsync(UserId);
                UpdateResponse(response, "ToDoItems retrieved successfully", true, toDoItems);
                _logger.LogInformation("Successfully returned all ToDoItems for user {UserId}", UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoItems for user {UserId}", UserId);
                UpdateResponse(response, "Failed to retrieve ToDoItems");
                return StatusCode(500, response);
            }
        }

        [HttpGet("list/{toDoListId}")]
        public async Task<ActionResult<ResponseDto<IEnumerable<ToDoItemResponse>>>> GetToDoItemsByListId(int toDoListId)
        {
            ResponseDto<IEnumerable<ToDoItemResponse>> response = new();

            if (toDoListId <= 0)
            {
                _logger.LogWarning("Invalid ToDoList ID {ToDoListId} provided by user {UserId}", toDoListId, UserId);
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} requesting ToDoItems for ToDoList {ToDoListId}", UserId, toDoListId);
                IEnumerable<ToDoItemResponse> toDoItems = await _toDoItemService.GetAllToDoItemsByListIdAsync(toDoListId, UserId);
                UpdateResponse(response, "ToDoItems retrieved successfully", true, toDoItems);
                _logger.LogInformation("Successfully returned ToDoItems for ToDoList {ToDoListId}", toDoListId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoItems for ToDoList {ToDoListId}", toDoListId);
                UpdateResponse(response, "Failed to retrieve ToDoItems");
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<ToDoItemResponse>>> GetToDoItemById(int id)
        {
            ResponseDto<ToDoItemResponse> response = new();

            if (id <= 0)
            {
                _logger.LogWarning("Invalid ToDoItem ID {ToDoItemId} provided by user {UserId}", id, UserId);
                UpdateResponse(response, "Invalid ToDoItem ID");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} requesting ToDoItem {ToDoItemId}", UserId, id);
                ToDoItemResponse? toDoItem = await _toDoItemService.GetToDoItemByIdAsync(id, UserId);
                if (toDoItem == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for user {UserId}", id, UserId);
                    UpdateResponse(response, "ToDoItem not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoItem retrieved successfully", true, toDoItem);
                _logger.LogInformation("Successfully returned ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to retrieve ToDoItem");
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<ToDoItemResponse>>> CreateToDoItem([FromBody] CreateToDoItemRequest request)
        {
            ResponseDto<ToDoItemResponse> response = new();        

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("Empty title provided for creating ToDoItem by user {UserId}", UserId);
                UpdateResponse(response, "Title is required");
                return BadRequest(response);
            }

            if (request.ToDoListId <= 0)
            {
                _logger.LogWarning("Invalid ToDoList ID provided for creating ToDoItem by user {UserId}", UserId);
                UpdateResponse(response, "Valid ToDoList ID is required");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} creating ToDoItem with title '{Title}'", UserId, request.Title);
                ToDoItemResponse toDoItem = await _toDoItemService.CreateToDoItemAsync(request, UserId);
                UpdateResponse(response, "ToDoItem created successfully", true, toDoItem);
                _logger.LogInformation("Controller: Successfully created ToDoItem {ToDoItemId} for user {UserId}", toDoItem.Id, UserId);
                return CreatedAtAction(nameof(GetToDoItemById), new { id = toDoItem.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating ToDoItem for user {UserId}", UserId);
                UpdateResponse(response, ex.Message);
                return BadRequest(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access creating ToDoItem for user {UserId}", UserId);
                UpdateResponse(response, ex.Message);
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDoItem for user {UserId}", UserId);
                UpdateResponse(response, "Failed to create ToDoItem");
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<ToDoItemResponse>>> UpdateToDoItem(int id, [FromBody] UpdateToDoItemRequest request)
        {
            ResponseDto<ToDoItemResponse> response = new();

            if (id <= 0 || id != request.Id)
            {
                _logger.LogWarning("ID mismatch or invalid ID. Route ID: {RouteId}, Request ID: {RequestId} by user {UserId}", id, request.Id, UserId);
                UpdateResponse(response, "Invalid ToDoItem ID");
                return BadRequest(response);
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("Empty title provided for updating ToDoItem {ToDoItemId} by user {UserId}", id, UserId);
                UpdateResponse(response, "Title is required");
                return BadRequest(response);
            }

            if (request.ToDoListId <= 0)
            {
                _logger.LogWarning("Invalid ToDoList ID provided for updating ToDoItem {ToDoItemId} by user {UserId}", id, UserId);
                UpdateResponse(response, "Valid ToDoList ID is required");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} updating ToDoItem {ToDoItemId}", UserId, id);
                ToDoItemResponse? toDoItem = await _toDoItemService.UpdateToDoItemAsync(request, UserId);
                if (toDoItem == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for update by user {UserId}", id, UserId);
                    UpdateResponse(response, "ToDoItem not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoItem updated successfully", true, toDoItem);
                _logger.LogInformation("Successfully updated ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access updating ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                UpdateResponse(response, ex.Message);
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to update ToDoItem");
                return StatusCode(500, response);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ResponseDto<ToDoItemResponse>>> UpdateToDoItemStatus(int id, [FromBody] UpdateToDoItemStatusRequest request)
        {
            ResponseDto<ToDoItemResponse> response = new();

            if (id <= 0 || id != request.Id || request.Status > EnumHelper.GetMaxValue<ProgressStatus>())
            {
                _logger.LogWarning("ID mismatch or invalid ID for status update. Route ID: {RouteId}, Request ID: {RequestId} by user {UserId}", id, request.Id, UserId);
                UpdateResponse(response, "Invalid ToDoItem ID");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} updating status of ToDoItem {ToDoItemId} to {Status}", UserId, id, request.Status);
                ToDoItemResponse? toDoItem = await _toDoItemService.UpdateToDoItemStatusAsync(request, UserId);
                if (toDoItem == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for status update by user {UserId}", id, UserId);
                    UpdateResponse(response, "ToDoItem not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoItem status updated successfully", true, toDoItem);
                _logger.LogInformation("Successfully updated status of ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status of ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to update ToDoItem status");
                return StatusCode(500, response);
            }
        }

        [HttpPatch("{id}/priority")]
        public async Task<ActionResult<ResponseDto<ToDoItemResponse>>> UpdateToDoItemPriority(int id, [FromBody] UpdateToDoItemPriorityRequest request)
        {
            ResponseDto<ToDoItemResponse> response = new();

            if (id <= 0 || id != request.Id || request.Priority > EnumHelper.GetMaxValue<PriorityStatus>())
            {
                _logger.LogWarning("ID mismatch or invalid ID for priority update. Route ID: {RouteId}, Request ID: {RequestId} by user {UserId}", id, request.Id, UserId);
                UpdateResponse(response, "Invalid ToDoItem ID");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} updating priority of ToDoItem {ToDoItemId} to {Priority}", UserId, id, request.Priority);
                ToDoItemResponse? toDoItem = await _toDoItemService.UpdateToDoItemPriorityAsync(request, UserId);
                if (toDoItem == null)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for priority update by user {UserId}", id, UserId);
                    UpdateResponse(response, "ToDoItem not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoItem priority updated successfully", true, toDoItem);
                _logger.LogInformation("Successfully updated priority of ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating priority of ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to update ToDoItem priority");
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteToDoItem(int id)
        {
            ResponseDto<bool> response = new();

            if (id <= 0)
            {
                _logger.LogWarning("Invalid ToDoItem ID {ToDoItemId} for deletion by user {UserId}", id, UserId);
                UpdateResponse(response, "Invalid ToDoItem ID");
                return BadRequest(response);
            }

            try
            {
                _logger.LogInformation("User {UserId} deleting ToDoItem {ToDoItemId}", UserId, id);
                bool deleted = await _toDoItemService.DeleteToDoItemAsync(id, UserId);
                if (!deleted)
                {
                    _logger.LogWarning("ToDoItem {ToDoItemId} not found for deletion by user {UserId}", id, UserId);
                    UpdateResponse(response, "ToDoItem not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoItem deleted successfully", true, true);
                _logger.LogInformation("Successfully deleted ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDoItem {ToDoItemId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to delete ToDoItem");
                return StatusCode(500, response);
            }
        }
    }
}