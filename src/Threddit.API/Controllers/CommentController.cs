using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.API.Mappers;
using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Comments;
using Threddit.Contracts.Responses.Comments;
using Threddit.Domain.Common;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/subthreads/{subThreadName}/posts/{postId:guid}/comments")]
public sealed class CommentController : ControllerBase
{
    private readonly IGetCommentRepliesUseCase _getCommentRepliesUseCase;
    private readonly ICreateCommentUseCase _createCommentUseCase;
    private readonly IVoteCommentUseCase _voteCommentUseCase;
    private readonly IDeleteCommentUseCase _deleteCommentUseCase;
    private readonly IEditCommentUseCase _editCommentUseCase;

    public CommentController(
        IGetCommentRepliesUseCase getCommentRepliesUseCase,
        ICreateCommentUseCase createCommentUseCase,
        IVoteCommentUseCase voteCommentUseCase,
        IDeleteCommentUseCase deleteCommentUseCase,
        IEditCommentUseCase editCommentUseCase
    )
    {
        _getCommentRepliesUseCase = getCommentRepliesUseCase;
        _createCommentUseCase = createCommentUseCase;
        _voteCommentUseCase = voteCommentUseCase;
        _deleteCommentUseCase = deleteCommentUseCase;
        _editCommentUseCase = editCommentUseCase;
    }

    [HttpGet("{commentId:guid}/replies")]
    [ProducesResponseType(typeof(IReadOnlyList<GetCommentApiResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<GetCommentApiResponse>>> GetCommentReplies(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId)
    {
        var result = await _getCommentRepliesUseCase
            .ExecuteAsync(new GetCommentRepliesRequest(commentId));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(CommentMapper.MapComments(result.Replies!));
    }

    [HttpPost]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(CreateCommentApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateCommentApiResponse>> CreateComment(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromBody] CreateCommentApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _createCommentUseCase.ExecuteAsync(new CreateCommentRequest(
            userId,
            postId,
            apiRequest.Content,
            apiRequest.ImageUrl,
            apiRequest.ParentCommentId
        ));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.ContentEmpty or ErrorType.ContentTooLong => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.PostNotFound or ErrorType.CommentNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.SubThreadBanned => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var comment = result.Comment!;

        return StatusCode(201, new CreateCommentApiResponse(
            comment.Id,
            comment.CommentedById,
            comment.CommentedByUsername,
            comment.CommentedByProfilePicture,
            comment.Content,
            comment.ImageUrl,
            comment.Score,
            comment.CommentedAt,
            comment.IsDeleted,
            apiRequest.ParentCommentId,
            comment.HasReplies
        ));
    }

    [HttpPost("{commentId:guid}/vote")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(VoteCommentApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VoteCommentApiResponse>> VoteComment(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromQuery] bool isUpvote)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result =
            await _voteCommentUseCase.ExecuteAsync(new VoteCommentRequest(commentId, userId, isUpvote));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.CommentNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(new VoteCommentApiResponse(result.NewScore!.Value));
    }

    [HttpDelete("{commentId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromQuery] string? reason = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isSiteAdmin = User.HasClaim("role", "SiteAdmin");
        var isSiteOwner = User.HasClaim("role", "SiteOwner");

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();
        
        var ownedSubThreads = User.FindAll("subthreadowner")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableHashSet();

        var result = await _deleteCommentUseCase.ExecuteAsync(new DeleteCommentRequest(
            commentId,
            userId,
            isSiteAdmin,
            isSiteOwner,
            moderatedSubThreadIds,
            ownedSubThreads,
            reason
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.CommentNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.AlreadyDeleted or ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DeleteReasonTooLong or ErrorType.DeleteReasonRequired =>
                    BadRequest(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message)),
            };

        return NoContent();
    }

    [HttpPatch("{commentId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditComment(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromBody] EditCommentApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _editCommentUseCase.ExecuteAsync(new EditCommentRequest(
            commentId,
            userId,
            apiRequest.NewContent
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.CommentNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.AlreadyDeleted or ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.ContentEmpty or ErrorType.ContentTooLong => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }
}