using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Shared;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all users with filtering and pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterDto filter)
        {
            var result = await _userService.GetUsersAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get users available for assignment
        /// </summary>
        [HttpGet("available-for-assignment")]
        public async Task<IActionResult> GetUsersForAssignment()
        {
            var result = await _userService.GetUsersForAssignmentAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get active users
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await _userService.GetActiveUsersAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Search users
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return BadRequest(Response<IEnumerable<UserDto>>.FailureResponse(
                    "Search term must be at least 2 characters", 400));
            }

            var result = await _userService.SearchUsersAsync(searchTerm);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get user performance
        /// </summary>
        [HttpGet("{id}/performance")]
        public async Task<IActionResult> GetUserPerformance(Guid id)
        {
            var requestingUserId = GetUserId();
            var result = await _userService.GetUserPerformanceAsync(id, requestingUserId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateDto)
        {
            var result = await _userService.UpdateUserAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Toggle user status
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get user statistics
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var result = await _userService.GetUserStatisticsAsync();
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