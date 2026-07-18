using SmartTaskManagement.Domain.Common;

namespace SmartTaskManagement.Domain.Entities.Chat
{
    public class ConversationParticipant : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? LastReadAt { get; set; }

        // Navigation properties
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}