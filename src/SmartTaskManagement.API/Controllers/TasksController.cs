using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.DTOs.Tasks;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createDto)
        {
            var userId = GetUserId();
            var result = await _taskService.CreateTaskAsync(createDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateDto)
        {
            var userId = GetUserId();
            var result = await _taskService.UpdateTaskAsync(id, updateDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a task (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserId();
            var result = await _taskService.DeleteTaskAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTaskByIdAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get all tasks with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTasksAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusDto statusDto)
        {
            var userId = GetUserId();
            var result = await _taskService.UpdateTaskStatusAsync(id, statusDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Assign task to a user
        /// </summary>
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignTaskDto assignDto)
        {
            var userId = GetUserId();
            var result = await _taskService.AssignTaskAsync(id, assignDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get task statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetTaskStatistics([FromQuery] Guid? projectId = null, [FromQuery] Guid? userId = null)
        {
            var result = await _taskService.GetTaskStatisticsAsync(projectId, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            var userId = GetUserId();
            var result = await _taskService.GetOverdueTasksAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get tasks due soon
        /// </summary>
        [HttpGet("due-soon")]
        public async Task<IActionResult> GetTasksDueSoon([FromQuery] int daysThreshold = 7)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTasksDueSoonAsync(userId, daysThreshold);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Bulk update task statuses
        /// </summary>
        [HttpPatch("bulk-status")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusDto bulkDto)
        {
            var userId = GetUserId();
            var result = await _taskService.BulkUpdateStatusAsync(bulkDto.TaskIds, bulkDto.NewStatus, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get task activity log
        /// </summary>
        [HttpGet("{id}/activity")]
        public async Task<IActionResult> GetTaskActivity(Guid id)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTaskActivityLogAsync(id, userId);
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

    // DTO for bulk status update
    public class BulkUpdateStatusDto
    {
        public List<Guid> TaskIds { get; set; } = new();
        public TaskItemStatus NewStatus { get; set; }
    }
}