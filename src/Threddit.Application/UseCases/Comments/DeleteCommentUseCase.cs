using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class DeleteCommentUseCase : IDeleteCommentUseCase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<DeleteCommentUseCase> _logger;

    public DeleteCommentUseCase(
        ICommentRepository commentRepository,
        ILogger<DeleteCommentUseCase> logger
    )
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<DeleteCommentResponse> ExecuteAsync(DeleteCommentRequest request)
    {
        var fetchResult = await _commentRepository.GetByIdAsync(request.CommentId);
        if (!fetchResult.IsSuccess)
        {
            LogFetchFailure(fetchResult.Exception, request.CommentId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new DeleteCommentResponse(false, message, fetchResult.ErrorType);
        }

        var comment = fetchResult.Value!;

        var isSitePrivileged = request.IsSiteAdmin || request.IsSiteOwner;
        var isModerator = request.ModeratedSubThreadIds.Contains(comment.SubThreadId);
        var isAuthor = comment.CommentedById == request.RequestingUserId;

        if (!isSitePrivileged && !isModerator && !isAuthor)
        {
            LogUnauthorizedDeletionAttempt(request.RequestingUserId, request.CommentId);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new DeleteCommentResponse(false, message, ErrorType.Forbidden);
        }
        
        if (!isAuthor && (isSitePrivileged || isModerator) && string.IsNullOrWhiteSpace(request.Reason))
        {
            LogDeletionFailure(null, request.CommentId,
                "Reason is required for site admins and moderators.");
            var message = ResolveErrorMessage(ErrorType.DeleteReasonRequired);
            return new DeleteCommentResponse(false, message, ErrorType.DeleteReasonRequired);
        }
        
        var deleteResult = comment.SoftDelete(request.Reason);
        if (!deleteResult.IsSuccess)
        {
            LogDeletionFailure(deleteResult.Exception, request.CommentId, deleteResult.ErrorMessage);
            var message = ResolveErrorMessage(deleteResult.ErrorType);
            return new DeleteCommentResponse(false, message, deleteResult.ErrorType);
        }

        var updateResult = await _commentRepository.UpdateAsync(comment);
        if (!updateResult.IsSuccess)
        {
            LogDatabaseUpdateFailure(updateResult.Exception, comment.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new DeleteCommentResponse(false, message, updateResult.ErrorType);
        }
        
        LogDeletionSuccess(comment.Id);
        return new DeleteCommentResponse(true, "Comment deletion successful.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.CommentNotFound => "Comment not found.",
        ErrorType.Forbidden => "You are not authorized to delete this comment.",
        ErrorType.DeleteReasonRequired => "Delete reason is required.",
        ErrorType.AlreadyDeleted => "Comment is already deleted.",
        ErrorType.DeleteReasonTooLong => $"Delete reason cannot exceed {Comment.Limits.MaxDeleteReasonLength} characters.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        ErrorType.ConcurrencyFailure => "The comment may have been modified by another user. Please refresh and try again.",
        _ => "An unexpected error occurred. Please try again later."
    };
}