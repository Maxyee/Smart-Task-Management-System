using SmartTaskManagement.Domain.Entities;


namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsByUserAsync(Guid userId);
    Task<IEnumerable<Project>> GetActiveProjectsAsync();
    Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm);
    Task<Project?> GetProjectWithTasksAsync(Guid projectId);
    Task<int> GetProjectCountByUserAsync(Guid userId);
}
}