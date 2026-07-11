using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;


namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createDto)
        {
            var userId = GetUserId();
            var result = await _projectService.CreateProjectAsync(createDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto updateDto)
        {
            var userId = GetUserId();
            var result = await _projectService.UpdateProjectAsync(id, updateDto, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a project (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.DeleteProjectAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get project by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectByIdAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get all projects with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] ProjectFilterDto filter)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectsAsync(filter, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get project task summary
        /// </summary>
        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetProjectSummary(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectTaskSummaryAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Toggle project active status
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleProjectStatus(Guid id)
        {
            var userId = GetUserId();
            var result = await _projectService.ToggleProjectStatusAsync(id, userId);
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