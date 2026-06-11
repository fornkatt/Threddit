using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.API.Mappers;
using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Posts;
using Threddit.Contracts.Responses.Posts;
using Threddit.Domain.Common;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/subthreads/{subThreadName}/posts")]
public sealed class PostController : ControllerBase
{
    private readonly ICreatePostUseCase _createPostUseCase;
    private readonly IDeletePostUseCase _deletePostUseCase;
    private readonly IGetPostByIdWithCommentsUseCase _getPostWithCommentsUseCase;
    private readonly IVotePostUseCase _votePostUseCase;
    private readonly IEditPostUseCase _editPostUseCase;

    public PostController(
        ICreatePostUseCase createPostUseCase,
        IDeletePostUseCase deletePostUseCase,
        IGetPostByIdWithCommentsUseCase getPostWithCommentsUseCase,
        IVotePostUseCase votePostUseCase,
        IEditPostUseCase editPostUseCase
    )
    {
        _createPostUseCase = createPostUseCase;
        _deletePostUseCase = deletePostUseCase;
        _getPostWithCommentsUseCase = getPostWithCommentsUseCase;
        _votePostUseCase = votePostUseCase;
        _editPostUseCase = editPostUseCase;
    }

    [HttpPost]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(CreatePostApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreatePostApiResponse>> CreatePost(
        [FromRoute] string subThreadName,
        [FromBody] CreatePostApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _createPostUseCase.ExecuteAsync(new CreatePostRequest(
            userId,
            subThreadName,
            apiRequest.Title,
            apiRequest.Content,
            apiRequest.ImageUrl
        ));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.TitleEmpty or ErrorType.TitleTooLong
                    or ErrorType.ContentEmpty or ErrorType.ContentTooLong
                    or ErrorType.ImageUrlTooLong => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.SubThreadBanned => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var post = result.Post!;

        return CreatedAtAction(nameof(GetPostWithComments), new { subThreadName, postId = post.Id },
            new CreatePostApiResponse(
                post.Id,
                post.PostedByUsername,
                post.PostedByProfilePicture,
                post.Title,
                post.Content,
                post.ImageUrl,
                post.Slug,
                post.Score,
                post.CommentCount,
                post.PostedAt
            ));
    }

    [HttpDelete("{postId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePost(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromQuery] string? reason = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isSiteAdmin = User.HasClaim("role", "SiteAdmin");
        var isSiteOwner = User.HasClaim("role", "SiteOwner");

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableArray();

        var useCaseRequest = new DeletePostRequest(
            postId,
            userId,
            isSiteAdmin,
            isSiteOwner,
            moderatedSubThreadIds,
            reason
        );

        var result = await _deletePostUseCase.ExecuteAsync(useCaseRequest);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.PostNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.AlreadyDeleted or ErrorType.ConcurrencyFailure =>
                    Conflict(new ErrorResponse(result.Message)),
                ErrorType.DeleteReasonTooLong or ErrorType.DeleteReasonRequired =>
                    BadRequest(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        return NoContent();
    }

    [HttpGet("{postId:guid}")]
    [ProducesResponseType(typeof(GetPostWithCommentsApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetPostWithCommentsApiResponse>> GetPostWithComments(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId)
    {
        var result = await _getPostWithCommentsUseCase
            .ExecuteAsync(new GetPostByIdWithCommentsRequest(postId));

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.PostNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };
        }

        var postWithComments = result.PostWithComments!;
        var post = postWithComments.Post;

        return Ok(new GetPostWithCommentsApiResponse(
            post.Id,
            post.SubThreadId,
            post.PostedById,
            post.PostedByUsername,
            post.PostedByProfilePicture,
            post.Title,
            post.Content,
            post.ImageUrl,
            post.Slug!,
            post.Score,
            post.CommentCount,
            post.PostedAt,
            post.UpdatedAt,
            CommentMapper.MapComments(postWithComments.Comments)
        ));
    }

    [HttpPost("{postId:guid}/vote")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(VotePostApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VotePostApiResponse>> VotePost(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromQuery] bool isUpvote)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _votePostUseCase.ExecuteAsync(new VotePostRequest(postId, userId, isUpvote));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.PostNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(new VotePostApiResponse(result.NewScore!.Value));
    }

    [HttpPatch("{postId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] string subThreadName,
        [FromRoute] Guid postId,
        [FromBody] EditPostApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _editPostUseCase.ExecuteAsync(new EditPostRequest(
            postId,
            userId,
            apiRequest.NewContent,
            apiRequest.NewImageUrl
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.PostNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.AlreadyDeleted or ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.ContentEmpty or ErrorType.ContentTooLong or ErrorType.ImageUrlTooLong
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }
}