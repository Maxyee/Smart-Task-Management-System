using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Tasks;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<Response<TaskDto>> CreateTaskAsync(CreateTaskDto createDto, Guid userId);
        Task<Response<TaskDto>> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateDto, Guid userId);
        Task<Response<bool>> DeleteTaskAsync(Guid taskId, Guid userId);
        Task<Response<TaskDto>> GetTaskByIdAsync(Guid taskId, Guid userId);
        Task<Response<PagedResponse<TaskDto>>> GetTasksAsync(TaskFilterDto filter, Guid userId);
        Task<Response<TaskDto>> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto statusDto, Guid userId);
        Task<Response<TaskDto>> AssignTaskAsync(Guid taskId, AssignTaskDto assignDto, Guid userId);
        Task<Response<TaskStatisticsDto>> GetTaskStatisticsAsync(Guid? projectId = null, Guid? userId = null);
        Task<Response<IEnumerable<TaskDto>>> GetOverdueTasksAsync(Guid userId);
        Task<Response<IEnumerable<TaskDto>>> GetTasksDueSoonAsync(Guid userId, int daysThreshold = 7);
        Task<Response<bool>> BulkUpdateStatusAsync(List<Guid> taskIds, TaskItemStatus newStatus, Guid userId);
        Task<Response<IEnumerable<TaskActivityDto>>> GetTaskActivityLogAsync(Guid taskId, Guid userId);
    }
}