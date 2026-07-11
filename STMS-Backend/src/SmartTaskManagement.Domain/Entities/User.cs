using SmartTaskManagement.Domain.Common;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}