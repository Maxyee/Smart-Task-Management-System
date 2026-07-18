using SmartTaskManagement.Domain.Entities.Chat;

namespace SmartTaskManagement.Application.Interfaces.Repositories.Chat
{
   
    public interface IMessageRepository : IGenericRepository<Message>
    {
        /// <summary>
        /// Get messages for a conversation with pagination
        /// </summary>
        Task<IEnumerable<Message>> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize);

        /// <summary>
        /// Get unread messages for a user in a conversation
        /// </summary>
        Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid conversationId, Guid userId);

        /// <summary>
        /// Get unread count for a user in a conversation
        /// </summary>
        Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId);

        /// <summary>
        /// Mark messages as read for a user in a conversation
        /// </summary>
        Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId, DateTime readAt);

        /// <summary>
        /// Get messages sent by a user
        /// </summary>
        Task<IEnumerable<Message>> GetMessagesByUserAsync(Guid userId, int page, int pageSize);

        /// <summary>
        /// Get recent messages for a user across all conversations
        /// </summary>
        Task<IEnumerable<Message>> GetRecentMessagesForUserAsync(Guid userId, int count);

        /// <summary>
        /// Search messages by content
        /// </summary>
        Task<IEnumerable<Message>> SearchMessagesAsync(Guid userId, string searchTerm);

        /// <summary>
        /// Get message with attachments
        /// </summary>
        Task<Message?> GetMessageWithAttachmentsAsync(Guid messageId);
    }

}