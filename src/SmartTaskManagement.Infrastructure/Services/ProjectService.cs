using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<ProjectDto>> CreateProjectAsync(CreateProjectDto createDto, Guid userId)
        {
            try
            {
                // Validate user exists
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectDto>.FailureResponse("User not found", 404);
                }

                // Validate dates
                if (createDto.EndDate.HasValue && createDto.EndDate < createDto.StartDate)
                {
                    return Response<ProjectDto>.FailureResponse("End date must be after start date", 400);
                }

                // Create project
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = createDto.Name,
                    Description = createDto.Description,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    IsActive = true,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Projects.AddAsync(project);
                await _unitOfWork.CompleteAsync();

                var projectDto = await MapToProjectDto(project);

                _logger.LogInformation("Project {ProjectName} created by user {UserId}", project.Name, userId);
                return Response<ProjectDto>.SuccessResponse(projectDto, "Project created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project for user {UserId}", userId);
                return Response<ProjectDto>.FailureResponse("An error occurred while creating the project", 500);
            }
        }

        public async Task<Response<ProjectDto>> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateDto, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null)
                {
                    return Response<ProjectDto>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission (Admin or Project Owner)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectDto>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId && user.Role != UserRole.Admin)
                {
                    return Response<ProjectDto>.FailureResponse("You don't have permission to update this project", 403);
                }

                // Validate dates
                if (updateDto.EndDate.HasValue && updateDto.EndDate < updateDto.StartDate)
                {
                    return Response<ProjectDto>.FailureResponse("End date must be after start date", 400);
                }

                // Update project
                project.Name = updateDto.Name;
                project.Description = updateDto.Description;
                project.StartDate = updateDto.StartDate;
                project.EndDate = updateDto.EndDate;
                project.IsActive = updateDto.IsActive;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = user.Username;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.CompleteAsync();

                var projectDto = await MapToProjectDto(project);

                _logger.LogInformation("Project {ProjectName} updated by user {UserId}", project.Name, userId);
                return Response<ProjectDto>.SuccessResponse(projectDto, "Project updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId} for user {UserId}", projectId, userId);
                return Response<ProjectDto>.FailureResponse("An error occurred while updating the project", 500);
            }
        }

        public async Task<Response<bool>> DeleteProjectAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
                if (project == null)
                {
                    return Response<bool>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission (Admin or Project Owner)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId && user.Role != UserRole.Admin)
                {
                    return Response<bool>.FailureResponse("You don't have permission to delete this project", 403);
                }

                // Soft delete
                project.IsDeleted = true;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = user.Username;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Project {ProjectName} deleted by user {UserId}", project.Name, userId);
                return Response<bool>.SuccessResponse(true, "Project deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId} for user {UserId}", projectId, userId);
                return Response<bool>.FailureResponse("An error occurred while deleting the project", 500);
            }
        }

        public async Task<Response<ProjectDto>> GetProjectByIdAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null)
                {
                    return Response<ProjectDto>.FailureResponse("Project not found", 404);
                }

                // Check if user has permission to view
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ProjectDto>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId &&
                    user.Role != UserRole.Admin &&
                    user.Role != UserRole.ProjectManager)
                {
                    return Response<ProjectDto>.FailureResponse("You don't have permission to view this project", 403);
                }

                var projectDto = await MapToProjectDto(project);
                return Response<ProjectDto>.SuccessResponse(projectDto, "Project retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {ProjectId} for user {UserId}", projectId, userId);
                return Response<ProjectDto>.FailureResponse("An error occurred while retrieving the project", 500);
            }
        }

        public async Task<Response<PagedResponse<ProjectDto>>> GetProjectsAsync(ProjectFilterDto filter, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PagedResponse<ProjectDto>>.FailureResponse("User not found", 404);
                }

                // Build query
                var query = _unitOfWork.Projects.FindAsync(p => !p.IsDeleted);

                // Apply filters
                var projects = await query;
                var filteredProjects = projects.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    filteredProjects = filteredProjects.Where(p =>
                        p.Name.Contains(filter.SearchTerm) ||
                        p.Description.Contains(filter.SearchTerm));
                }

                if (filter.IsActive.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.IsActive == filter.IsActive.Value);
                }

                if (filter.StartDateFrom.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.StartDate >= filter.StartDateFrom.Value);
                }

                if (filter.StartDateTo.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.StartDate <= filter.StartDateTo.Value);
                }

                // Apply user filter for non-admins
                if (user.Role != UserRole.Admin)
                {
                    filteredProjects = filteredProjects.Where(p => p.CreatedByUserId == userId);
                }
                else if (filter.CreatedByUserId.HasValue)
                {
                    filteredProjects = filteredProjects.Where(p => p.CreatedByUserId == filter.CreatedByUserId.Value);
                }

                // Apply sorting
                filteredProjects = ApplySorting(filteredProjects, filter.SortBy, filter.SortDescending);

                // Get total count
                var totalCount = filteredProjects.Count();

                // Apply pagination
                var pagedProjects = filteredProjects
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                // Map to DTOs
                var projectDtos = new List<ProjectDto>();
                foreach (var project in pagedProjects)
                {
                    var dto = await MapToProjectDto(project);
                    projectDtos.Add(dto);
                }

                var response = new PagedResponse<ProjectDto>
                {
                    Items = projectDtos,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };

                return Response<PagedResponse<ProjectDto>>.SuccessResponse(response, "Projects retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
                return Response<PagedResponse<ProjectDto>>.FailureResponse(
                    "An error occurred while retrieving projects", 500);
            }
        }

        public async Task<Response<ProjectTaskSummaryDto>> GetProjectTaskSummaryAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetProjectWithTasksAsync(projectId);
                if (project == null)
                {
                    return Response<ProjectTaskSummaryDto>.FailureResponse("Project not found", 404);
                }

                var activeTasks = project.Tasks.Where(t => !t.IsDeleted).ToList();
                var totalTasks = activeTasks.Count;
                var completedTasks = activeTasks.Count(t => t.Status == TaskItemStatus.Completed);
                var inProgressTasks = activeTasks.Count(t => t.Status == TaskItemStatus.InProgress);
                var toDoTasks = activeTasks.Count(t => t.Status == TaskItemStatus.ToDo);
                var cancelledTasks = activeTasks.Count(t => t.Status == TaskItemStatus.Cancelled);

                var summary = new ProjectTaskSummaryDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    InProgressTasks = inProgressTasks,
                    ToDoTasks = toDoTasks,
                    CancelledTasks = cancelledTasks,
                    CompletionPercentage = totalTasks > 0
                        ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                        : 0
                };

                return Response<ProjectTaskSummaryDto>.SuccessResponse(summary, "Project summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project summary for project {ProjectId}", projectId);
                return Response<ProjectTaskSummaryDto>.FailureResponse(
                    "An error occurred while retrieving project summary", 500);
            }
        }

        public async Task<Response<bool>> ToggleProjectStatusAsync(Guid projectId, Guid userId)
        {
            try
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
                if (project == null)
                {
                    return Response<bool>.FailureResponse("Project not found", 404);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                if (project.CreatedByUserId != userId && user.Role != UserRole.Admin)
                {
                    return Response<bool>.FailureResponse("You don't have permission to modify this project", 403);
                }

                project.IsActive = !project.IsActive;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = user.Username;

                _unitOfWork.Projects.Update(project);
                await _unitOfWork.CompleteAsync();

                var status = project.IsActive ? "activated" : "deactivated";
                _logger.LogInformation("Project {ProjectName} {Status} by user {UserId}", project.Name, status, userId);
                return Response<bool>.SuccessResponse(true, $"Project {status} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling project status for project {ProjectId}", projectId);
                return Response<bool>.FailureResponse("An error occurred while toggling project status", 500);
            }
        }

        #region Private Methods

        private async Task<ProjectDto> MapToProjectDto(Project project)
        {
            var activeTasks = project.Tasks?.Where(t => !t.IsDeleted).ToList() ?? new List<TaskItem>();
            var completedTasks = activeTasks.Count(t => t.Status == TaskItemStatus.Completed);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                IsActive = project.IsActive,
                TaskCount = activeTasks.Count,
                CompletedTasks = completedTasks,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                CreatedByUser = new Application.DTOs.Shared.UserDto
                {
                    Id = project.CreatedByUser?.Id ?? Guid.Empty,
                    Email = project.CreatedByUser?.Email ?? string.Empty,
                    Username = project.CreatedByUser?.Username ?? string.Empty,
                    FirstName = project.CreatedByUser?.FirstName ?? string.Empty,
                    LastName = project.CreatedByUser?.LastName ?? string.Empty,
                    Role = project.CreatedByUser?.Role.ToString() ?? string.Empty
                }
            };
        }

        private IQueryable<Project> ApplySorting(IQueryable<Project> query, string sortBy, bool sortDescending)
        {
            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "startdate" => sortDescending
                    ? query.OrderByDescending(p => p.StartDate)
                    : query.OrderBy(p => p.StartDate),
                "enddate" => sortDescending
                    ? query.OrderByDescending(p => p.EndDate)
                    : query.OrderBy(p => p.EndDate),
                "isactive" => sortDescending
                    ? query.OrderByDescending(p => p.IsActive)
                    : query.OrderBy(p => p.IsActive),
                "taskcount" => sortDescending
                    ? query.OrderByDescending(p => p.Tasks.Count(t => !t.IsDeleted))
                    : query.OrderBy(p => p.Tasks.Count(t => !t.IsDeleted)),
                _ => sortDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt)
            };
        }

        #endregion
    }
}