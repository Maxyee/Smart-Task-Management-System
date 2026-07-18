using SmartTaskManagement.Domain.Entities.Chat;

namespace SmartTaskManagement.Application.Interfaces.Repositories.Chat
{
    public interface IConversationParticipantRepository : IGenericRepository<ConversationParticipant>
    {
        /// <summary>
        /// Get all participants for a conversation
        /// </summary>
        Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(Guid conversationId);

        /// <summary>
        /// Get participant by conversation and user
        /// </summary>
        Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId);

        /// <summary>
        /// Get all conversations a user is participating in
        /// </summary>
        Task<IEnumerable<ConversationParticipant>> GetUserParticipationsAsync(Guid userId);

        /// <summary>
        /// Check if user is a participant in a conversation
        /// </summary>
        Task<bool> IsParticipantAsync(Guid conversationId, Guid userId);

        /// <summary>
        /// Get admins for a conversation
        /// </summary>
        Task<IEnumerable<ConversationParticipant>> GetConversationAdminsAsync(Guid conversationId);

        /// <summary>
        /// Get participant count for a conversation
        /// </summary>
        Task<int> GetParticipantCountAsync(Guid conversationId);

        /// <summary>
        /// Update last read timestamp for a participant
        /// </summary>
        Task UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt);

        /// <summary>
        /// Get online participants in a conversation
        /// </summary>
        Task<IEnumerable<ConversationParticipant>> GetOnlineParticipantsAsync(Guid conversationId);
    }

}