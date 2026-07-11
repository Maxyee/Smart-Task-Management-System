using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.CreatedByUserId == userId && !p.IsDeleted)
                .Include(p => p.CreatedByUser)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive && !p.IsDeleted)
                .Include(p => p.CreatedByUser)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted &&
                    (p.Name.Contains(searchTerm) ||
                     p.Description.Contains(searchTerm)))
                .Include(p => p.CreatedByUser)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectWithTasksAsync(Guid projectId)
        {
            return await _dbSet
                .Include(p => p.Tasks.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.AssignedToUser)
                .Include(p => p.Tasks.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.CreatedByUser)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);
        }

        public async Task<int> GetProjectCountByUserAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(p => p.CreatedByUserId == userId && !p.IsDeleted);
        }
    }
}