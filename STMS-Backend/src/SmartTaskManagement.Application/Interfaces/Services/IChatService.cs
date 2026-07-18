using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Chat;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IChatService
    {
        // Conversation Management
        Task<Response<ConversationDto>> CreateConversationAsync(CreateConversationDto dto, Guid userId);
        Task<Response<ConversationDto>> GetConversationByIdAsync(Guid conversationId, Guid userId);
        Task<Response<IEnumerable<ConversationDto>>> GetUserConversationsAsync(Guid userId);
        Task<Response<bool>> AddParticipantAsync(Guid conversationId, Guid userId, Guid currentUserId);
        Task<Response<bool>> RemoveParticipantAsync(Guid conversationId, Guid userId, Guid currentUserId);
        Task<Response<bool>> LeaveConversationAsync(Guid conversationId, Guid userId);

        // Message Management
        Task<Response<MessageDto>> SendMessageAsync(SendMessageDto dto, Guid userId);
        Task<Response<IEnumerable<MessageDto>>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50);
        Task<Response<bool>> MarkMessagesAsReadAsync(Guid conversationId, Guid userId);
        Task<Response<bool>> DeleteMessageAsync(Guid messageId, Guid userId);
        Task<Response<MessageDto>> EditMessageAsync(Guid messageId, string newContent, Guid userId);

        // Presence & Typing
        Task<Response<bool>> SetUserOnlineAsync(Guid userId);
        Task<Response<bool>> SetUserOfflineAsync(Guid userId);
        Task<Response<UserPresenceDto>> GetUserPresenceAsync(Guid userId);
        Task<Response<bool>> SendTypingIndicatorAsync(Guid conversationId, bool isTyping, Guid userId);
    }
}