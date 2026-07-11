using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Infrastructure.Data;


namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.FirstOrDefaultAsync(u =>
                u.RefreshToken == refreshToken &&
                !u.IsDeleted &&
                u.RefreshTokenExpiryTime > DateTime.UtcNow);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role.ToString() == role && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Email == email && !u.IsDeleted);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Username == username && !u.IsDeleted);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}