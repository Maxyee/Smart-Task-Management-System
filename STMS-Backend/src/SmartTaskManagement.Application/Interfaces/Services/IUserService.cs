using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.DTOs.Shared;


namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get all users with filtering and pagination
        /// </summary>
        Task<Response<PagedResponse<UserDto>>> GetUsersAsync(UserFilterDto filter);

        /// <summary>
        /// Get users available for task assignment (active users with roles)
        /// </summary>
        Task<Response<IEnumerable<UserDto>>> GetUsersForAssignmentAsync();

        /// <summary>
        /// Get all active users
        /// </summary>
        Task<Response<IEnumerable<UserDto>>> GetActiveUsersAsync();

        /// <summary>
        /// Search users by name, username, or email
        /// </summary>
        Task<Response<IEnumerable<UserDto>>> SearchUsersAsync(string searchTerm);

        /// <summary>
        /// Get user by ID
        /// </summary>
        Task<Response<UserDto>> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Get user performance metrics
        /// </summary>
        Task<Response<UserPerformanceDto>> GetUserPerformanceAsync(Guid userId, Guid requestingUserId);

        /// <summary>
        /// Update user
        /// </summary>
        Task<Response<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto updateDto);

        /// <summary>
        /// Toggle user active status
        /// </summary>
        Task<Response<UserDto>> ToggleUserStatusAsync(Guid userId);

        /// <summary>
        /// Get user statistics
        /// </summary>
        Task<Response<UserStatisticsDto>> GetUserStatisticsAsync();
    }
}