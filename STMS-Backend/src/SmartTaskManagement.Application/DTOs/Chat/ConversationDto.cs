using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ConversationType Type { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public MessageDto? LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public List<ParticipantDto> Participants { get; set; } = new();
    }
}