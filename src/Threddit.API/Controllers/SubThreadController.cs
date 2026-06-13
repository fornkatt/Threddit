using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.SubThreads;
using Threddit.Contracts.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/subthreads")]
public sealed class SubThreadController : ControllerBase
{
    private readonly IGetSubThreadsBySearchUseCase _searchUseCase;
    private readonly IGetSubThreadByNameUseCase _getByNameUseCase;
    private readonly IGetSubThreadPostsUseCase _getPostsUseCase;
    private readonly ICreateSubThreadUseCase _createSubThreadUseCase;
    private readonly IDeleteSubThreadUseCase _deleteSubThreadUseCase;
    private readonly ISubscribeSubThreadUseCase _subscribeSubThreadUseCase;
    private readonly IUnsubscribeSubThreadUseCase _unsubscribeSubThreadUseCase;
    private readonly IEditSubThreadUseCase _editSubThreadUseCase;
    private readonly ICreateSubThreadRuleUseCase _createSubThreadRuleUseCase;
    private readonly IDeleteSubThreadRuleUseCase _deleteSubThreadRuleUseCase;
    private readonly IEditSubThreadRuleUseCase _editSubThreadRuleUseCase;
    private readonly IGetSubThreadModeratorsUseCase _getModeratorsUseCase;

