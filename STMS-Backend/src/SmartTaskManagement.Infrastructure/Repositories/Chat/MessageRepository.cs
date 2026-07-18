using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories.Chat;
using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories.Chat
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize)
        {
            return await _dbSet
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid conversationId, Guid userId)
        {
            var participant = await _context.Set<ConversationParticipant>()
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null) return Enumerable.Empty<Message>();

            var lastReadAt = participant.LastReadAt ?? DateTime.MinValue;

            return await _dbSet
                .Where(m => m.ConversationId == conversationId &&
                            m.SentAt > lastReadAt &&
                            m.SenderId != userId &&
                            !m.IsDeleted)
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId)
        {
            var participant = await _context.Set<ConversationParticipant>()
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null) return 0;

            var lastReadAt = participant.LastReadAt ?? DateTime.MinValue;

            return await _dbSet
                .CountAsync(m => m.ConversationId == conversationId &&
                                 m.SentAt > lastReadAt &&
                                 m.SenderId != userId &&
                                 !m.IsDeleted);
        }

        public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId, DateTime readAt)
        {
            var messages = await _dbSet
                .Where(m => m.ConversationId == conversationId &&
                            m.SenderId != userId &&
                            m.ReadAt == null &&
                            !m.IsDeleted)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.ReadAt = readAt;
                message.UpdatedAt = DateTime.UtcNow;
            }

            if (messages.Any())
            {
                _dbSet.UpdateRange(messages);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserAsync(Guid userId, int page, int pageSize)
        {
            return await _dbSet
                .Where(m => m.SenderId == userId && !m.IsDeleted)
                .Include(m => m.Conversation)
                .Include(m => m.Attachments)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesForUserAsync(Guid userId, int count)
        {
            var conversationIds = await _context.Set<ConversationParticipant>()
                .Where(p => p.UserId == userId && p.LeftAt == null)
                .Select(p => p.ConversationId)
                .ToListAsync();

            return await _dbSet
                .Where(m => conversationIds.Contains(m.ConversationId) && !m.IsDeleted)
                .Include(m => m.Sender)
                .Include(m => m.Conversation)
                .OrderByDescending(m => m.SentAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid userId, string searchTerm)
        {
            var conversationIds = await _context.Set<ConversationParticipant>()
                .Where(p => p.UserId == userId && p.LeftAt == null)
                .Select(p => p.ConversationId)
                .ToListAsync();

            return await _dbSet
                .Where(m => conversationIds.Contains(m.ConversationId) &&
                            m.Content.Contains(searchTerm) &&
                            !m.IsDeleted)
                .Include(m => m.Sender)
                .Include(m => m.Conversation)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message?> GetMessageWithAttachmentsAsync(Guid messageId)
        {
            return await _dbSet
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);
        }
    }
}