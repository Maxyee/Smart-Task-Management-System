using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Tasks;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.DTOs.Shared;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskService> _logger;

        public TaskService(IUnitOfWork unitOfWork, ILogger<TaskService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<TaskDto>> CreateTaskAsync(CreateTaskDto createDto, Guid userId)
        {
            try
            {
                // Validate project exists
                var project = await _unitOfWork.Projects.GetByIdAsync(createDto.ProjectId);
                if (project == null || project.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission to create tasks in this project
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to create tasks in this project", 403);
                }

                // Validate assigned user if provided
                if (createDto.AssignedToUserId.HasValue)
                {
                    var assignedUser = await _unitOfWork.Users.GetByIdAsync(createDto.AssignedToUserId.Value);
                    if (assignedUser == null || assignedUser.IsDeleted)
                    {
                        return Response<TaskDto>.FailureResponse("Assigned user not found", 404);
                    }
                }

                // Validate due date
                if (createDto.DueDate < DateTime.UtcNow.Date)
                {
                    return Response<TaskDto>.FailureResponse("Due date cannot be in the past", 400);
                }

                // Create task
                var task = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = createDto.Title,
                    Description = createDto.Description,
                    Status = TaskItemStatus.ToDo,
                    Priority = createDto.Priority,
                    DueDate = createDto.DueDate,
                    EstimatedHours = createDto.EstimatedHours,
                    ActualHours = 0,
                    ProjectId = createDto.ProjectId,
                    AssignedToUserId = createDto.AssignedToUserId,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.CompleteAsync();

                // Get the complete task with navigation properties
                var createdTask = await _unitOfWork.Tasks.GetByIdAsync(task.Id);
                var taskDto = await MapToTaskDto(createdTask!);

                _logger.LogInformation("Task {TaskTitle} created in project {ProjectName} by user {UserId}",
                    task.Title, project.Name, userId);

                return Response<TaskDto>.SuccessResponse(taskDto, "Task created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task by user {UserId}", userId);
                return Response<TaskDto>.FailureResponse("An error occurred while creating the task", 500);
            }
        }

        public async Task<Response<TaskDto>> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateDto, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check permission
                if (!await HasTaskPermission(task, userId, user.Role))
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to update this task", 403);
                }

                // Validate assigned user if provided
                if (updateDto.AssignedToUserId.HasValue)
                {
                    var assignedUser = await _unitOfWork.Users.GetByIdAsync(updateDto.AssignedToUserId.Value);
                    if (assignedUser == null || assignedUser.IsDeleted)
                    {
                        return Response<TaskDto>.FailureResponse("Assigned user not found", 404);
                    }
                }

                // Validate due date
                if (updateDto.DueDate < DateTime.UtcNow.Date &&
                    task.Status != TaskItemStatus.Completed &&
                    task.Status != TaskItemStatus.Cancelled)
                {
                    return Response<TaskDto>.FailureResponse("Due date cannot be in the past for active tasks", 400);
                }

                // Update task
                task.Title = updateDto.Title;
                task.Description = updateDto.Description;
                task.Priority = updateDto.Priority;
                task.DueDate = updateDto.DueDate;
                task.EstimatedHours = updateDto.EstimatedHours;
                task.ActualHours = updateDto.ActualHours;
                task.AssignedToUserId = updateDto.AssignedToUserId;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                var taskDto = await MapToTaskDto(task);

                _logger.LogInformation("Task {TaskTitle} updated by user {UserId}", task.Title, userId);
                return Response<TaskDto>.SuccessResponse(taskDto, "Task updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId} by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while updating the task", 500);
            }
        }

        public async Task<Response<bool>> DeleteTaskAsync(Guid taskId, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                // Check permission
                if (!await HasTaskPermission(task, userId, user.Role))
                {
                    return Response<bool>.FailureResponse(
                        "You don't have permission to delete this task", 403);
                }

                // Soft delete
                task.IsDeleted = true;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Task {TaskTitle} deleted by user {UserId}", task.Title, userId);
                return Response<bool>.SuccessResponse(true, "Task deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId} by user {UserId}", taskId, userId);
                return Response<bool>.FailureResponse("An error occurred while deleting the task", 500);
            }
        }

        public async Task<Response<TaskDto>> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check permission - anyone can view if they're part of the project
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null || project.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                if (project.CreatedByUserId != userId &&
                    task.AssignedToUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to view this task", 403);
                }

                var taskDto = await MapToTaskDto(task);
                return Response<TaskDto>.SuccessResponse(taskDto, "Task retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId} by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while retrieving the task", 500);
            }
        }

        public async Task<Response<PagedResponse<TaskDto>>> GetTasksAsync(TaskFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PagedResponse<TaskDto>>.FailureResponse("User not found", 404);
                }

                // Get all tasks
                var allTasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);
                var query = allTasks.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(t =>
                        t.Title.Contains(filter.SearchTerm) ||
                        t.Description.Contains(filter.SearchTerm));
                }

                if (filter.ProjectId.HasValue)
                {
                    query = query.Where(t => t.ProjectId == filter.ProjectId.Value);
                }

                if (filter.AssignedToUserId.HasValue)
                {
                    query = query.Where(t => t.AssignedToUserId == filter.AssignedToUserId.Value);
                }

                if (filter.Status.HasValue)
                {
                    query = query.Where(t => t.Status == filter.Status.Value);
                }

                if (filter.Priority.HasValue)
                {
                    query = query.Where(t => t.Priority == filter.Priority.Value);
                }

                if (filter.DueDateFrom.HasValue)
                {
                    query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);
                }

                if (filter.DueDateTo.HasValue)
                {
                    query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);
                }

                if (filter.IsOverdue.HasValue && filter.IsOverdue.Value)
                {
                    query = query.Where(t =>
                        t.DueDate < DateTime.UtcNow &&
                        t.Status != TaskItemStatus.Completed &&
                        t.Status != TaskItemStatus.Cancelled);
                }

                // Apply user filter for non-admins
                if (user.Role != UserRole.Admin && user.Role != UserRole.ProjectManager)
                {
                    // Team members can only see tasks assigned to them or tasks from projects they created
                    query = query.Where(t =>
                        t.AssignedToUserId == userId ||
                        t.CreatedByUserId == userId);
                }

                if (filter.ShowOnlyAssignedToMe.HasValue && filter.ShowOnlyAssignedToMe.Value)
                {
                    query = query.Where(t => t.AssignedToUserId == userId);
                }

                // Get total count before pagination
                var totalCount = query.Count();

                // Apply sorting
                query = ApplyTaskSorting(query, filter.SortBy, filter.SortDescending);

                // Apply pagination
                var pagedTasks = query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                // Map to DTOs
                var taskDtos = new List<TaskDto>();
                foreach (var task in pagedTasks)
                {
                    var dto = await MapToTaskDto(task);
                    taskDtos.Add(dto);
                }

                var response = new PagedResponse<TaskDto>
                {
                    Items = taskDtos,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };

                return Response<PagedResponse<TaskDto>>.SuccessResponse(response, "Tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user {UserId}", userId);
                return Response<PagedResponse<TaskDto>>.FailureResponse(
                    "An error occurred while retrieving tasks", 500);
            }
        }

        public async Task<Response<TaskDto>> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto statusDto, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check if user has permission to update status
                // Assigned user, creator, project manager, or admin can update status
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                if (task.AssignedToUserId != userId &&
                    task.CreatedByUserId != userId &&
                    project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to update this task's status", 403);
                }

                // Validate status transition
                if (!IsValidStatusTransition(task.Status, statusDto.Status))
                {
                    return Response<TaskDto>.FailureResponse(
                        $"Invalid status transition from {task.Status} to {statusDto.Status}", 400);
                }

                // Update status
                var oldStatus = task.Status;
                task.Status = statusDto.Status;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                // If task is completed, set actual hours if not set
                if (statusDto.Status == TaskItemStatus.Completed && task.ActualHours == 0)
                {
                    task.ActualHours = task.EstimatedHours;
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                var taskDto = await MapToTaskDto(task);

                _logger.LogInformation("Task {TaskTitle} status changed from {OldStatus} to {NewStatus} by user {UserId}",
                    task.Title, oldStatus, statusDto.Status, userId);

                return Response<TaskDto>.SuccessResponse(taskDto, "Task status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId} status by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while updating task status", 500);
            }
        }

        public async Task<Response<TaskDto>> AssignTaskAsync(Guid taskId, AssignTaskDto assignDto, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<TaskDto>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<TaskDto>.FailureResponse("User not found", 404);
                }

                // Check permission (Admin, Project Manager, or Task Creator can assign)
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null)
                {
                    return Response<TaskDto>.FailureResponse("Project not found", 404);
                }

                if (task.CreatedByUserId != userId &&
                    project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<TaskDto>.FailureResponse(
                        "You don't have permission to assign this task", 403);
                }

                // Validate user to assign
                var assignUser = await _unitOfWork.Users.GetByIdAsync(assignDto.UserId);
                if (assignUser == null || assignUser.IsDeleted || !assignUser.IsActive)
                {
                    return Response<TaskDto>.FailureResponse("User to assign not found or inactive", 404);
                }

                // Assign task
                task.AssignedToUserId = assignDto.UserId;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = user.Username;

                // If task was in ToDo, move to InProgress
                if (task.Status == TaskItemStatus.ToDo)
                {
                    task.Status = TaskItemStatus.InProgress;
                }

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.CompleteAsync();

                var taskDto = await MapToTaskDto(task);

                _logger.LogInformation("Task {TaskTitle} assigned to user {AssignUser} by {UserId}",
                    task.Title, assignUser.Username, userId);

                return Response<TaskDto>.SuccessResponse(taskDto, "Task assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId} by user {UserId}", taskId, userId);
                return Response<TaskDto>.FailureResponse("An error occurred while assigning the task", 500);
            }
        }

        public async Task<Response<TaskStatisticsDto>> GetTaskStatisticsAsync(Guid? projectId = null, Guid? userId = null)
        {
            try
            {
                var statistics = new TaskStatisticsDto();

                // Get tasks based on filters
                var tasks = await _unitOfWork.Tasks.FindAsync(t => !t.IsDeleted);
                var filteredTasks = tasks.AsQueryable();

                if (projectId.HasValue)
                {
                    filteredTasks = filteredTasks.Where(t => t.ProjectId == projectId.Value);
                }

                if (userId.HasValue)
                {
                    filteredTasks = filteredTasks.Where(t => t.AssignedToUserId == userId.Value);
                }

                var taskList = filteredTasks.ToList();

                // Calculate statistics
                statistics.TotalTasks = taskList.Count;
                statistics.CompletedTasks = taskList.Count(t => t.Status == TaskItemStatus.Completed);
                statistics.InProgressTasks = taskList.Count(t => t.Status == TaskItemStatus.InProgress);
                statistics.ToDoTasks = taskList.Count(t => t.Status == TaskItemStatus.ToDo);
                statistics.CancelledTasks = taskList.Count(t => t.Status == TaskItemStatus.Cancelled);
                statistics.OverdueTasks = taskList.Count(t =>
                    t.DueDate < DateTime.UtcNow &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);
                statistics.TasksDueThisWeek = taskList.Count(t =>
                    t.DueDate >= DateTime.UtcNow &&
                    t.DueDate <= DateTime.UtcNow.AddDays(7) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);

                statistics.CompletionRate = statistics.TotalTasks > 0
                    ? Math.Round((double)statistics.CompletedTasks / statistics.TotalTasks * 100, 2)
                    : 0;

                // Group by priority
                statistics.TasksByPriority = taskList
                    .GroupBy(t => t.Priority)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Ensure all priorities are present
                foreach (TaskItemPriority priority in Enum.GetValues(typeof(TaskItemPriority)))
                {
                    if (!statistics.TasksByPriority.ContainsKey(priority))
                    {
                        statistics.TasksByPriority[priority] = 0;
                    }
                }

                // Group by status
                statistics.TasksByStatus = taskList
                    .GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Ensure all statuses are present
                foreach (TaskItemStatus status in Enum.GetValues(typeof(TaskItemStatus)))
                {
                    if (!statistics.TasksByStatus.ContainsKey(status))
                    {
                        statistics.TasksByStatus[status] = 0;
                    }
                }

                return Response<TaskStatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task statistics");
                return Response<TaskStatisticsDto>.FailureResponse(
                    "An error occurred while retrieving statistics", 500);
            }
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetOverdueTasksAsync(Guid userId)
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetOverdueTasksAsync();

                // Filter for user if not admin
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<TaskDto>>.FailureResponse("User not found", 404);
                }

                var filteredTasks = tasks.AsEnumerable();
                if (user.Role != UserRole.Admin && user.Role != UserRole.ProjectManager)
                {
                    filteredTasks = filteredTasks.Where(t =>
                        t.AssignedToUserId == userId ||
                        t.CreatedByUserId == userId);
                }

                var taskDtos = new List<TaskDto>();
                foreach (var task in filteredTasks)
                {
                    var dto = await MapToTaskDto(task);
                    taskDtos.Add(dto);
                }

                return Response<IEnumerable<TaskDto>>.SuccessResponse(
                    taskDtos, "Overdue tasks retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue tasks for user {UserId}", userId);
                return Response<IEnumerable<TaskDto>>.FailureResponse(
                    "An error occurred while retrieving overdue tasks", 500);
            }
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetTasksDueSoonAsync(Guid userId, int daysThreshold = 7)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<TaskDto>>.FailureResponse("User not found", 404);
                }

                var tasks = await _unitOfWork.Tasks.FindAsync(t =>
                    !t.IsDeleted &&
                    t.DueDate >= DateTime.UtcNow &&
                    t.DueDate <= DateTime.UtcNow.AddDays(daysThreshold) &&
                    t.Status != TaskItemStatus.Completed &&
                    t.Status != TaskItemStatus.Cancelled);

                // Filter for user if not admin
                var filteredTasks = tasks.AsEnumerable();
                if (user.Role != UserRole.Admin && user.Role != UserRole.ProjectManager)
                {
                    filteredTasks = filteredTasks.Where(t =>
                        t.AssignedToUserId == userId ||
                        t.CreatedByUserId == userId);
                }

                var taskDtos = new List<TaskDto>();
                foreach (var task in filteredTasks.OrderBy(t => t.DueDate))
                {
                    var dto = await MapToTaskDto(task);
                    taskDtos.Add(dto);
                }

                return Response<IEnumerable<TaskDto>>.SuccessResponse(
                    taskDtos, $"Tasks due in next {daysThreshold} days retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks due soon for user {UserId}", userId);
                return Response<IEnumerable<TaskDto>>.FailureResponse(
                    "An error occurred while retrieving tasks due soon", 500);
            }
        }

        public async Task<Response<bool>> BulkUpdateStatusAsync(List<Guid> taskIds, TaskItemStatus newStatus, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                var updatedCount = 0;
                var errors = new List<string>();

                foreach (var taskId in taskIds)
                {
                    try
                    {
                        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                        if (task == null || task.IsDeleted)
                        {
                            errors.Add($"Task {taskId} not found");
                            continue;
                        }

                        // Check permission
                        if (!await HasTaskPermission(task, userId, user.Role))
                        {
                            errors.Add($"Task {taskId} - permission denied");
                            continue;
                        }

                        // Validate status transition
                        if (!IsValidStatusTransition(task.Status, newStatus))
                        {
                            errors.Add($"Task {taskId} - invalid status transition from {task.Status} to {newStatus}");
                            continue;
                        }

                        task.Status = newStatus;
                        task.UpdatedAt = DateTime.UtcNow;
                        task.UpdatedBy = user.Username;

                        _unitOfWork.Tasks.Update(task);
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Task {taskId} - {ex.Message}");
                    }
                }

                await _unitOfWork.CompleteAsync();

                var message = $"Updated {updatedCount} out of {taskIds.Count} tasks";
                if (errors.Any())
                {
                    message += $". Errors: {string.Join("; ", errors)}";
                }

                _logger.LogInformation("Bulk status update completed: {Message}", message);
                return Response<bool>.SuccessResponse(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk status update by user {UserId}", userId);
                return Response<bool>.FailureResponse("An error occurred during bulk update", 500);
            }
        }

        public async Task<Response<IEnumerable<TaskActivityDto>>> GetTaskActivityLogAsync(Guid taskId, Guid userId)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null || task.IsDeleted)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse("Task not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse("User not found", 404);
                }

                // Check permission
                var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
                if (project == null)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse("Project not found", 404);
                }

                if (task.AssignedToUserId != userId &&
                    task.CreatedByUserId != userId &&
                    project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin)
                {
                    return Response<IEnumerable<TaskActivityDto>>.FailureResponse(
                        "You don't have permission to view this task's activity", 403);
                }

                // In a real implementation, you would have a separate activity log table
                // For now, we'll return a simulated activity log based on task history
                var activities = new List<TaskActivityDto>
            {
                new TaskActivityDto
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    Action = "Created",
                    PerformedBy = task.CreatedByUser?.Username ?? "Unknown",
                    PerformedAt = task.CreatedAt,
                    Details = $"Task created with priority {task.Priority}"
                }
            };

                // Add status change activities if we had tracking
                // This is a placeholder - you would need to implement proper activity tracking

                return Response<IEnumerable<TaskActivityDto>>.SuccessResponse(
                    activities, "Task activity log retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity log for task {TaskId}", taskId);
                return Response<IEnumerable<TaskActivityDto>>.FailureResponse(
                    "An error occurred while retrieving activity log", 500);
            }
        }

        #region Private Methods

        private async Task<TaskDto> MapToTaskDto(TaskItem task)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            var assignedUser = task.AssignedToUserId.HasValue
                ? await _unitOfWork.Users.GetByIdAsync(task.AssignedToUserId.Value)
                : null;
            var createdByUser = await _unitOfWork.Users.GetByIdAsync(task.CreatedByUserId);

            var isOverdue = task.DueDate < DateTime.UtcNow &&
                            task.Status != TaskItemStatus.Completed &&
                            task.Status != TaskItemStatus.Cancelled;

            var daysUntilDue = (int)(task.DueDate - DateTime.UtcNow.Date).TotalDays;

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                ProjectId = task.ProjectId,
                ProjectName = project?.Name ?? string.Empty,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUser = assignedUser != null ? new UserDto
                {
                    Id = assignedUser.Id,
                    Email = assignedUser.Email,
                    Username = assignedUser.Username,
                    FirstName = assignedUser.FirstName,
                    LastName = assignedUser.LastName,
                    Role = assignedUser.Role.ToString()
                } : null,
                CreatedByUser = createdByUser != null ? new UserDto
                {
                    Id = createdByUser.Id,
                    Email = createdByUser.Email,
                    Username = createdByUser.Username,
                    FirstName = createdByUser.FirstName,
                    LastName = createdByUser.LastName,
                    Role = createdByUser.Role.ToString()
                } : null,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                IsOverdue = isOverdue,
                DaysUntilDue = daysUntilDue
            };
        }

        private async Task<bool> HasTaskPermission(TaskItem task, Guid userId, UserRole userRole)
        {
            if (userRole == UserRole.Admin)
                return true;

            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            if (project == null)
                return false;

            // Project Manager can manage all tasks in their projects
            if (userRole == UserRole.ProjectManager && project.CreatedByUserId == userId)
                return true;

            // Task creator can manage their tasks
            if (task.CreatedByUserId == userId)
                return true;

            // Assigned user can update their tasks (except delete)
            if (task.AssignedToUserId == userId)
                return true;

            return false;
        }

        private bool IsValidStatusTransition(TaskItemStatus currentStatus, TaskItemStatus newStatus)
        {
            // Allow same status
            if (currentStatus == newStatus)
                return true;

            // Define valid transitions
            return (currentStatus, newStatus) switch
            {
                // From ToDo
                (TaskItemStatus.ToDo, TaskItemStatus.InProgress) => true,
                (TaskItemStatus.ToDo, TaskItemStatus.Completed) => true,
                (TaskItemStatus.ToDo, TaskItemStatus.Cancelled) => true,

                // From InProgress
                (TaskItemStatus.InProgress, TaskItemStatus.Completed) => true,
                (TaskItemStatus.InProgress, TaskItemStatus.Cancelled) => true,

                // From Completed
                (TaskItemStatus.Completed, TaskItemStatus.InProgress) => true, // Can reopen
                (TaskItemStatus.Completed, TaskItemStatus.Cancelled) => true,

                // From Cancelled
                (TaskItemStatus.Cancelled, TaskItemStatus.ToDo) => true, // Can revive
                (TaskItemStatus.Cancelled, TaskItemStatus.InProgress) => true,

                _ => false
            };
        }

        private IQueryable<TaskItem> ApplyTaskSorting(IQueryable<TaskItem> query, string sortBy, bool sortDescending)
        {
            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),
                "status" => sortDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),
                "priority" => sortDescending
                    ? query.OrderByDescending(t => t.Priority)
                    : query.OrderBy(t => t.Priority),
                "duedate" => sortDescending
                    ? query.OrderByDescending(t => t.DueDate)
                    : query.OrderBy(t => t.DueDate),
                "estimatedhours" => sortDescending
                    ? query.OrderByDescending(t => t.EstimatedHours)
                    : query.OrderBy(t => t.EstimatedHours),
                "actualhours" => sortDescending
                    ? query.OrderByDescending(t => t.ActualHours)
                    : query.OrderBy(t => t.ActualHours),
                "projectname" => sortDescending
                    ? query.OrderByDescending(t => t.Project.Name)
                    : query.OrderBy(t => t.Project.Name),
                "assignedto" => sortDescending
                    ? query.OrderByDescending(t => t.AssignedToUser.Username)
                    : query.OrderBy(t => t.AssignedToUser.Username),
                _ => sortDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
            };
        }

        #endregion
    }
}