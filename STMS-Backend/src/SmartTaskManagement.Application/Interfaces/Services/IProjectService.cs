using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Projects;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<Response<ProjectDto>> CreateProjectAsync(CreateProjectDto createDto, Guid userId);
        Task<Response<ProjectDto>> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateDto, Guid userId);
        Task<Response<bool>> DeleteProjectAsync(Guid projectId, Guid userId);
        Task<Response<ProjectDto>> GetProjectByIdAsync(Guid projectId, Guid userId);
        Task<Response<PagedResponse<ProjectDto>>> GetProjectsAsync(ProjectFilterDto filter, Guid userId);
        Task<Response<ProjectTaskSummaryDto>> GetProjectTaskSummaryAsync(Guid projectId, Guid userId);
        Task<Response<bool>> ToggleProjectStatusAsync(Guid projectId, Guid userId);
    }
}