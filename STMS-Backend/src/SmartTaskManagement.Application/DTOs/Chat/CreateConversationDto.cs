using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Application.DTOs.Chat
{
    public class CreateConversationDto
    {
        public ConversationType Type { get; set; }
        public string? Name { get; set; }
        public List<Guid> ParticipantIds { get; set; } = new();
    }
}