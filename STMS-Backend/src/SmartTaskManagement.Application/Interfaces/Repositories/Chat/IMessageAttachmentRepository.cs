using SmartTaskManagement.Domain.Entities.Chat;

namespace SmartTaskManagement.Application.Interfaces.Repositories.Chat
{
    public interface IMessageAttachmentRepository : IGenericRepository<MessageAttachment>
    {
        /// <summary>
        /// Get attachments for a message
        /// </summary>
        Task<IEnumerable<MessageAttachment>> GetMessageAttachmentsAsync(Guid messageId);

        /// <summary>
        /// Get attachments for multiple messages
        /// </summary>
        Task<IEnumerable<MessageAttachment>> GetMessageAttachmentsAsync(IEnumerable<Guid> messageIds);

        /// <summary>
        /// Get attachments for a conversation
        /// </summary>
        Task<IEnumerable<MessageAttachment>> GetConversationAttachmentsAsync(Guid conversationId);

        /// <summary>
        /// Get attachments by file type
        /// </summary>
        Task<IEnumerable<MessageAttachment>> GetAttachmentsByTypeAsync(string fileType);

        /// <summary>
        /// Get total size of attachments for a conversation
        /// </summary>
        Task<long> GetConversationAttachmentsSizeAsync(Guid conversationId);
    }


}