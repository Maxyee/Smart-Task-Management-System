using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Repositories.Chat;
using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTaskManagement.Infrastructure.Repositories.Chat
{
    public class MessageAttachmentRepository : GenericRepository<MessageAttachment>, IMessageAttachmentRepository
    {
        public MessageAttachmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MessageAttachment>> GetMessageAttachmentsAsync(Guid messageId)
        {
            return await _dbSet
                .Where(a => a.MessageId == messageId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageAttachment>> GetMessageAttachmentsAsync(IEnumerable<Guid> messageIds)
        {
            return await _dbSet
                .Where(a => messageIds.Contains(a.MessageId))
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageAttachment>> GetConversationAttachmentsAsync(Guid conversationId)
        {
            var messageIds = await _context.Set<Message>()
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .Select(m => m.Id)
                .ToListAsync();

            return await _dbSet
                .Where(a => messageIds.Contains(a.MessageId))
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageAttachment>> GetAttachmentsByTypeAsync(string fileType)
        {
            return await _dbSet
                .Where(a => a.FileType == fileType)
                .ToListAsync();
        }

        public async Task<long> GetConversationAttachmentsSizeAsync(Guid conversationId)
        {
            var messageIds = await _context.Set<Message>()
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .Select(m => m.Id)
                .ToListAsync();

            return await _dbSet
                .Where(a => messageIds.Contains(a.MessageId))
                .SumAsync(a => a.FileSize);
        }
    }
}