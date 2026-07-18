using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories.Chat;
using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories.Chat
{
    public class ConversationParticipantRepository : GenericRepository<ConversationParticipant>, IConversationParticipantRepository
    {
        public ConversationParticipantRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(Guid conversationId)
        {
            return await _dbSet
                .Where(p => p.ConversationId == conversationId && p.LeftAt == null)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId &&
                                          p.UserId == userId &&
                                          p.LeftAt == null);
        }

        public async Task<IEnumerable<ConversationParticipant>> GetUserParticipationsAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId && p.LeftAt == null)
                .Include(p => p.Conversation)
                .ToListAsync();
        }

        public async Task<bool> IsParticipantAsync(Guid conversationId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(p => p.ConversationId == conversationId &&
                               p.UserId == userId &&
                               p.LeftAt == null);
        }

        public async Task<IEnumerable<ConversationParticipant>> GetConversationAdminsAsync(Guid conversationId)
        {
            return await _dbSet
                .Where(p => p.ConversationId == conversationId &&
                            p.IsAdmin &&
                            p.LeftAt == null)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<int> GetParticipantCountAsync(Guid conversationId)
        {
            return await _dbSet
                .CountAsync(p => p.ConversationId == conversationId && p.LeftAt == null);
        }

        public async Task UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt)
        {
            var participant = await GetParticipantAsync(conversationId, userId);
            if (participant != null)
            {
                participant.LastReadAt = lastReadAt;
                participant.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(participant);
            }
        }

        public async Task<IEnumerable<ConversationParticipant>> GetOnlineParticipantsAsync(Guid conversationId)
        {
            // This would typically check against a signalr connection list
            // We'll implement this later with SignalR integration
            return await _dbSet
                .Where(p => p.ConversationId == conversationId && p.LeftAt == null)
                .Include(p => p.User)
                .ToListAsync();
        }
    }
}