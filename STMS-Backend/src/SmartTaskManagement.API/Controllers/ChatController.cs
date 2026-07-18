using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Chat;
using SmartTaskManagement.Application.Interfaces.Services;
using System.Security.Claims;

namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// Get all conversations for the current user
    /// </summary>
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = GetUserId();
        var result = await _chatService.GetUserConversationsAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Get a specific conversation by ID
    /// </summary>
    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetConversation(Guid id)
    {
        var userId = GetUserId();
        var result = await _chatService.GetConversationByIdAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var userId = GetUserId();
        var result = await _chatService.CreateConversationAsync(dto, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Get messages for a conversation
    /// </summary>
    [HttpGet("conversations/{id}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = GetUserId();
        var result = await _chatService.GetConversationMessagesAsync(id, userId, page, pageSize);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Send a message to a conversation
    /// </summary>
    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageDto dto)
    {
        if (id != dto.ConversationId)
        {
            return BadRequest(Response<MessageDto>.FailureResponse("Conversation ID mismatch", 400));
        }

        var userId = GetUserId();
        var result = await _chatService.SendMessageAsync(dto, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Add a participant to a conversation
    /// </summary>
    [HttpPost("conversations/{id}/participants")]
    public async Task<IActionResult> AddParticipant(Guid id, [FromBody] ParticipantDto dto)
    {
        var userId = GetUserId();
        var result = await _chatService.AddParticipantAsync(id, dto.UserId, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Remove a participant from a conversation
    /// </summary>
    [HttpDelete("conversations/{id}/participants/{participantId}")]
    public async Task<IActionResult> RemoveParticipant(Guid id, Guid participantId)
    {
        var userId = GetUserId();
        var result = await _chatService.RemoveParticipantAsync(id, participantId, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Leave a conversation
    /// </summary>
    [HttpPost("conversations/{id}/leave")]
    public async Task<IActionResult> LeaveConversation(Guid id)
    {
        var userId = GetUserId();
        var result = await _chatService.LeaveConversationAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Delete a message
    /// </summary>
    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var userId = GetUserId();
        var result = await _chatService.DeleteMessageAsync(messageId, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Edit a message
    /// </summary>
    [HttpPut("messages/{messageId}")]
    public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] string content)
    {
        var userId = GetUserId();
        var result = await _chatService.EditMessageAsync(messageId, content, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Mark messages as read in a conversation
    /// </summary>
    [HttpPost("conversations/{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = GetUserId();
        var result = await _chatService.MarkMessagesAsReadAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Get user presence (online/offline status)
    /// </summary>
    [HttpGet("users/{userId}/presence")]
    public async Task<IActionResult> GetUserPresence(Guid userId)
    {
        var result = await _chatService.GetUserPresenceAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    #region Private Methods

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return Guid.Parse(userIdClaim);
    }

    #endregion
}
}