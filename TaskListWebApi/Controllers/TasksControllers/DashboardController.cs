using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Services.ToDoListServices;
using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;
using TaskList.Api.Domain.Tasks.Models;
using TaskList.Api.Domian;

namespace TaskListWebApi.Controllers.TasksControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseAuthController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(ILogger<BaseAuthController> logger, IDashboardService dashboardService) : base(logger)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<DashboardResponse>>> GetUsersDashboard()
        {
            _logger.LogInformation("User {UserId} requesting all ToDoItems", UserId);
            ResponseDto<DashboardResponse> response = new();

            try
            {
                DashboardResponse? dashboardData = await _dashboardService.GetAllToDoListsForDashboardAsync(UserId);
                if (dashboardData == null)
                {
                    _logger.LogWarning("Dashboard data not found for user {UserId}", UserId);
                    UpdateResponse(response, "Dashboard data not found");
                    return NotFound(response);
                }

                UpdateResponse(response, "Dashboard data retrieved successfully", true, dashboardData);
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data for user {UserId}", UserId);
                UpdateResponse(response, "Failed to retrieve dashboard data");
                return StatusCode(500, response);
            }
        }
    }
}           
