using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Application.DTOs.Chat
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsOwn { get; set; }
        public bool IsRead { get; set; }
        public List<MessageAttachmentDto>? Attachments { get; set; }
    }
}