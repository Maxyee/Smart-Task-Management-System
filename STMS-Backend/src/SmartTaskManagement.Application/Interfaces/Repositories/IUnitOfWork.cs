using SmartTaskManagement.Application.Interfaces.Repositories.Chat;

namespace SmartTaskManagement.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }

        // Chat Repositories (New)
        IConversationRepository Conversations { get; }
        IMessageRepository Messages { get; }
        IConversationParticipantRepository ConversationParticipants { get; }
        IMessageAttachmentRepository MessageAttachments { get; }

        
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}