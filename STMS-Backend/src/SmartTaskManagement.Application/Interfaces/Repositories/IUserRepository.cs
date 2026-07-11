using SmartTaskManagement.Domain.Entities;

namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
        Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null);
    }
}