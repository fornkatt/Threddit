using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class CreateCommentUseCase : ICreateCommentUseCase
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateCommentUseCase> _logger;

    public CreateCommentUseCase(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILogger<CreateCommentUseCase> logger
    )
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreateCommentResponse> ExecuteAsync(CreateCommentRequest request)
    {
        var userResult = await _userRepository.GetByIdWithRolesAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserNotFound(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new CreateCommentResponse(false, Message: message, ErrorType: userResult.ErrorType);
        }

        var postResult = await _postRepository.GetByIdAsync(request.PostId);
        if (!postResult.IsSuccess)
        {
            LogPostNotFound(postResult.Exception, request.PostId, postResult.ErrorMessage);
            var message = ResolveErrorMessage(postResult.ErrorType);
            return new CreateCommentResponse(false, Message: message, ErrorType: postResult.ErrorType);
        }
        
        var user = userResult.Value!;
        var post = postResult.Value!;
        
        if (user.ReceivedSubThreadBans
            .Any(bs => bs.SubThreadId == post.SubThreadId && !bs.IsExpired))
        {
            LogUserSubThreadBanned(user.Id, post.SubThreadId, post.Title!);
            return new CreateCommentResponse(false, Message: "User is banned from this SubThread.",
                ErrorType: ErrorType.SubThreadBanned);
        }

        Comment? parentComment = null;
        if (request.ParentCommentId.HasValue)
        {
            var parentResult = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value);
            if (!parentResult.IsSuccess)
            {
                LogParentCommentNotFound(parentResult.Exception, request.ParentCommentId.Value,
                    parentResult.ErrorMessage);
                var message = ResolveErrorMessage(parentResult.ErrorType);
                return new CreateCommentResponse(false, Message: message, ErrorType: parentResult.ErrorType);
            }
            parentComment = parentResult.Value!;
            parentComment.ApplyReplyCountDelta(1);
        }

        var createResult = Comment.Create(
            user,
            post,
            request.Content,
            parentComment,
            request.ImageUrl
        );

        if (!createResult.IsSuccess)
            return new CreateCommentResponse(false, Message: ResolveErrorMessage(createResult.ErrorType),
                ErrorType: createResult.ErrorType);

        var newComment = createResult.Value!;
        
        post.ApplyCommentCountDelta(1);

        var saveResult = await _commentRepository.CreateWithCounterUpdatesAsync(newComment,
            post, parentComment);
        if (!saveResult.IsSuccess)
        {
            LogCreationFailure(saveResult.Exception, request.PostId, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new CreateCommentResponse(false, Message: message, ErrorType: saveResult.ErrorType);
        }

        var comment = saveResult.Value!;

        LogCreationSuccess(comment.Id, comment.PostId);

        var dto = new CommentDto(
            comment.Id,
            comment.CommentedById,
            comment.CommentedBy?.UserName,
            comment.CommentedBy?.ProfilePicture,
            comment.Content,
            comment.ImageUrl,
            comment.Score,
            comment.CommentedAt,
            comment.UpdatedAt,
            comment.IsDeleted,
            [],
            comment.ReplyCount > 0
        );

        return new CreateCommentResponse(true, dto);
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.ContentEmpty => "Comment content cannot be empty.",
        ErrorType.ContentTooLong => $"Comment content cannot exceed {Comment.Limits.MaxContentLength} characters.",
        ErrorType.PostNotFound => "Post not found.",
        ErrorType.CommentNotFound => "Parent comment not found.",
        ErrorType.UserNotFound => "Requesting user not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again.",
        ErrorType.ConcurrencyFailure => "Comment creation failed due to internal server error. Please refresh and try again.",
        _ => "An unexpected error occurred. Please try again later."
    };
}