using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.Application.Interfaces.Driven;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Conversations;
using Threddit.Contracts.Responses.Conversations;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/conversations")]
[Authorize(Policy = "NotBanned")]
public class ConversationController : ControllerBase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public ConversationController(
        IConversationRepository conversationRepository,
        IUserRepository userRepository
    )
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType(typeof(GetConversationsApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetConversationsApiResponse>> GetAll(
        CancellationToken ct = default)
    {
        var userId = CurrentUserId;

        var conversationsResult = await _conversationRepository.GetConversationsForUserAsync(userId);
        var groupsResult = await _conversationRepository.GetGroupConversationsForUserAsync(userId);

        var conversationDtos = (conversationsResult.Value ?? []).Select(c =>
        {
            var isInitiator = c.InitiatorId == userId;
            var other = isInitiator ? c.Recipient : c.Initiator;
            return new ConversationApiDto(c.Id, other?.UserName, other?.ProfilePicture, c.CreatedAt);
        }).ToList();

        var groupDtos = (groupsResult.Value ?? []).Select(g =>
            new GroupConversationApiDto(
                g.Id, g.Name,
                g.Members.Select(m =>
                        new GroupMemberApiDto(m.UserId, m.User?.UserName, m.User?.ProfilePicture, m.HasLeft))
                    .ToList(),
                g.CreatedAt
            )).ToList();

        return Ok(new GetConversationsApiResponse(conversationDtos, groupDtos));
    }

    [HttpPost("with/{username}")]
    [ProducesResponseType(typeof(CreateConversationApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateConversationApiResponse>> StartConversation(
        [FromRoute] string username,
        CancellationToken ct = default)
    {
        var userId = CurrentUserId;
        var otherResult = await _userRepository.GetByUsernameAsync(username);
        if (!otherResult.IsSuccess)
            return NotFound(new ErrorResponse("User not found"));

        var result = await _conversationRepository.GetOrCreateConversationAsync(userId, otherResult.Value!.Id);
        if (!result.IsSuccess)
            return StatusCode(500, new ErrorResponse(result.ErrorMessage));

        return Ok(new CreateConversationApiResponse(result.Value!.Id));
    }

    [HttpGet("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(GetMessagesApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetMessagesApiResponse>> GetMessages(
        [FromRoute] Guid conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _conversationRepository.GetConversationMessagesAsync(conversationId, page, pageSize);
        if (!result.IsSuccess)
            return StatusCode(500, new ErrorResponse(result.ErrorMessage));

        return Ok(new GetMessagesApiResponse(MapMessages(result.Value!), page, pageSize));
    }

    [HttpPost("{conversationId:guid}/messages")]
    public async Task<ActionResult<SendMessageApiResponse>> SendMessage(
        [FromRoute] Guid conversationId,
        [FromBody] SendMessageApiRequest apiRequest,
        CancellationToken ct = default)
    {
        var userId = CurrentUserId;
        var result = await _conversationRepository.SendToConversationAsync(conversationId, userId,
            apiRequest.Content, apiRequest.ParentMessageId);
        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.ContentEmpty or ErrorType.ContentTooLong =>
                    BadRequest(new ErrorResponse(result.ErrorMessage)),
                _ => StatusCode(500, new ErrorResponse(result.ErrorMessage))
            };

        return Ok(new SendMessageApiResponse(MapMessage(result.Value!)));
    }

    [HttpPost("groups")]
    [ProducesResponseType(typeof(CreateGroupConversationApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateGroupConversationApiResponse>> CreateGroup(
        [FromBody] CreateGroupConversationApiRequest apiRequest,
        CancellationToken ct = default)
    {
        var userId = CurrentUserId;
        var result =
            await _conversationRepository.CreateGroupConversationAsync(userId, apiRequest.Name, apiRequest.MemberIds);
        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.NameEmpty or ErrorType.NameTooLong =>
                    BadRequest(new ErrorResponse(result.ErrorMessage)),
                _ => StatusCode(500, new ErrorResponse(result.ErrorMessage))
            };

        return Ok(new CreateGroupConversationApiResponse(result.Value!.Id));
    }

    [HttpGet("groups/{groupId:guid}/messages")]
    public async Task<ActionResult<GetMessagesApiResponse>> GetGroupMessages(
        [FromRoute] Guid groupId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _conversationRepository.GetGroupConversationMessagesAsync(groupId, page, pageSize);
        if (!result.IsSuccess)
            return StatusCode(500, new ErrorResponse(result.ErrorMessage));

        return Ok(new GetMessagesApiResponse(MapMessages(result.Value!), page, pageSize));
    }

    [HttpPost("groups/{groupId:guid}/messages")]
    [ProducesResponseType(typeof(SendMessageApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SendMessageApiResponse>> SendGroupMessage(
        [FromRoute] Guid groupId,
        [FromBody] SendMessageApiRequest apiRequest,
        CancellationToken ct = default)
    {
        var userId = CurrentUserId;
        var result = await _conversationRepository.SendToGroupConversationAsync(groupId, userId,
            apiRequest.Content, apiRequest.ParentMessageId);
        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.ContentEmpty or ErrorType.ContentTooLong =>
                    BadRequest(new ErrorResponse(result.ErrorMessage)),
                ErrorType.NotAMember => Forbid(),
                _ => StatusCode(500, new ErrorResponse(result.ErrorMessage))
            };

        return Ok(new SendMessageApiResponse(MapMessage(result.Value!)));
    }

    [HttpPost("groups/{groupId:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddMember(
        [FromRoute] Guid groupId,
        [FromBody] AddGroupMemberApiRequest apiRequest,
        CancellationToken ct = default)
    {
        var userId = CurrentUserId;
        var result = await _conversationRepository.AddMemberToGroupAsync(groupId, userId, apiRequest.UserId);
        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.AlreadyMember => BadRequest(new ErrorResponse(result.ErrorMessage)),
                ErrorType.NotAMember => Forbid(),
                _ => StatusCode(500, new ErrorResponse(result.ErrorMessage))
            };

        return NoContent();
    }

    private static DirectMessageApiDto MapMessage(DirectMessage dm) => new(
        dm.Id, dm.SenderId, dm.Sender?.UserName, dm.Sender?.ProfilePicture,
        dm.Content, dm.IsDeleted, dm.IsReply,
        dm.ParentMessageId, dm.ParentMessage?.Content, dm.ParentMessage?.Sender?.UserName,
        dm.SentAt, dm.EditedAt
    );

    private static List<DirectMessageApiDto> MapMessages(IEnumerable<DirectMessage> messages) =>
        messages.Select(MapMessage).ToList();
}