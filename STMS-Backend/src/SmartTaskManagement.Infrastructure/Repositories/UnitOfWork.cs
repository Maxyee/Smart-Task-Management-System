using Microsoft.EntityFrameworkCore.Storage;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Repositories.Chat;
using SmartTaskManagement.Infrastructure.Data;

namespace SmartTaskManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository,
            IConversationParticipantRepository conversationParticipantRepository,
            IMessageAttachmentRepository messageAttachmentRepository)
        {
            _context = context;
            Users = userRepository;
            Projects = projectRepository;
            Tasks = taskRepository;
            Conversations = conversationRepository;
            Messages = messageRepository;
            ConversationParticipants = conversationParticipantRepository;
            MessageAttachments = messageAttachmentRepository;
        }

        public IUserRepository Users { get; }
        public IProjectRepository Projects { get; }
        public ITaskRepository Tasks { get; }

        // Chat Repositories
        public IConversationRepository Conversations { get; }
        public IMessageRepository Messages { get; }
        public IConversationParticipantRepository ConversationParticipants { get; }
        public IMessageAttachmentRepository MessageAttachments { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
}