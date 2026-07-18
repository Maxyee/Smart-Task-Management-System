using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Application.DTOs.Chat
{
    public class TypingIndicatorDto
    {
        public Guid ConversationId { get; set; }
        public bool IsTyping { get; set; }
    }
}