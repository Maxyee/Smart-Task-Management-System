using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Application.DTOs.Chat
{
    public class MarkAsReadDto
    {
        public Guid ConversationId { get; set; }
        public Guid MessageId { get; set; }
    }
}