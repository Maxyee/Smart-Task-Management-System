using SmartTaskManagement.Domain.Common;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Domain.Entities.Chat
{
    public class Conversation : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public Guid? LastMessageId { get; set; }

        // Navigation properties
        public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual Message? LastMessage { get; set; }
    }
}