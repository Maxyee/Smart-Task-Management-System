using SmartTaskManagement.Domain.Common;

namespace SmartTaskManagement.Domain.Entities.Chat
{
    public class MessageAttachment : BaseEntity
    {
        public Guid MessageId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        // Navigation properties
        public virtual Message Message { get; set; } = null!;
    }
}