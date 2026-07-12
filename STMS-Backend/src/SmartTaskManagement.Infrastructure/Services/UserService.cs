using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.DTOs.Shared;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<PagedResponse<UserDto>>> GetUsersAsync(UserFilterDto filter)
        {
            try
            {
                // Get all users
                var allUsers = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted);
                var query = allUsers.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(u =>
                        u.Username.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                    query = query.Where(u => u.Role.ToString() == filter.Role);
                }

                if (filter.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == filter.IsActive.Value);
                }

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDescending);

                // Get total count
                var totalCount = query.Count();

                // Apply pagination
                var pagedUsers = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                // Map to DTOs
                var userDtos = pagedUsers.Select(MapToUserDto);

                var response = new PagedResponse<UserDto>
                {
                    Items = userDtos,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };

                return Response<PagedResponse<UserDto>>.SuccessResponse(response, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return Response<PagedResponse<UserDto>>.FailureResponse("An error occurred while retrieving users", 500);
            }
        }

        public async Task<Response<IEnumerable<UserDto>>> GetUsersForAssignmentAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u =>
                    !u.IsDeleted &&
                    u.IsActive &&
                    (u.Role == UserRole.TeamMember || u.Role == UserRole.ProjectManager));

                var userDtos = users.Select(MapToUserDto);
                return Response<IEnumerable<UserDto>>.SuccessResponse(userDtos, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for assignment");
                return Response<IEnumerable<UserDto>>.FailureResponse("An error occurred while retrieving users", 500);
            }
        }

        public async Task<Response<IEnumerable<UserDto>>> GetActiveUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted && u.IsActive);
                var userDtos = users.Select(MapToUserDto);
                return Response<IEnumerable<UserDto>>.SuccessResponse(userDtos, "Active users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active users");
                return Response<IEnumerable<UserDto>>.FailureResponse("An error occurred while retrieving active users", 500);
            }
        }

        public async Task<Response<IEnumerable<UserDto>>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                {
                    return Response<IEnumerable<UserDto>>.FailureResponse("Search term must be at least 2 characters", 400);
                }

                var searchTermLower = searchTerm.ToLower();
                var users = await _unitOfWork.Users.FindAsync(u =>
                    !u.IsDeleted &&
                    u.IsActive &&
                    (u.Username.ToLower().Contains(searchTermLower) ||
                     u.Email.ToLower().Contains(searchTermLower) ||
                     u.FirstName.ToLower().Contains(searchTermLower) ||
                     u.LastName.ToLower().Contains(searchTermLower)));

                var userDtos = users.Select(MapToUserDto);
                return Response<IEnumerable<UserDto>>.SuccessResponse(userDtos, "Users found successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                return Response<IEnumerable<UserDto>>.FailureResponse("An error occurred while searching users", 500);
            }
        }

        public async Task<Response<UserDto>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return Response<UserDto>.FailureResponse("User not found", 404);
                }

                var userDto = MapToUserDto(user);
                return Response<UserDto>.SuccessResponse(userDto, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return Response<UserDto>.FailureResponse("An error occurred while retrieving the user", 500);
            }
        }

        public async Task<Response<UserPerformanceDto>> GetUserPerformanceAsync(Guid userId, Guid requestingUserId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return Response<UserPerformanceDto>.FailureResponse("User not found", 404);
                }

                var requestingUser = await _unitOfWork.Users.GetByIdAsync(requestingUserId);
                if (requestingUser == null)
                {
                    return Response<UserPerformanceDto>.FailureResponse("Requesting user not found", 404);
                }

                // Check permissions
                if (requestingUser.Role != UserRole.Admin &&
                    requestingUser.Role != UserRole.ProjectManager &&
                    requestingUserId != userId)
                {
                    return Response<UserPerformanceDto>.FailureResponse(
                        "You don't have permission to view this user's performance", 403);
                }

                // Get all tasks assigned to this user
                var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                    !t.IsDeleted &&
                    t.AssignedToUserId == userId);

                var taskList = tasks.ToList();

                // Calculate metrics
                var assignedTasks = taskList.Count;
                var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
                var overdueTasks = taskList.Count(t =>
                    t.DueDate < DateTime.UtcNow &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);
                var inProgressTasks = taskList.Count(t => t.Status == TaskItemStatus.InProgress);
                var onTimeCompleted = taskList.Count(t =>
                    t.Status == TaskItemStatus.Completed &&
                    t.UpdatedAt <= t.DueDate);

                var completionRate = assignedTasks > 0
                    ? Math.Round((double)completedTasks / assignedTasks * 100, 2)
                    : 0;

                // Average completion time
                var completedTaskList = taskList.Where(t => t.Status == TaskItemStatus.Completed).ToList();
                var avgCompletionTime = 0.0;
                if (completedTaskList.Any())
                {
                    var totalDays = completedTaskList.Sum(t => (t.UpdatedAt - t.CreatedAt).TotalDays);
                    avgCompletionTime = Math.Round(totalDays / completedTaskList.Count, 2);
                }

                // On-time delivery rate
                var onTimeDeliveryRate = completedTasks > 0
                    ? Math.Round((double)onTimeCompleted / completedTasks * 100, 2)
                    : 0;

                // Tasks by priority
                var tasksByPriority = taskList
                    .GroupBy(t => t.Priority)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

                // Tasks by status
                var tasksByStatus = taskList
                    .GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

                // Productivity Score (weighted calculation)
                var productivityScore = (completionRate * 0.35) +
                                        (Math.Min(assignedTasks / 10.0, 1.0) * 25) +
                                        (onTimeDeliveryRate * 0.25) +
                                        (Math.Max(0, 100 - (avgCompletionTime * 3)) * 0.15);
                productivityScore = Math.Min(100, productivityScore);

                var performanceDto = new UserPerformanceDto
                {
                    UserId = user.Id,
                    UserName = user.Username,
                    FullName = $"{user.FirstName} {user.LastName}",
                    AssignedTasks = assignedTasks,
                    CompletedTasks = completedTasks,
                    OverdueTasks = overdueTasks,
                    CompletionRate = completionRate,
                    AverageCompletionTime = avgCompletionTime,
                    ProductivityScore = Math.Round(productivityScore, 2),
                    TasksCreated = await _unitOfWork.Tasks.CountAsync(t => !t.IsDeleted && t.CreatedByUserId == userId),
                    TasksInProgress = inProgressTasks,
                    OnTimeDeliveryRate = onTimeDeliveryRate,
                    TasksByPriority = tasksByPriority,
                    TasksByStatus = tasksByStatus
                };

                return Response<UserPerformanceDto>.SuccessResponse(performanceDto, "User performance retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user performance for user {UserId}", userId);
                return Response<UserPerformanceDto>.FailureResponse("An error occurred while retrieving user performance", 500);
            }
        }

        public async Task<Response<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return Response<UserDto>.FailureResponse("User not found", 404);
                }

                // Update fields
                if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                    user.FirstName = updateDto.FirstName;

                if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                    user.LastName = updateDto.LastName;

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                {
                    // Check email uniqueness
                    if (!await _unitOfWork.Users.IsEmailUniqueAsync(updateDto.Email, userId))
                    {
                        return Response<UserDto>.FailureResponse("Email is already taken", 409);
                    }
                    user.Email = updateDto.Email;
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Role))
                {
                    if (Enum.TryParse<UserRole>(updateDto.Role, true, out var role))
                    {
                        user.Role = role;
                    }
                    else
                    {
                        return Response<UserDto>.FailureResponse("Invalid role", 400);
                    }
                }

                if (updateDto.IsActive.HasValue)
                    user.IsActive = updateDto.IsActive.Value;

                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var userDto = MapToUserDto(user);
                return Response<UserDto>.SuccessResponse(userDto, "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return Response<UserDto>.FailureResponse("An error occurred while updating the user", 500);
            }
        }

        public async Task<Response<UserDto>> ToggleUserStatusAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return Response<UserDto>.FailureResponse("User not found", 404);
                }

                // Prevent deactivating the last admin
                if (user.Role == UserRole.Admin && user.IsActive)
                {
                    var adminCount = await _unitOfWork.Users.CountAsync(u =>
                        !u.IsDeleted && u.Role == UserRole.Admin && u.IsActive);
                    if (adminCount <= 1)
                    {
                        return Response<UserDto>.FailureResponse(
                            "Cannot deactivate the last admin user", 400);
                    }
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var userDto = MapToUserDto(user);
                var status = user.IsActive ? "activated" : "deactivated";
                return Response<UserDto>.SuccessResponse(userDto, $"User {status} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status for user {UserId}", userId);
                return Response<UserDto>.FailureResponse("An error occurred while toggling user status", 500);
            }
        }

        public async Task<Response<UserStatisticsDto>> GetUserStatisticsAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted);
                var userList = users.ToList();

                var statistics = new UserStatisticsDto
                {
                    TotalUsers = userList.Count,
                    ActiveUsers = userList.Count(u => u.IsActive),
                    InactiveUsers = userList.Count(u => !u.IsActive),
                    UsersByRole = userList
                        .GroupBy(u => u.Role.ToString())
                        .ToDictionary(g => g.Key, g => g.Count()),
                    UsersByActivity = CalculateUsersByActivity(userList),
                    TopPerformers = await CalculateTopPerformers(userList)
                };

                return Response<UserStatisticsDto>.SuccessResponse(statistics, "User statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user statistics");
                return Response<UserStatisticsDto>.FailureResponse("An error occurred while retrieving user statistics", 500);
            }
        }

        #region Private Methods

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                //IsActive = user.IsActive,
                //CreatedAt = user.CreatedAt,
                //UpdatedAt = user.UpdatedAt
            };
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, bool sortDescending)
        {
            return sortBy.ToLower() switch
            {
                "username" => sortDescending
                    ? query.OrderByDescending(u => u.Username)
                    : query.OrderBy(u => u.Username),
                "email" => sortDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "firstname" => sortDescending
                    ? query.OrderByDescending(u => u.FirstName)
                    : query.OrderBy(u => u.FirstName),
                "lastname" => sortDescending
                    ? query.OrderByDescending(u => u.LastName)
                    : query.OrderBy(u => u.LastName),
                "role" => sortDescending
                    ? query.OrderByDescending(u => u.Role)
                    : query.OrderBy(u => u.Role),
                "isactive" => sortDescending
                    ? query.OrderByDescending(u => u.IsActive)
                    : query.OrderBy(u => u.IsActive),
                _ => sortDescending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt)
            };
        }

        private Dictionary<string, int> CalculateUsersByActivity(List<User> users)
        {
            var result = new Dictionary<string, int>();

            // Get all tasks
            var tasks = _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted).Result;
            var taskList = tasks.ToList();

            foreach (var user in users)
            {
                var assignedTasks = taskList.Count(t => t.AssignedToUserId == user.Id);
                var activityLevel = assignedTasks switch
                {
                    >= 15 => "Very Active",
                    >= 8 => "Active",
                    >= 3 => "Moderate",
                    >= 1 => "Occasional",
                    _ => "Inactive"
                };

                if (!result.ContainsKey(activityLevel))
                    result[activityLevel] = 0;
                result[activityLevel]++;
            }

            return result;
        }

        private async Task<List<UserPerformanceDto>> CalculateTopPerformers(List<User> users)
        {
            var topPerformers = new List<UserPerformanceDto>();

            foreach (var user in users)
            {
                var performance = await GetUserPerformanceAsync(user.Id, user.Id);
                if (performance.Success && performance.Data != null)
                {
                    topPerformers.Add(performance.Data);
                }
            }

            // Order by productivity score and take top 5
            return topPerformers
                .OrderByDescending(p => p.ProductivityScore)
                .Take(5)
                .ToList();
        }

        #endregion
    }
}