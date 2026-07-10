using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId);
        Task<IEnumerable<TaskItem>> GetTasksByUserAsync(Guid userId);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskItemStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskItemPriority priority);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> SearchTasksAsync(string searchTerm);
        Task<Dictionary<TaskItemStatus, int>> GetTaskStatusCountsAsync(Guid? projectId = null);
        Task<Dictionary<TaskItemPriority, int>> GetTaskPriorityCountsAsync(Guid? projectId = null);
    }
}