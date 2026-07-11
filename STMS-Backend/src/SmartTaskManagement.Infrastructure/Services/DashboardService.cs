using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Dashboard;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<DashboardSummaryDto>> GetDashboardSummaryAsync(DashboardFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<DashboardSummaryDto>.FailureResponse("User not found", 404);
                }

                // Get data based on user role
                var isAdmin = user.Role == UserRole.Admin;
                var isManager = user.Role == UserRole.ProjectManager;

                // Get projects based on permissions
                var projects = await GetAccessibleProjects(user);

                // Apply date filter
                if (filter.StartDate.HasValue)
                {
                    projects = projects.Where(p => p.CreatedAt >= filter.StartDate.Value);
                }
                if (filter.EndDate.HasValue)
                {
                    projects = projects.Where(p => p.CreatedAt <= filter.EndDate.Value);
                }

                // Get all tasks
                var tasks = new List<TaskItem>();
                foreach (var project in projects)
                {
                    var projectTasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.Id);
                    tasks.AddRange(projectTasks.Where(t => !t.IsDeleted));
                }

                // Build dashboard summary
                var dashboard = new DashboardSummaryDto
                {
                    ProjectStatistics = await GetProjectStatisticsAsync(projects, tasks),
                    TaskStatistics = await GetTaskStatisticsAsync(tasks, projects),
                    UserStatistics = await GetUserStatisticsAsync(user),
                    RecentActivity = await GetActivitySummaryAsync(tasks, user),
                    PerformanceMetrics = await CalculatePerformanceMetrics(projects, tasks, user),
                    GeneratedAt = DateTime.UtcNow
                };

                return Response<DashboardSummaryDto>.SuccessResponse(dashboard, "Dashboard summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard summary for user {UserId}", userId);
                return Response<DashboardSummaryDto>.FailureResponse(
                    "An error occurred while generating dashboard summary", 500);
            }
        }

        public async Task<Response<ProjectProgressDto>> GetProjectProgressAsync(Guid projectId, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectProgressDto>.FailureResponse("User not found", 404);
                }

                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null || project.IsDeleted)
                {
                    return Response<ProjectProgressDto>.FailureResponse("Project not found", 404);
                }

                // Check permissions
                if (!await HasProjectAccess(project, user))
                {
                    return Response<ProjectProgressDto>.FailureResponse(
                        "You don't have permission to view this project", 403);
                }

                var progress = await CalculateProjectProgress(project);
                return Response<ProjectProgressDto>.SuccessResponse(progress, "Project progress retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project progress for project {ProjectId}", projectId);
                return Response<ProjectProgressDto>.FailureResponse(
                    "An error occurred while retrieving project progress", 500);
            }
        }

        public async Task<Response<IEnumerable<ProjectProgressDto>>> GetAllProjectsProgressAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<ProjectProgressDto>>.FailureResponse("User not found", 404);
                }

                var projects = await GetAccessibleProjects(user);
                var progressList = new List<ProjectProgressDto>();

                foreach (var project in projects)
                {
                    var progress = await CalculateProjectProgress(project);
                    progressList.Add(progress);
                }

                return Response<IEnumerable<ProjectProgressDto>>.SuccessResponse(
                    progressList, "Projects progress retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all projects progress for user {UserId}", userId);
                return Response<IEnumerable<ProjectProgressDto>>.FailureResponse(
                    "An error occurred while retrieving projects progress", 500);
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

                var performance = await CalculateUserPerformance(user);
                return Response<UserPerformanceDto>.SuccessResponse(performance, "User performance retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance for user {UserId}", userId);
                return Response<UserPerformanceDto>.FailureResponse(
                    "An error occurred while retrieving user performance", 500);
            }
        }

        public async Task<Response<IEnumerable<UserPerformanceDto>>> GetTeamPerformanceAsync(Guid requestingUserId)
        {
            try
            {
                var requestingUser = await _unitOfWork.Users.GetByIdAsync(requestingUserId);
                if (requestingUser == null)
                {
                    return Response<IEnumerable<UserPerformanceDto>>.FailureResponse("User not found", 404);
                }

                // Only admins and project managers can view team performance
                if (requestingUser.Role != UserRole.Admin && requestingUser.Role != UserRole.ProjectManager)
                {
                    return Response<IEnumerable<UserPerformanceDto>>.FailureResponse(
                        "You don't have permission to view team performance", 403);
                }

                // Get all active users
                var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted && u.IsActive);
                var performanceList = new List<UserPerformanceDto>();

                foreach (var user in users)
                {
                    // For project managers, only show team members (not admins)
                    if (requestingUser.Role == UserRole.ProjectManager && user.Role == UserRole.Admin)
                    {
                        continue;
                    }

                    var performance = await CalculateUserPerformance(user);
                    performanceList.Add(performance);
                }

                // Order by productivity score
                performanceList = performanceList.OrderByDescending(p => p.ProductivityScore).ToList();

                return Response<IEnumerable<UserPerformanceDto>>.SuccessResponse(
                    performanceList, "Team performance retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team performance for user {UserId}", requestingUserId);
                return Response<IEnumerable<UserPerformanceDto>>.FailureResponse(
                    "An error occurred while retrieving team performance", 500);
            }
        }

        public async Task<Response<PerformanceMetricsDto>> GetPerformanceMetricsAsync(DashboardFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PerformanceMetricsDto>.FailureResponse("User not found", 404);
                }

                var projects = await GetAccessibleProjects(user);
                var tasks = new List<TaskItem>();

                foreach (var project in projects)
                {
                    var projectTasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.Id);
                    tasks.AddRange(projectTasks.Where(t => !t.IsDeleted));
                }

                var metrics = await CalculatePerformanceMetrics(projects, tasks, user);
                return Response<PerformanceMetricsDto>.SuccessResponse(metrics, "Performance metrics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance metrics for user {UserId}", userId);
                return Response<PerformanceMetricsDto>.FailureResponse(
                    "An error occurred while retrieving performance metrics", 500);
            }
        }

        public async Task<Response<IEnumerable<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10, Guid? userId = null)
        {
            try
            {
                var activities = new List<RecentActivityDto>();

                // Get recent tasks
                var tasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);

                if (userId.HasValue)
                {
                    tasks = tasks.Where(t => t.AssignedToUserId == userId.Value || t.CreatedByUserId == userId.Value);
                }

                var recentTasks = tasks
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(count)
                    .ToList();

                foreach (var task in recentTasks)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(task.CreatedByUserId);
                    var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);

                    activities.Add(new RecentActivityDto
                    {
                        Timestamp = task.UpdatedAt,
                        User = user?.Username ?? "Unknown",
                        Action = task.CreatedAt == task.UpdatedAt ? "Created" : "Updated",
                        Entity = "Task",
                        EntityName = task.Title,
                        Details = $"Status: {task.Status}, Priority: {task.Priority}"
                    });
                }

                // Get recent projects
                var projects = await _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);

                if (userId.HasValue)
                {
                    projects = projects.Where(p => p.CreatedByUserId == userId.Value);
                }

                var recentProjects = projects
                    .OrderByDescending(p => p.UpdatedAt)
                    .Take(count)
                    .ToList();

                foreach (var project in recentProjects)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(project.CreatedByUserId);

                    activities.Add(new RecentActivityDto
                    {
                        Timestamp = project.UpdatedAt,
                        User = user?.Username ?? "Unknown",
                        Action = project.CreatedAt == project.UpdatedAt ? "Created" : "Updated",
                        Entity = "Project",
                        EntityName = project.Name,
                        Details = project.IsActive ? "Active" : "Inactive"
                    });
                }

                // Sort by timestamp and take top count
                activities = activities
                    .OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .ToList();

                return Response<IEnumerable<RecentActivityDto>>.SuccessResponse(
                    activities, "Recent activities retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                return Response<IEnumerable<RecentActivityDto>>.FailureResponse(
                    "An error occurred while retrieving recent activities", 500);
            }
        }

        #region Private Methods

        private async Task<ProjectStatisticsDto> GetProjectStatisticsAsync(IEnumerable<Project> projects, List<TaskItem> tasks)
        {
            var projectList = projects.ToList();
            var activeProjects = projectList.Count(p => p.IsActive);
            var inactiveProjects = projectList.Count(p => !p.IsActive);

            // Calculate project completion
            var completedProjects = 0;
            var inProgressProjects = 0;
            var projectProgress = new List<ProjectProgressDto>();

            foreach (var project in projectList)
            {
                var projectTasks = tasks.Where(t => t.ProjectId == project.Id).ToList();
                var totalTasks = projectTasks.Count;
                var completedTasks = projectTasks.Count(t => t.Status == TaskItemStatus.Completed);

                var progress = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

                if (progress == 100 && totalTasks > 0)
                {
                    completedProjects++;
                }
                else if (progress > 0 && progress < 100)
                {
                    inProgressProjects++;
                }

                projectProgress.Add(new ProjectProgressDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    Progress = Math.Round(progress, 2),
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    EndDate = project.EndDate,
                    IsOverdue = project.EndDate.HasValue && project.EndDate < DateTime.UtcNow && progress < 100
                });
            }

            // Group projects by month
            var projectsByMonth = projectList
                .GroupBy(p => new DateTime(p.CreatedAt.Year, p.CreatedAt.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            return new ProjectStatisticsDto
            {
                TotalProjects = projectList.Count,
                ActiveProjects = activeProjects,
                InactiveProjects = inactiveProjects,
                ProjectsCompleted = completedProjects,
                ProjectsInProgress = inProgressProjects,
                ProjectCompletionRate = projectList.Count > 0
                    ? Math.Round((double)completedProjects / projectList.Count * 100, 2)
                    : 0,
                ProjectProgress = projectProgress,
                ProjectsByMonth = projectsByMonth
            };
        }

        private async Task<TaskStatisticsDto> GetTaskStatisticsAsync(List<TaskItem> tasks, IEnumerable<Project> projects)
        {
            var taskList = tasks.ToList();
            var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
            var inProgressTasks = taskList.Count(t => t.Status == TaskItemStatus.InProgress);
            var toDoTasks = taskList.Count(t => t.Status == TaskItemStatus.ToDo);
            var cancelledTasks = taskList.Count(t => t.Status == TaskItemStatus.Cancelled);
            var overdueTasks = taskList.Count(t =>
                t.DueDate < DateTime.UtcNow &&
                t.Status != TaskItemStatus.Completed &&
                t.Status != TaskItemStatus.Cancelled);

            // Calculate average completion time
            var completedTaskList = taskList.Where(t => t.Status == TaskItemStatus.Completed).ToList();
            var avgCompletionTime = 0.0;
            if (completedTaskList.Any())
            {
                var totalDays = completedTaskList.Sum(t => (t.UpdatedAt - t.CreatedAt).TotalDays);
                avgCompletionTime = Math.Round(totalDays / completedTaskList.Count, 2);
            }

            // Tasks by assignee
            var tasksByAssignee = taskList
                .Where(t => t.AssignedToUserId.HasValue)
                .GroupBy(t => t.AssignedToUserId!.Value)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            // Get assignee names
            var assigneeNames = new Dictionary<string, int>();
            foreach (var kvp in tasksByAssignee)
            {
                if (Guid.TryParse(kvp.Key, out var userId))
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userId);
                    var userName = user?.Username ?? "Unknown";
                    assigneeNames[userName] = kvp.Value;
                }
            }

            // Task trends (last 30 days)
            var taskTrends = new List<TaskTrendDto>();
            var startDate = DateTime.UtcNow.AddDays(-30);

            for (int i = 0; i < 30; i++)
            {
                var date = startDate.AddDays(i);
                var dayStart = date.Date;
                var dayEnd = dayStart.AddDays(1);

                var created = taskList.Count(t => t.CreatedAt >= dayStart && t.CreatedAt < dayEnd);
                var completed = taskList.Count(t =>
                    t.UpdatedAt >= dayStart &&
                    t.UpdatedAt < dayEnd &&
                    t.Status == TaskItemStatus.Completed);
                var inProgress = taskList.Count(t =>
                    t.UpdatedAt >= dayStart &&
                    t.UpdatedAt < dayEnd &&
                    t.Status == TaskItemStatus.InProgress);

                taskTrends.Add(new TaskTrendDto
                {
                    Period = date,
                    Created = created,
                    Completed = completed,
                    InProgress = inProgress
                });
            }

            return new TaskStatisticsDto
            {
                TotalTasks = taskList.Count,
                CompletedTasks = completedTasks,
                InProgressTasks = inProgressTasks,
                ToDoTasks = toDoTasks,
                CancelledTasks = cancelledTasks,
                OverdueTasks = overdueTasks,
                TasksDueThisWeek = taskList.Count(t =>
                    t.DueDate >= DateTime.UtcNow &&
                    t.DueDate <= DateTime.UtcNow.AddDays(7) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled),
                TasksDueNextWeek = taskList.Count(t =>
                    t.DueDate > DateTime.UtcNow.AddDays(7) &&
                    t.DueDate <= DateTime.UtcNow.AddDays(14) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled),
                CompletionRate = taskList.Count > 0
                    ? Math.Round((double)completedTasks / taskList.Count * 100, 2)
                    : 0,
                AverageCompletionTime = avgCompletionTime,
                TasksByStatus = taskList.GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TasksByPriority = taskList.GroupBy(t => t.Priority)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TasksByAssignee = assigneeNames,
                TaskTrends = taskTrends
            };
        }

        private async Task<UserStatisticsDto> GetUserStatisticsAsync(User currentUser)
        {
            var allUsers = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted);
            var userList = allUsers.ToList();

            var usersByRole = userList
                .GroupBy(u => u.Role.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Users by activity (based on task assignments)
            var usersByActivity = new Dictionary<string, int>();
            var tasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);
            var taskList = tasks.ToList();

            foreach (var user in userList)
            {
                var assignedTasks = taskList.Count(t => t.AssignedToUserId == user.Id);
                var activityLevel = assignedTasks switch
                {
                    >= 10 => "Very Active",
                    >= 5 => "Active",
                    >= 1 => "Moderate",
                    _ => "Inactive"
                };

                if (!usersByActivity.ContainsKey(activityLevel))
                    usersByActivity[activityLevel] = 0;
                usersByActivity[activityLevel]++;
            }

            // Top performers (only if current user is admin or manager)
            var topPerformers = new List<UserPerformanceDto>();
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.ProjectManager)
            {
                foreach (var user in userList)
                {
                    var performance = await CalculateUserPerformance(user);
                    topPerformers.Add(performance);
                }
                topPerformers = topPerformers.OrderByDescending(p => p.ProductivityScore).Take(5).ToList();
            }

            return new UserStatisticsDto
            {
                TotalUsers = userList.Count,
                ActiveUsers = userList.Count(u => u.IsActive),
                InactiveUsers = userList.Count(u => !u.IsActive),
                UsersByRole = usersByRole,
                UsersByActivity = usersByActivity,
                TopPerformers = topPerformers
            };
        }

        private async Task<ActivitySummaryDto> GetActivitySummaryAsync(List<TaskItem> tasks, User currentUser)
        {
            var recentActivities = new List<RecentActivityDto>();

            // Get recent tasks (last 10)
            var recentTasks = tasks
                .OrderByDescending(t => t.UpdatedAt)
                .Take(10)
                .ToList();

            foreach (var task in recentTasks)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(task.CreatedByUserId);
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);

                recentActivities.Add(new RecentActivityDto
                {
                    Timestamp = task.UpdatedAt,
                    User = user?.Username ?? "Unknown",
                    Action = task.CreatedAt == task.UpdatedAt ? "Created" : "Updated",
                    Entity = "Task",
                    EntityName = task.Title,
                    Details = $"{project?.Name} - {task.Status}"
                });
            }

            // Activity types
            var activityTypes = new Dictionary<string, int>
            {
                ["Task Created"] = tasks.Count(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
                ["Task Completed"] = tasks.Count(t =>
                    t.Status == TaskItemStatus.Completed &&
                    t.UpdatedAt >= DateTime.UtcNow.AddDays(-7)),
                ["Task Updated"] = tasks.Count(t =>
                    t.CreatedAt != t.UpdatedAt &&
                    t.UpdatedAt >= DateTime.UtcNow.AddDays(-7))
            };

            // Activity timeline (last 7 days)
            var activityTimeline = new Dictionary<DateTime, int>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i).Date;
                var count = tasks.Count(t => t.UpdatedAt.Date == date);
                activityTimeline[date] = count;
            }

            return new ActivitySummaryDto
            {
                RecentActivities = recentActivities,
                ActivityTypes = activityTypes,
                ActivityTimeline = activityTimeline
            };
        }

        private async Task<PerformanceMetricsDto> CalculatePerformanceMetrics(IEnumerable<Project> projects, List<TaskItem> tasks, User user)
        {
            var projectList = projects.ToList();
            var taskList = tasks.ToList();

            // Overall productivity
            var totalTasks = taskList.Count;
            var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
            var onTimeTasks = taskList.Count(t =>
                t.Status == TaskItemStatus.Completed &&
                t.UpdatedAt <= t.DueDate);

            var productivity = totalTasks > 0
                ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                : 0;

            var projectSuccessRate = projectList.Count > 0
                ? Math.Round((double)projectList.Count(p =>
                    p.Tasks.Any() &&
                    p.Tasks.Count(t => !t.IsDeleted && t.Status == TaskItemStatus.Completed) == p.Tasks.Count(t => !t.IsDeleted)
                ) / projectList.Count * 100, 2)
                : 0;

            var taskEfficiency = completedTasks > 0
                ? Math.Round((double)onTimeTasks / completedTasks * 100, 2)
                : 0;

            var onTimeDeliveryRate = totalTasks > 0
                ? Math.Round((double)onTimeTasks / totalTasks * 100, 2)
                : 0;

            var resourceUtilization = totalTasks > 0
                ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                : 0;

            // Metrics by project
            var metricsByProject = new Dictionary<string, double>();
            foreach (var project in projectList)
            {
                var projectTasks = taskList.Where(t => t.ProjectId == project.Id).ToList();
                if (projectTasks.Any())
                {
                    var projectCompleted = projectTasks.Count(t => t.Status == TaskItemStatus.Completed);
                    var projectTotal = projectTasks.Count;
                    var projectSuccess = Math.Round((double)projectCompleted / projectTotal * 100, 2);
                    metricsByProject[project.Name] = projectSuccess;
                }
            }

            // Metrics trend (last 30 days)
            var metricsTrend = new List<KeyValuePair<string, double>>();
            for (int i = 30; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                var dailyTasks = taskList.Where(t => t.CreatedAt.Date == date.Date).ToList();
                if (dailyTasks.Any())
                {
                    var dailyCompleted = dailyTasks.Count(t => t.Status == TaskItemStatus.Completed);
                    var dailyProductivity = Math.Round((double)dailyCompleted / dailyTasks.Count * 100, 2);
                    metricsTrend.Add(new KeyValuePair<string, double>(
                        date.ToString("MMM dd"),
                        dailyProductivity
                    ));
                }
            }

            return new PerformanceMetricsDto
            {
                OverallProductivity = productivity,
                ProjectSuccessRate = projectSuccessRate,
                TaskEfficiency = taskEfficiency,
                OnTimeDeliveryRate = onTimeDeliveryRate,
                ResourceUtilization = resourceUtilization,
                MetricsByProject = metricsByProject,
                MetricsTrend = metricsTrend
            };
        }

        private async Task<ProjectProgressDto> CalculateProjectProgress(Project project)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.Id);
            var taskList = tasks.Where(t => !t.IsDeleted).ToList();
            var totalTasks = taskList.Count;
            var completedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);

            var progress = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

            return new ProjectProgressDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Progress = Math.Round(progress, 2),
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                EndDate = project.EndDate,
                IsOverdue = project.EndDate.HasValue &&
                            project.EndDate < DateTime.UtcNow &&
                            progress < 100
            };
        }

        private async Task<UserPerformanceDto> CalculateUserPerformance(User user)
        {
            var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                !t.IsDeleted &&
                (t.AssignedToUserId == user.Id || t.CreatedByUserId == user.Id));

            var taskList = tasks.ToList();
            var assignedTasks = taskList.Count(t => t.AssignedToUserId == user.Id);
            var completedTasks = taskList.Count(t =>
                t.Status == TaskItemStatus.Completed &&
                t.AssignedToUserId == user.Id);
            var overdueTasks = taskList.Count(t =>
                t.AssignedToUserId == user.Id &&
                t.DueDate < DateTime.UtcNow &&
                t.Status != TaskItemStatus.Completed &&
                t.Status != TaskItemStatus.Cancelled);

            var completionRate = assignedTasks > 0
                ? Math.Round((double)completedTasks / assignedTasks * 100, 2)
                : 0;

            // Average completion time
            var completedTaskList = taskList
                .Where(t => t.Status == TaskItemStatus.Completed && t.AssignedToUserId == user.Id)
                .ToList();

            var avgCompletionTime = 0.0;
            if (completedTaskList.Any())
            {
                var totalDays = completedTaskList.Sum(t => (t.UpdatedAt - t.CreatedAt).TotalDays);
                avgCompletionTime = Math.Round(totalDays / completedTaskList.Count, 2);
            }

            // Productivity score (weighted calculation)
            var productivityScore = (completionRate * 0.4) +
                                    (Math.Min(assignedTasks / 10.0, 1.0) * 30) +
                                    (Math.Max(0, (100 - (avgCompletionTime * 2))) * 0.3);

            productivityScore = Math.Min(100, productivityScore);

            return new UserPerformanceDto
            {
                UserId = user.Id,
                UserName = user.Username,
                FullName = $"{user.FirstName} {user.LastName}",
                AssignedTasks = assignedTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                CompletionRate = completionRate,
                AverageCompletionTime = avgCompletionTime,
                ProductivityScore = Math.Round(productivityScore, 2)
            };
        }

        private async Task<IEnumerable<Project>> GetAccessibleProjects(User user)
        {
            if (user.Role == UserRole.Admin)
            {
                return await _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);
            }
            else if (user.Role == UserRole.ProjectManager)
            {
                return await _unitOfWork.Projects.FindAsync(p =>
                    !p.IsDeleted && p.CreatedByUserId == user.Id);
            }
            else // Team Member
            {
                // Get projects where user is assigned to tasks or created the project
                var allProjects = await _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);
                var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                    !t.IsDeleted &&
                    (t.AssignedToUserId == user.Id || t.CreatedByUserId == user.Id));

                var projectIds = tasks.Select(t => t.ProjectId).Distinct();
                return allProjects.Where(p => projectIds.Contains(p.Id));
            }
        }

        private async Task<bool> HasProjectAccess(Project project, User user)
        {
            if (user.Role == UserRole.Admin)
                return true;

            if (user.Role == UserRole.ProjectManager && project.CreatedByUserId == user.Id)
                return true;

            // Check if user is assigned to any task in the project
            var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                !t.IsDeleted &&
                t.ProjectId == project.Id &&
                t.AssignedToUserId == user.Id);

            return tasks.Any();
        }

        #endregion
    }
}