using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Application.DTOs.Chat
{
    public class UserPresenceDto
    {
        public Guid UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}