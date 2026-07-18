using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartTaskManagement.Application.DTOs.Chat;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;


namespace SmartTaskManagement.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;
        private static readonly Dictionary<Guid, string> _userConnections = new();

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("User connected with invalid ID");
                    await base.OnConnectedAsync();
                    return;
                }

                // Store connection
                _userConnections[userId] = Context.ConnectionId;

                // Update user status
                await _chatService.SetUserOnlineAsync(userId);

                // Notify other users
                await Clients.All.SendAsync("UserOnline", userId.ToString());

                await base.OnConnectedAsync();

                _logger.LogInformation("User {UserId} connected with connection {ConnectionId}",
                    userId, Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    await base.OnDisconnectedAsync(exception);
                    return;
                }

                // Remove connection
                _userConnections.Remove(userId);

                // Update user status
                await _chatService.SetUserOfflineAsync(userId);

                // Notify other users
                await Clients.All.SendAsync("UserOffline", userId.ToString());

                await base.OnDisconnectedAsync(exception);

                _logger.LogInformation("User {UserId} disconnected. Exception: {Exception}",
                    userId, exception?.Message ?? "None");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
                throw;
            }
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to send message with invalid user ID");
                    return;
                }

                var result = await _chatService.SendMessageAsync(dto, userId);

                if (result.Success && result.Data != null)
                {
                    var message = result.Data;
                    var conversationId = dto.ConversationId.ToString();

                    // Send to all participants in the conversation
                    await Clients.Group(conversationId).SendAsync("ReceiveMessage", message);

                    // Update conversation list for participants
                    await Clients.Group(conversationId).SendAsync("ConversationUpdated", conversationId);

                    _logger.LogInformation("Message sent in conversation {ConversationId} by user {UserId}",
                        dto.ConversationId, userId);
                }
                else
                {
                    _logger.LogWarning("Failed to send message: {Message}", result.Message);
                    await Clients.Caller.SendAsync("MessageError", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessage");
                await Clients.Caller.SendAsync("MessageError", "An error occurred while sending the message");
            }
        }

        public async Task JoinConversation(string conversationId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to join conversation with invalid user ID");
                    return;
                }

                if (!Guid.TryParse(conversationId, out var conversationGuid))
                {
                    _logger.LogWarning("Invalid conversation ID format: {ConversationId}", conversationId);
                    await Clients.Caller.SendAsync("Error", "Invalid conversation ID");
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);

                _logger.LogInformation("User {UserId} joined conversation {ConversationId}",
                    userId, conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in JoinConversation");
                await Clients.Caller.SendAsync("Error", "An error occurred while joining the conversation");
            }
        }

        public async Task LeaveConversation(string conversationId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return;
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);

                _logger.LogInformation("User {UserId} left conversation {ConversationId}",
                    userId, conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LeaveConversation");
            }
        }

        public async Task MarkAsRead(MarkAsReadDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to mark as read with invalid user ID");
                    return;
                }

                var result = await _chatService.MarkMessagesAsReadAsync(dto.ConversationId, userId);

                if (result.Success)
                {
                    await Clients.Group(dto.ConversationId.ToString())
                        .SendAsync("MessagesRead", dto.ConversationId.ToString(), userId.ToString());

                    _logger.LogInformation("Messages marked as read in conversation {ConversationId} by user {UserId}",
                        dto.ConversationId, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MarkAsRead");
            }
        }

        public async Task TypingIndicator(TypingIndicatorDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return;
                }

                await _chatService.SendTypingIndicatorAsync(dto.ConversationId, dto.IsTyping, userId);

                await Clients.Group(dto.ConversationId.ToString())
                    .SendAsync("UserTyping", userId.ToString(), dto.IsTyping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TypingIndicator");
            }
        }

        public async Task DeleteMessage(Guid messageId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to delete message with invalid user ID");
                    return;
                }

                var result = await _chatService.DeleteMessageAsync(messageId, userId);

                if (result.Success)
                {
                    await Clients.All.SendAsync("MessageDeleted", messageId.ToString());

                    _logger.LogInformation("Message {MessageId} deleted by user {UserId}",
                        messageId, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteMessage");
            }
        }

        public async Task EditMessage(Guid messageId, string newContent)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to edit message with invalid user ID");
                    return;
                }

                var result = await _chatService.EditMessageAsync(messageId, newContent, userId);

                if (result.Success && result.Data != null)
                {
                    await Clients.All.SendAsync("MessageEdited", result.Data);

                    _logger.LogInformation("Message {MessageId} edited by user {UserId}",
                        messageId, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditMessage");
            }
        }

        public async Task CreateConversation(CreateConversationDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to create conversation with invalid user ID");
                    await Clients.Caller.SendAsync("Error", "Invalid user ID");
                    return;
                }

                var result = await _chatService.CreateConversationAsync(dto, userId);

                if (result.Success && result.Data != null)
                {
                    var conversation = result.Data;

                    // Notify all participants
                    foreach (var participant in conversation.Participants)
                    {
                        if (_userConnections.TryGetValue(participant.UserId, out var connectionId))
                        {
                            await Clients.Client(connectionId).SendAsync("NewConversation", conversation);
                        }
                    }

                    _logger.LogInformation("Conversation {ConversationId} created by user {UserId}",
                        conversation.Id, userId);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConversation");
                await Clients.Caller.SendAsync("Error", "An error occurred while creating the conversation");
            }
        }

        public async Task GetConversations()
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to get conversations with invalid user ID");
                    await Clients.Caller.SendAsync("Error", "Invalid user ID");
                    return;
                }

                var result = await _chatService.GetUserConversationsAsync(userId);

                if (result.Success)
                {
                    await Clients.Caller.SendAsync("ConversationsLoaded", result.Data);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetConversations");
                await Clients.Caller.SendAsync("Error", "An error occurred while loading conversations");
            }
        }

        public async Task GetConversationMessages(Guid conversationId, int page = 1, int pageSize = 50)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Attempt to get messages with invalid user ID");
                    await Clients.Caller.SendAsync("Error", "Invalid user ID");
                    return;
                }

                var result = await _chatService.GetConversationMessagesAsync(conversationId, userId, page, pageSize);

                if (result.Success)
                {
                    await Clients.Caller.SendAsync("MessagesLoaded", conversationId.ToString(), result.Data);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetConversationMessages");
                await Clients.Caller.SendAsync("Error", "An error occurred while loading messages");
            }
        }

        #region Private Methods

        private Guid GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("User ID claim not found in token");
                return Guid.Empty;
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user ID format: {UserIdClaim}", userIdClaim);
                return Guid.Empty;
            }

            return userId;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get all online users
        /// </summary>
        public async Task GetOnlineUsers()
        {
            try
            {
                var onlineUsers = _userConnections.Keys.ToList();
                await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOnlineUsers");
            }
        }

        /// <summary>
        /// Check if a user is online
        /// </summary>
        public bool IsUserOnline(Guid userId)
        {
            return _userConnections.ContainsKey(userId);
        }

        /// <summary>
        /// Get connection ID for a user
        /// </summary>
        public string? GetUserConnectionId(Guid userId)
        {
            return _userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
        }

        #endregion
    }
}