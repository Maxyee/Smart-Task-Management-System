using SmartTaskManagement.Domain.Common;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Domain.Entities.Chat
{
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public Guid? ReplyToMessageId { get; set; }

        // Navigation properties
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;
        public virtual Message? ReplyToMessage { get; set; }
        public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
    }
}