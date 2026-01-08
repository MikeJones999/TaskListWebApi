using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs;
using TaskList.Api.Domian;

namespace TaskListWebApi.Controllers.TasksControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoListsController : BaseAuthController
    {
        private readonly IToDoListService _toDoListService;

        public ToDoListsController(ILogger<BaseAuthController> logger, IToDoListService toDoListService) : base(logger)
        {
            _toDoListService = toDoListService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<ToDoListResponse>>>> GetAllToDoLists()
        {
            var response = new ResponseDto<IEnumerable<ToDoListResponse>>();

            try
            {
                var toDoLists = await _toDoListService.GetAllToDoListsAsync(UserId);
                UpdateResponse(response, "ToDoLists retrieved successfully", true, toDoLists);
                _logger.LogInformation("User {UserId} retrieved all ToDoLists", UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoLists for user {UserId}", UserId);
                UpdateResponse(response, "Failed to retrieve ToDoLists");
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<ToDoListResponse>>> GetToDoListById(int id)
        {
            var response = new ResponseDto<ToDoListResponse>();

            if (id <= 0)
            {
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            try
            {
                var toDoList = await _toDoListService.GetToDoListByIdAsync(id, UserId);
                if (toDoList == null)
                {
                    UpdateResponse(response, "ToDoList not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoList retrieved successfully", true, toDoList);
                _logger.LogInformation("User {UserId} retrieved ToDoList {ToDoListId}", UserId, id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ToDoList {ToDoListId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to retrieve ToDoList");
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<ToDoListResponse>>> CreateToDoList([FromBody] CreateToDoListRequest request)
        {
            var response = new ResponseDto<ToDoListResponse>();

            if (!ModelState.IsValid)
            {
                UpdateResponse(response, "Invalid request data");
                return BadRequest(response);
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                UpdateResponse(response, "Title is required");
                return BadRequest(response);
            }

            try
            {
                var toDoList = await _toDoListService.CreateToDoListAsync(request, UserId);
                UpdateResponse(response, "ToDoList created successfully", true, toDoList);
                _logger.LogInformation("User {UserId} created ToDoList {ToDoListId}", UserId, toDoList.Id);
                return CreatedAtAction(nameof(GetToDoListById), new { id = toDoList.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating ToDoList for user {UserId}", UserId);
                UpdateResponse(response, ex.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDoList for user {UserId}", UserId);
                UpdateResponse(response, "Failed to create ToDoList");
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<ToDoListResponse>>> UpdateToDoList(int id, [FromBody] UpdateToDoListRequest request)
        {
            var response = new ResponseDto<ToDoListResponse>();

            if (id <= 0 || id != request.Id)
            {
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            if (!ModelState.IsValid)
            {
                UpdateResponse(response, "Invalid request data");
                return BadRequest(response);
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                UpdateResponse(response, "Title is required");
                return BadRequest(response);
            }

            try
            {
                var toDoList = await _toDoListService.UpdateToDoListAsync(request, UserId);
                if (toDoList == null)
                {
                    UpdateResponse(response, "ToDoList not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoList updated successfully", true, toDoList);
                _logger.LogInformation("User {UserId} updated ToDoList {ToDoListId}", UserId, id);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating ToDoList {ToDoListId} for user {UserId}", id, UserId);
                UpdateResponse(response, ex.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDoList {ToDoListId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to update ToDoList");
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteToDoList(int id)
        {
            var response = new ResponseDto<bool>();

            if (id <= 0)
            {
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            try
            {
                var deleted = await _toDoListService.DeleteToDoListAsync(id, UserId);
                if (!deleted)
                {
                    UpdateResponse(response, "ToDoList not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoList deleted successfully", true, true);
                _logger.LogInformation("User {UserId} deleted ToDoList {ToDoListId}", UserId, id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDoList {ToDoListId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to delete ToDoList");
                return StatusCode(500, response);
            }
        }
    }
}
