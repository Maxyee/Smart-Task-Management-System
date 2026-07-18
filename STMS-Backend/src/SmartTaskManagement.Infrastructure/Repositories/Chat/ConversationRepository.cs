using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories.Chat;
using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories.Chat
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Conversation?> GetConversationWithDetailsAsync(Guid conversationId)
        {
            return await _dbSet
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Sender)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Attachments)
                .FirstOrDefaultAsync(c => c.Id == conversationId && !c.IsDeleted);
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted &&
                    c.Participants.Any(p => p.UserId == userId && p.LeftAt == null))
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                    .ThenInclude(m => m != null ? m.Sender : null!) // Conditional include
                .OrderByDescending(c => c.LastMessageAt ?? c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2)
        {
            var user1Conversations = await GetUserParticipatedConversationsAsync(userId1);
            var user2Conversations = await GetUserParticipatedConversationsAsync(userId2);

            var directConversation = user1Conversations
                .Intersect(user2Conversations)
                .FirstOrDefault(c => c.Type == ConversationType.Direct);

            return directConversation;
        }

        public async Task<IEnumerable<Conversation>> GetUserParticipatedConversationsAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted && c.Participants.Any(p => p.UserId == userId && p.LeftAt == null))
                .ToListAsync();
        }

        public async Task<IEnumerable<Conversation>> GetConversationsWithUnreadMessagesAsync(Guid userId)
        {
            var conversations = await GetUserConversationsAsync(userId);
            var result = new List<Conversation>();

            foreach (var conversation in conversations)
            {
                var unreadCount = await GetUnreadCountAsync(conversation.Id, userId);
                if (unreadCount > 0)
                {
                    result.Add(conversation);
                }
            }

            return result;
        }

        public async Task UpdateLastMessageAsync(Guid conversationId, Guid messageId, DateTime lastMessageAt)
        {
            var conversation = await _dbSet.FindAsync(conversationId);
            if (conversation != null)
            {
                conversation.LastMessageId = messageId;
                conversation.LastMessageAt = lastMessageAt;
                conversation.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(conversation);
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId)
        {
            var participant = await _context.Set<ConversationParticipant>()
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null) return 0;

            var lastReadAt = participant.LastReadAt ?? DateTime.MinValue;

            return await _context.Set<Message>()
                .CountAsync(m => m.ConversationId == conversationId &&
                                 m.SentAt > lastReadAt &&
                                 m.SenderId != userId);
        }
    }
}