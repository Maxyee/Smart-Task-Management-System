using SmartTaskManagement.Domain.Entities.Chat;

namespace SmartTaskManagement.Application.Interfaces.Repositories.Chat
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        /// <summary>
        /// Get conversation with all details (participants, last message, etc.)
        /// </summary>
        Task<Conversation?> GetConversationWithDetailsAsync(Guid conversationId);

        /// <summary>
        /// Get all conversations for a specific user
        /// </summary>
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId);

        /// <summary>
        /// Get direct conversation between two users (if exists)
        /// </summary>
        Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2);

        /// <summary>
        /// Get all conversations where user is a participant
        /// </summary>
        Task<IEnumerable<Conversation>> GetUserParticipatedConversationsAsync(Guid userId);

        /// <summary>
        /// Get conversations with unread messages for a user
        /// </summary>
        Task<IEnumerable<Conversation>> GetConversationsWithUnreadMessagesAsync(Guid userId);

        /// <summary>
        /// Update conversation last message
        /// </summary>
        Task UpdateLastMessageAsync(Guid conversationId, Guid messageId, DateTime lastMessageAt);

        /// <summary>
        /// Get unread count for a specific conversation
        /// </summary>
        Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId);
    }

}