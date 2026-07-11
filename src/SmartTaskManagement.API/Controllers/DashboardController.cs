using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Dashboard;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete dashboard summary
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DashboardFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetDashboardSummaryAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get progress for a specific project
        /// </summary>
        [HttpGet("projects/{projectId}/progress")]
        public async Task<IActionResult> GetProjectProgress(Guid projectId)
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetProjectProgressAsync(projectId, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get progress for all accessible projects
        /// </summary>
        [HttpGet("projects/progress")]
        public async Task<IActionResult> GetAllProjectsProgress()
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetAllProjectsProgressAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get user performance
        /// </summary>
        [HttpGet("users/{userId}/performance")]
        public async Task<IActionResult> GetUserPerformance(Guid userId)
        {
            var requestingUserId = GetUserId();
            var result = await _dashboardService.GetUserPerformanceAsync(userId, requestingUserId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get team performance
        /// </summary>
        [HttpGet("team/performance")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> GetTeamPerformance()
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetTeamPerformanceAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get performance metrics
        /// </summary>
        [HttpGet("metrics")]
        public async Task<IActionResult> GetPerformanceMetrics([FromQuery] DashboardFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetPerformanceMetricsAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get recent activities
        /// </summary>
        [HttpGet("activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 10, [FromQuery] Guid? userId = null)
        {
            var result = await _dashboardService.GetRecentActivitiesAsync(count, userId);
            return StatusCode(result.StatusCode, result);
        }

        #region Private Methods

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        #endregion
    }
}