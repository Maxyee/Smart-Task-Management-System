using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.AI;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly ILogger<AiController> _logger;

        public AiController(IAiService aiService, ILogger<AiController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Improve a single task description using AI
        /// </summary>
        [HttpPost("improve-task")]
        public async Task<IActionResult> ImproveTaskDescription([FromBody] TaskImprovementRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.ImproveTaskDescriptionAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Bulk improve task descriptions
        /// </summary>
        [HttpPost("bulk-improve")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> BulkImproveTasks([FromBody] TaskBulkImprovementDto bulkRequest)
        {
            var userId = GetUserId();
            var result = await _aiService.BulkImproveTaskDescriptionsAsync(bulkRequest, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Generate a summary for a task description
        /// </summary>
        [HttpPost("summarize")]
        public async Task<IActionResult> GenerateSummary([FromBody] SummarizeRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.GenerateTaskSummaryAsync(request.Description, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Suggest next actions for a task
        /// </summary>
        [HttpPost("suggest-actions")]
        public async Task<IActionResult> SuggestActions([FromBody] SuggestActionsRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.SuggestNextActionsAsync(
                request.Title, request.Description, request.Status, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Check AI service health
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckHealth()
        {
            var result = await _aiService.CheckHealthAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Preview improvement without saving
        /// </summary>
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewImprovement([FromBody] TaskImprovementRequestDto request)
        {
            var userId = GetUserId();
            var result = await _aiService.ImproveTaskDescriptionAsync(request, userId);
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

    // Additional DTOs for specific endpoints
    public class SummarizeRequestDto
    {
        public string Description { get; set; } = string.Empty;
    }

    public class SuggestActionsRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "To Do";
    }
}