    public SubThreadController(
        IGetSubThreadsBySearchUseCase searchUseCase,
        IGetSubThreadByNameUseCase getByNameUseCase,
        IGetSubThreadPostsUseCase getPostsUseCase,
        ICreateSubThreadUseCase createSubThreadUseCase,
        IDeleteSubThreadUseCase deleteSubThreadUseCase,
        ISubscribeSubThreadUseCase subscribeSubThreadUseCase,
        IUnsubscribeSubThreadUseCase unsubscribeSubThreadUseCase,
        IEditSubThreadUseCase editSubThreadUseCase,
        ICreateSubThreadRuleUseCase createSubThreadRuleUseCase,
        IDeleteSubThreadRuleUseCase deleteSubThreadRuleUseCase,
        IEditSubThreadRuleUseCase editSubThreadRuleUseCase,
        IGetSubThreadModeratorsUseCase getModeratorsUseCase
    )
    {
        _searchUseCase = searchUseCase;
        _getByNameUseCase = getByNameUseCase;
        _getPostsUseCase = getPostsUseCase;
        _createSubThreadUseCase = createSubThreadUseCase;
        _deleteSubThreadUseCase = deleteSubThreadUseCase;
        _subscribeSubThreadUseCase = subscribeSubThreadUseCase;
        _unsubscribeSubThreadUseCase = unsubscribeSubThreadUseCase;
        _editSubThreadUseCase = editSubThreadUseCase;
        _createSubThreadRuleUseCase = createSubThreadRuleUseCase;
        _deleteSubThreadRuleUseCase = deleteSubThreadRuleUseCase;
        _editSubThreadRuleUseCase = editSubThreadRuleUseCase;
        _getModeratorsUseCase = getModeratorsUseCase;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedApiResponse<GetSubThreadSearchApiResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedApiResponse<GetSubThreadSearchApiResponse>>> Search(
        [FromQuery] GetSubThreadsBySearchApiRequest apiRequest)
    {
        var result = await _searchUseCase
            .ExecuteAsync(new GetSubThreadsBySearchRequest(apiRequest.Query, apiRequest.Page, apiRequest.PageSize));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var paged = result.SubThreads!;

        return Ok(new PagedApiResponse<GetSubThreadSearchApiResponse>(
            paged.Items.Select(s => new GetSubThreadSearchApiResponse(
                s.Name,
                s.SubscriberCount
            )).ToList().AsReadOnly(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount,
            paged.HasNextPage
        ));
    }

    [HttpGet("{subThreadName}")]
    [ProducesResponseType(typeof(GetSubThreadApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSubThreadApiResponse>> GetByName(
        [FromRoute] string subThreadName)
    {
        var result = await _getByNameUseCase
            .ExecuteAsync(new GetSubThreadByNameRequest(subThreadName));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.NameEmpty => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var subThread = result.SubThread!;

        return Ok(new GetSubThreadApiResponse(
            subThread.Id,
            subThread.CreatedByUsername,
            subThread.Name,
            subThread.Description,
            subThread.BannerImageUrl,
            subThread.SubscriberCount,
            subThread.CreatedAt,
            subThread.UpdatedAt,
            subThread.Rules
                .Select(r => new GetSubThreadRuleApiResponse(r.Id, r.Title, r.Content))
                .ToImmutableList()
        ));
    }

    [HttpGet("{subThreadName}/posts")]
    [ProducesResponseType(typeof(PagedApiResponse<GetSubThreadPostApiResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedApiResponse<GetSubThreadPostApiResponse>>> GetPosts(
        [FromRoute] string subThreadName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] PostSortOrder sort = PostSortOrder.New)
    {
        var result = await _getPostsUseCase
            .ExecuteAsync(new GetSubThreadPostsRequest(subThreadName, page, pageSize, sort));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.NameEmpty => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var paged = result.Posts!;

        return Ok(new PagedApiResponse<GetSubThreadPostApiResponse>(
            paged.Items.Select(p => new GetSubThreadPostApiResponse(
                p.Id,
                p.PostedById,
                p.PostedByUsername,
                p.PostedByProfilePicture,
                p.Title,
                p.Content,
                p.ImageUrl,
                p.Slug,
                p.Score,
                p.CommentCount,
                p.PostedAt,
                p.UpdatedAt
            )).ToList().AsReadOnly(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount,
            paged.HasNextPage
        ));
    }

    [HttpPost]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(CreateSubThreadApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateSubThreadApiResponse>> CreateSubThread(
        [FromBody] CreateSubThreadApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _createSubThreadUseCase.ExecuteAsync(new CreateSubThreadRequest(
            userId,
            apiRequest.Name,
            apiRequest.Description,
            apiRequest.BannerUrl
        ));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NameEmpty or ErrorType.NameTooLong or
                    ErrorType.ContentTooLong
                    or ErrorType.ImageUrlTooLong => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.AlreadyAssigned or ErrorType.ConcurrencyFailure =>
                    Conflict(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var subThread = result.SubThread!;

        return CreatedAtAction(nameof(GetByName), new { subThreadName = subThread.Name },
            new CreateSubThreadApiResponse(
                subThread.Id,
                subThread.Name,
                subThread.Description,
                subThread.BannerImageUrl,
                subThread.SubscriberCount,
                subThread.CreatedAt
            ));
    }

    [HttpDelete("{subThreadId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSubThread([FromRoute] Guid subThreadId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isSiteAdmin = User.HasClaim("role", "SiteAdmin");
        var isSiteOwner = User.HasClaim("role", "SiteOwner");

        var result = await _deleteSubThreadUseCase.ExecuteAsync(new DeleteSubThreadRequest(
            subThreadId,
            userId,
            isSiteAdmin,
            isSiteOwner
        ));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(
                    new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        return NoContent();
    }

    [HttpPost("{subThreadName}/subscribe")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Subscribe([FromRoute] string subThreadName)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _subscribeSubThreadUseCase
            .ExecuteAsync(new SubscribeSubThreadRequest(subThreadName, userId));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.AlreadySubscribed or ErrorType.ConcurrencyFailure => Conflict(
                    new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpDelete("{subThreadName}/subscribe")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Unsubscribe([FromRoute] string subThreadName)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _unsubscribeSubThreadUseCase
            .ExecuteAsync(new UnsubscribeSubThreadRequest(subThreadName, userId));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.NotSubscribed or ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpPatch("{subThreadName}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditSubThread(
        [FromRoute] string subThreadName,
        [FromBody] EditSubThreadApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _editSubThreadUseCase.ExecuteAsync(new EditSubThreadRequest(
            subThreadName,
            userId,
            apiRequest.Description,
            apiRequest.BannerImageUrl
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.ContentTooLong or ErrorType.ImageUrlTooLong
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpPost("{subThreadName}/rules")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CreateSubThreadRuleApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRule(
        [FromRoute] string subThreadName,
        [FromBody] CreateSubThreadRuleApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();
        
        var ownedSubThreadIds = User.FindAll("subthreadowner")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();

        var result = await _createSubThreadRuleUseCase
            .ExecuteAsync(new CreateSubThreadRuleRequest(
                subThreadName,
                userId,
                moderatedSubThreadIds,
                ownedSubThreadIds,
                apiRequest.RuleTitle,
                apiRequest.RuleContent
            ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound or ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.TitleEmpty or ErrorType.ContentEmpty or ErrorType.TitleTooLong or ErrorType.ContentTooLong
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return StatusCode(201, new CreateSubThreadRuleApiResponse(result.RuleId!.Value));
    }

    [HttpPatch("{subThreadName}/rules/{ruleId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditRule(
        [FromRoute] string subThreadName,
        [FromRoute] Guid ruleId,
        [FromBody] EditSubThreadRuleApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();
        
        var ownedSubThreadIds = User.FindAll("subthreadowner")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();
        
        var result = await _editSubThreadRuleUseCase.ExecuteAsync(new EditSubThreadRuleRequest(
            ruleId,
            Guid.Empty,
            userId,
            moderatedSubThreadIds,
            ownedSubThreadIds,
            apiRequest.RuleTitle,
            apiRequest.RuleContent
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadRuleNotFound or ErrorType.UserNotFound =>
                    NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.TitleEmpty or ErrorType.ContentEmpty or ErrorType.TitleTooLong or ErrorType.ContentTooLong
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpDelete("{subThreadName}/rules/{ruleId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRule(
        [FromRoute] string subThreadName,
        [FromRoute] Guid ruleId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();

        var ownedSubThreadIds = User.FindAll("subthreadowner")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();

        var result = await _deleteSubThreadRuleUseCase
            .ExecuteAsync(new DeleteSubThreadRuleRequest(
                ruleId,
                Guid.Empty,
                userId,
                moderatedSubThreadIds,
                ownedSubThreadIds
            ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadRuleNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpGet("{subThreadName}/moderators")]
    [ProducesResponseType(typeof(GetSubThreadModeratorsApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSubThreadModeratorsApiResponse>> GetModerators(
        [FromRoute] string subThreadName)
    {
        var result = await _getModeratorsUseCase
            .ExecuteAsync(new GetSubThreadModeratorsRequest(subThreadName));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(new GetSubThreadModeratorsApiResponse(
            result.Moderators.Select(m => new SubThreadModeratorDto(m.UserId, m.Username))
                .ToImmutableList()
        ));
    }
}