using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Chat;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Entities.Chat;
using SmartTaskManagement.Domain.Enums.Chat;

namespace SmartTaskManagement.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChatService> _logger;
        private static readonly Dictionary<Guid, DateTime> _userPresence = new();

        public ChatService(IUnitOfWork unitOfWork, ILogger<ChatService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Conversation Management

        public async Task<Response<ConversationDto>> CreateConversationAsync(CreateConversationDto dto, Guid userId)
        {
            try
            {
                // Validate user exists
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<ConversationDto>.FailureResponse("User not found", 404);
                }

                // Validate participants
                var participantUsers = new List<User>();
                foreach (var participantId in dto.ParticipantIds)
                {
                    var participant = await _unitOfWork.Users.GetByIdAsync(participantId);
                    if (participant == null)
                    {
                        return Response<ConversationDto>.FailureResponse($"User {participantId} not found", 404);
                    }
                    participantUsers.Add(participant);
                }

                // For direct conversations, check if one already exists
                if (dto.Type == ConversationType.Direct && dto.ParticipantIds.Count == 1)
                {
                    var existing = await _unitOfWork.Conversations.GetDirectConversationAsync(userId, dto.ParticipantIds[0]);
                    if (existing != null)
                    {
                        var existingDto = await MapToConversationDto(existing, userId);
                        return Response<ConversationDto>.SuccessResponse(existingDto, "Existing conversation found");
                    }
                }

                // Create conversation
                var conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = dto.Type,
                    Name = dto.Type == ConversationType.Direct
                        ? string.Join(", ", participantUsers.Select(u => u.Username))
                        : dto.Name ?? "Group Chat",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.Conversations.AddAsync(conversation);

                // CRITICAL: Add the creator as a participant FIRST
                var creatorParticipant = new ConversationParticipant
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    UserId = userId,
                    IsAdmin = true,
                    JoinedAt = DateTime.UtcNow,
                    LastReadAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.ConversationParticipants.AddAsync(creatorParticipant);

                // Add other participants
                foreach (var participantId in dto.ParticipantIds)
                {
                    // Skip if the participant is the creator (already added)
                    if (participantId == userId) continue;

                    var participant = new ConversationParticipant
                    {
                        Id = Guid.NewGuid(),
                        ConversationId = conversation.Id,
                        UserId = participantId,
                        IsAdmin = false,
                        JoinedAt = DateTime.UtcNow,
                        LastReadAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _unitOfWork.ConversationParticipants.AddAsync(participant);
                }

                // Save all changes
                await _unitOfWork.CompleteAsync();

                // Get the complete conversation with all navigation properties
                var createdConversation = await _unitOfWork.Conversations
                    .GetConversationWithDetailsAsync(conversation.Id);

                var resultDto = await MapToConversationDto(createdConversation!, userId);

                _logger.LogInformation("Conversation {ConversationId} created by user {UserId} with {ParticipantCount} participants",
                    conversation.Id, userId, dto.ParticipantIds.Count + 1);

                return Response<ConversationDto>.SuccessResponse(resultDto, "Conversation created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating conversation by user {UserId}", userId);
                return Response<ConversationDto>.FailureResponse("An error occurred while creating the conversation", 500);
            }
        }

        public async Task<Response<ConversationDto>> GetConversationByIdAsync(Guid conversationId, Guid userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations
                    .GetConversationWithDetailsAsync(conversationId);

                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<ConversationDto>.FailureResponse("Conversation not found", 404);
                }

                // Check if user is a participant - THIS IS CRITICAL
                var isParticipant = await _unitOfWork.ConversationParticipants
                    .IsParticipantAsync(conversationId, userId);

                if (!isParticipant)
                {
                    return Response<ConversationDto>.FailureResponse("You are not a participant in this conversation", 403);
                }

                var conversationDto = await MapToConversationDto(conversation, userId);
                return Response<ConversationDto>.SuccessResponse(conversationDto, "Conversation retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation {ConversationId}", conversationId);
                return Response<ConversationDto>.FailureResponse("An error occurred while retrieving the conversation", 500);
            }
        }

        public async Task<Response<IEnumerable<ConversationDto>>> GetUserConversationsAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<ConversationDto>>.FailureResponse("User not found", 404);
                }

                var conversations = await _unitOfWork.Conversations.GetUserConversationsAsync(userId);
                var conversationDtos = new List<ConversationDto>();

                foreach (var conversation in conversations)
                {
                    var dto = await MapToConversationDto(conversation, userId);
                    conversationDtos.Add(dto);
                }

                return Response<IEnumerable<ConversationDto>>.SuccessResponse(conversationDtos, "Conversations retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations for user {UserId}", userId);
                return Response<IEnumerable<ConversationDto>>.FailureResponse("An error occurred while retrieving conversations", 500);
            }
        }

        public async Task<Response<bool>> AddParticipantAsync(Guid conversationId, Guid userId, Guid currentUserId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Conversation not found", 404);
                }

                // Check if current user is an admin or owner
                var currentParticipant = await _unitOfWork.ConversationParticipants.GetParticipantAsync(conversationId, currentUserId);
                if (currentParticipant == null || !currentParticipant.IsAdmin)
                {
                    return Response<bool>.FailureResponse("You don't have permission to add participants", 403);
                }

                // Check if user to add exists
                var userToAdd = await _unitOfWork.Users.GetByIdAsync(userId);
                if (userToAdd == null || userToAdd.IsDeleted)
                {
                    return Response<bool>.FailureResponse("User to add not found", 404);
                }

                // Check if user is already a participant
                var existingParticipant = await _unitOfWork.ConversationParticipants.GetParticipantAsync(conversationId, userId);
                if (existingParticipant != null && existingParticipant.LeftAt == null)
                {
                    return Response<bool>.FailureResponse("User is already a participant", 409);
                }

                // Add participant
                var participant = new ConversationParticipant
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversationId,
                    UserId = userId,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow,
                    LastReadAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ConversationParticipants.AddAsync(participant);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("User {UserId} added to conversation {ConversationId} by {CurrentUserId}",
                    userId, conversationId, currentUserId);

                return Response<bool>.SuccessResponse(true, "Participant added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding participant to conversation {ConversationId}", conversationId);
                return Response<bool>.FailureResponse("An error occurred while adding participant", 500);
            }
        }

        public async Task<Response<bool>> RemoveParticipantAsync(Guid conversationId, Guid userId, Guid currentUserId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Conversation not found", 404);
                }

                // Check if current user is an admin or owner
                var currentParticipant = await _unitOfWork.ConversationParticipants.GetParticipantAsync(conversationId, currentUserId);
                if (currentParticipant == null || !currentParticipant.IsAdmin)
                {
                    return Response<bool>.FailureResponse("You don't have permission to remove participants", 403);
                }

                // Can't remove yourself
                if (userId == currentUserId)
                {
                    return Response<bool>.FailureResponse("You cannot remove yourself. Use leave conversation instead.", 400);
                }

                // Check if user to remove is a participant
                var participantToRemove = await _unitOfWork.ConversationParticipants.GetParticipantAsync(conversationId, userId);
                if (participantToRemove == null || participantToRemove.LeftAt != null)
                {
                    return Response<bool>.FailureResponse("User is not a participant", 404);
                }

                // Remove participant (soft delete)
                participantToRemove.LeftAt = DateTime.UtcNow;
                participantToRemove.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.ConversationParticipants.Update(participantToRemove);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("User {UserId} removed from conversation {ConversationId} by {CurrentUserId}",
                    userId, conversationId, currentUserId);

                return Response<bool>.SuccessResponse(true, "Participant removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing participant from conversation {ConversationId}", conversationId);
                return Response<bool>.FailureResponse("An error occurred while removing participant", 500);
            }
        }

        public async Task<Response<bool>> LeaveConversationAsync(Guid conversationId, Guid userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Conversation not found", 404);
                }

                // Check if user is a participant
                var participant = await _unitOfWork.ConversationParticipants.GetParticipantAsync(conversationId, userId);
                if (participant == null || participant.LeftAt != null)
                {
                    return Response<bool>.FailureResponse("You are not a participant in this conversation", 404);
                }

                // Leave conversation
                participant.LeftAt = DateTime.UtcNow;
                participant.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.ConversationParticipants.Update(participant);
                await _unitOfWork.CompleteAsync();

                // If no participants left, delete conversation
                var participantCount = await _unitOfWork.ConversationParticipants.GetParticipantCountAsync(conversationId);
                if (participantCount == 0)
                {
                    conversation.IsDeleted = true;
                    conversation.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Conversations.Update(conversation);
                    await _unitOfWork.CompleteAsync();
                }

                _logger.LogInformation("User {UserId} left conversation {ConversationId}", userId, conversationId);

                return Response<bool>.SuccessResponse(true, "Left conversation successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving conversation {ConversationId}", conversationId);
                return Response<bool>.FailureResponse("An error occurred while leaving conversation", 500);
            }
        }

        #endregion

        #region Message Management

        public async Task<Response<MessageDto>> SendMessageAsync(SendMessageDto dto, Guid userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(dto.ConversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<MessageDto>.FailureResponse("Conversation not found", 404);
                }

                // Check if user is a participant
                var participant = await _unitOfWork.ConversationParticipants.GetParticipantAsync(dto.ConversationId, userId);
                if (participant == null || participant.LeftAt != null)
                {
                    return Response<MessageDto>.FailureResponse("You are not a participant in this conversation", 403);
                }

                // Check if replying to a message
                if (dto.ReplyToMessageId.HasValue)
                {
                    var replyToMessage = await _unitOfWork.Messages.GetByIdAsync(dto.ReplyToMessageId.Value);
                    if (replyToMessage == null || replyToMessage.IsDeleted)
                    {
                        return Response<MessageDto>.FailureResponse("Message to reply to not found", 404);
                    }
                    if (replyToMessage.ConversationId != dto.ConversationId)
                    {
                        return Response<MessageDto>.FailureResponse("Message to reply to is not in this conversation", 400);
                    }
                }

                // Create message
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    ConversationId = dto.ConversationId,
                    SenderId = userId,
                    Content = dto.Content,
                    Type = dto.Type,
                    SentAt = DateTime.UtcNow,
                    ReplyToMessageId = dto.ReplyToMessageId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Messages.AddAsync(message);

                // Update conversation last message
                conversation.LastMessageId = message.Id;
                conversation.LastMessageAt = message.SentAt;
                conversation.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                var messageDto = await MapToMessageDto(message, userId);

                _logger.LogInformation("Message sent in conversation {ConversationId} by user {UserId}",
                    dto.ConversationId, userId);

                return Response<MessageDto>.SuccessResponse(messageDto, "Message sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return Response<MessageDto>.FailureResponse("An error occurred while sending the message", 500);
            }
        }

        public async Task<Response<IEnumerable<MessageDto>>> GetConversationMessagesAsync(
            Guid conversationId, Guid userId, int page = 1, int pageSize = 50)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<IEnumerable<MessageDto>>.FailureResponse("Conversation not found", 404);
                }

                // Check if user is a participant
                var isParticipant = await _unitOfWork.ConversationParticipants.IsParticipantAsync(conversationId, userId);
                if (!isParticipant)
                {
                    return Response<IEnumerable<MessageDto>>.FailureResponse(
                        "You are not a participant in this conversation", 403);
                }

                var messages = await _unitOfWork.Messages.GetConversationMessagesAsync(conversationId, page, pageSize);
                var messageDtos = new List<MessageDto>();

                foreach (var message in messages)
                {
                    var dto = await MapToMessageDto(message, userId);
                    messageDtos.Add(dto);
                }

                return Response<IEnumerable<MessageDto>>.SuccessResponse(messageDtos, "Messages retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages for conversation {ConversationId}", conversationId);
                return Response<IEnumerable<MessageDto>>.FailureResponse(
                    "An error occurred while retrieving messages", 500);
            }
        }

        public async Task<Response<bool>> MarkMessagesAsReadAsync(Guid conversationId, Guid userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Conversation not found", 404);
                }

                // Check if user is a participant
                var participant = await _unitOfWork.ConversationParticipants.GetParticipantAsync(conversationId, userId);
                if (participant == null || participant.LeftAt != null)
                {
                    return Response<bool>.FailureResponse("You are not a participant in this conversation", 403);
                }

                // Mark messages as read
                var readAt = DateTime.UtcNow;
                await _unitOfWork.Messages.MarkMessagesAsReadAsync(conversationId, userId, readAt);

                // Update participant's last read
                participant.LastReadAt = readAt;
                participant.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ConversationParticipants.Update(participant);

                await _unitOfWork.CompleteAsync();

                return Response<bool>.SuccessResponse(true, "Messages marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking messages as read in conversation {ConversationId}", conversationId);
                return Response<bool>.FailureResponse("An error occurred while marking messages as read", 500);
            }
        }

        public async Task<Response<bool>> DeleteMessageAsync(Guid messageId, Guid userId)
        {
            try
            {
                var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
                if (message == null || message.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Message not found", 404);
                }

                // Check if user is the sender or an admin of the conversation
                var isSender = message.SenderId == userId;
                if (!isSender)
                {
                    // Check if user is admin of the conversation
                    var participant = await _unitOfWork.ConversationParticipants
                        .GetParticipantAsync(message.ConversationId, userId);
                    if (participant == null || !participant.IsAdmin)
                    {
                        return Response<bool>.FailureResponse(
                            "You don't have permission to delete this message", 403);
                    }
                }

                // Soft delete message
                message.IsDeleted = true;
                message.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Messages.Update(message);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Message {MessageId} deleted by user {UserId}", messageId, userId);

                return Response<bool>.SuccessResponse(true, "Message deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
                return Response<bool>.FailureResponse("An error occurred while deleting the message", 500);
            }
        }

        public async Task<Response<MessageDto>> EditMessageAsync(Guid messageId, string newContent, Guid userId)
        {
            try
            {
                var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
                if (message == null || message.IsDeleted)
                {
                    return Response<MessageDto>.FailureResponse("Message not found", 404);
                }

                // Check if user is the sender
                if (message.SenderId != userId)
                {
                    return Response<MessageDto>.FailureResponse(
                        "You can only edit your own messages", 403);
                }

                // Update message
                message.Content = newContent;
                message.EditedAt = DateTime.UtcNow;
                message.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Messages.Update(message);
                await _unitOfWork.CompleteAsync();

                var messageDto = await MapToMessageDto(message, userId);

                _logger.LogInformation("Message {MessageId} edited by user {UserId}", messageId, userId);

                return Response<MessageDto>.SuccessResponse(messageDto, "Message edited successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing message {MessageId}", messageId);
                return Response<MessageDto>.FailureResponse("An error occurred while editing the message", 500);
            }
        }

        #endregion

        #region Presence & Typing

        public async Task<Response<bool>> SetUserOnlineAsync(Guid userId)
        {
            try
            {
                _userPresence[userId] = DateTime.UtcNow;

                // Update user's last seen
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user != null)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.CompleteAsync();
                }

                return Response<bool>.SuccessResponse(true, "User set online");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting user {UserId} online", userId);
                return Response<bool>.FailureResponse("An error occurred while setting user online", 500);
            }
        }

        public async Task<Response<bool>> SetUserOfflineAsync(Guid userId)
        {
            try
            {
                _userPresence.Remove(userId);
                return Response<bool>.SuccessResponse(true, "User set offline");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting user {UserId} offline", userId);
                return Response<bool>.FailureResponse("An error occurred while setting user offline", 500);
            }
        }

        public async Task<Response<UserPresenceDto>> GetUserPresenceAsync(Guid userId)
        {
            try
            {
                var isOnline = _userPresence.ContainsKey(userId);
                var lastSeen = isOnline ? _userPresence[userId] : await GetLastSeenAsync(userId);

                return Response<UserPresenceDto>.SuccessResponse(new UserPresenceDto
                {
                    UserId = userId,
                    IsOnline = isOnline,
                    LastSeenAt = lastSeen
                }, "Presence retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting presence for user {UserId}", userId);
                return Response<UserPresenceDto>.FailureResponse(
                    "An error occurred while retrieving user presence", 500);
            }
        }

        public async Task<Response<bool>> SendTypingIndicatorAsync(Guid conversationId, bool isTyping, Guid userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
                if (conversation == null || conversation.IsDeleted)
                {
                    return Response<bool>.FailureResponse("Conversation not found", 404);
                }

                // Check if user is a participant
                var isParticipant = await _unitOfWork.ConversationParticipants.IsParticipantAsync(conversationId, userId);
                if (!isParticipant)
                {
                    return Response<bool>.FailureResponse("You are not a participant in this conversation", 403);
                }

                // Typing indicator doesn't need to be stored in database
                // It's handled through SignalR

                return Response<bool>.SuccessResponse(true, "Typing indicator sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending typing indicator");
                return Response<bool>.FailureResponse("An error occurred while sending typing indicator", 500);
            }
        }

        #endregion

        #region Private Methods

        private async Task<DateTime?> GetLastSeenAsync(Guid userId)
        {
            try
            {
                // Get last message sent by user to determine last seen
                var lastMessage = await _unitOfWork.Messages
                    .FindAsync(m => m.SenderId == userId)
                    .ContinueWith(t => t.Result.OrderByDescending(m => m.SentAt).FirstOrDefault());

                return lastMessage?.SentAt;
            }
            catch
            {
                return null;
            }
        }

        private async Task<ConversationDto> MapToConversationDto(Conversation conversation, Guid userId)
        {
            var unreadCount = await _unitOfWork.Conversations.GetUnreadCountAsync(conversation.Id, userId);
            var participants = await _unitOfWork.ConversationParticipants
                .GetConversationParticipantsAsync(conversation.Id);

            var participantDtos = new List<ParticipantDto>();
            foreach (var p in participants)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(p.UserId);
                if (user != null)
                {
                    var isOnline = _userPresence.ContainsKey(user.Id);
                    participantDtos.Add(new ParticipantDto
                    {
                        UserId = user.Id,
                        UserName = user.Username,
                        FullName = $"{user.FirstName} {user.LastName}",
                        AvatarUrl = "", // You can add avatar URL here
                        IsAdmin = p.IsAdmin,
                        IsOnline = isOnline,
                        LastSeenAt = isOnline ? _userPresence[user.Id] : null
                    });
                }
            }

            MessageDto? lastMessageDto = null;
            if (conversation.LastMessageId.HasValue)
            {
                var lastMessage = await _unitOfWork.Messages.GetByIdAsync(conversation.LastMessageId.Value);
                if (lastMessage != null && !lastMessage.IsDeleted)
                {
                    lastMessageDto = await MapToMessageDto(lastMessage, userId);
                }
            }

            return new ConversationDto
            {
                Id = conversation.Id,
                Name = conversation.Name,
                Type = conversation.Type,
                AvatarUrl = conversation.AvatarUrl,
                LastMessageAt = conversation.LastMessageAt,
                LastMessage = lastMessageDto,
                UnreadCount = unreadCount,
                Participants = participantDtos
            };
        }

        private async Task<MessageDto> MapToMessageDto(Message message, Guid userId)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId);
            var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown";

            // Check if message is read
            var isRead = message.ReadAt.HasValue;

            // Check if it's the user's own message
            var isOwn = message.SenderId == userId;

            // Get attachments
            var attachments = new List<MessageAttachmentDto>();
            var attachmentsList = await _unitOfWork.MessageAttachments.GetMessageAttachmentsAsync(message.Id);
            foreach (var attachment in attachmentsList)
            {
                attachments.Add(new MessageAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    FileUrl = attachment.FilePath,
                    FileType = attachment.FileType,
                    FileSize = attachment.FileSize
                });
            }

            return new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = senderName,
                SenderAvatar = "", // You can add avatar URL here
                Content = message.Content,
                Type = message.Type,
                SentAt = message.SentAt,
                ReadAt = message.ReadAt,
                IsOwn = isOwn,
                IsRead = isRead,
                Attachments = attachments.Any() ? attachments : null
            };
        }

        #endregion
    }
}