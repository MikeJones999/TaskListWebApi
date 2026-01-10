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
                IEnumerable<ToDoListResponse> toDoLists = await _toDoListService.GetAllToDoListsAsync(UserId);
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
            ResponseDto<ToDoListResponse> response = GetResponseModel();

            if (id <= 0)
            {
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            try
            {
                ToDoListResponse? toDoList = await _toDoListService.GetToDoListByIdAsync(id, UserId);
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

        [HttpGet("{id}/paginated")]
        public async Task<ActionResult<ResponseDto<PaginatedToDoListResponse>>> GetToDoListByIdWithPagination(
            int id,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "",
            [FromQuery] bool ascending = true)
        {
            ResponseDto<PaginatedToDoListResponse> response = new ResponseDto<PaginatedToDoListResponse>();

            if (id <= 0)
            {
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            if (pageNumber < 1)
            {
                UpdateResponse(response, "Page number must be greater than 0");
                return BadRequest(response);
            }

            if (pageSize < 1 || pageSize > 100) //TODO worth having config to stop more than 100 tasks per task list being created
            {
                UpdateResponse(response, "Page size must be between 1 and 100");
                return BadRequest(response);
            }

            try
            {
                PaginatedToDoListResponse? toDoList = await _toDoListService.GetToDoListByIdWithPaginationAsync(id, UserId, pageNumber, pageSize, sortBy, ascending);
                if (toDoList == null)
                {
                    UpdateResponse(response, "ToDoList not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "ToDoList retrieved successfully", true, toDoList);
                _logger.LogInformation("User {UserId} retrieved ToDoList {ToDoListId} (Page {PageNumber}, Size {PageSize}, Sort {SortBy} {SortDirection})", UserId, id, pageNumber, pageSize, sortBy, ascending ? "ASC" : "DESC");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated ToDoList {ToDoListId} for user {UserId}", id, UserId);
                UpdateResponse(response, "Failed to retrieve ToDoList");
                return StatusCode(500, response);
            }
        }


        [HttpPost]
        public async Task<ActionResult<ResponseDto<ToDoListResponse>>> CreateToDoList([FromBody] CreateToDoListRequest request)
        {
            ResponseDto<ToDoListResponse> response = GetResponseModel();

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                UpdateResponse(response, "Title is required");
                return BadRequest(response);
            }

            try
            {
                ToDoListResponse toDoList = await _toDoListService.CreateToDoListAsync(request, UserId);
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
            ResponseDto<ToDoListResponse> response = GetResponseModel();

            if (id <= 0 || id != request.Id)
            {
                UpdateResponse(response, "Invalid ToDoList ID");
                return BadRequest(response);
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                UpdateResponse(response, "Title is required");
                return BadRequest(response);
            }

            try
            {
                ToDoListResponse? toDoList = await _toDoListService.UpdateToDoListAsync(request, UserId);
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

        private static ResponseDto<ToDoListResponse> GetResponseModel()
        {
            return new ResponseDto<ToDoListResponse>();
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
                if (!await _toDoListService.DeleteToDoListAsync(id, UserId))
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
