using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class EditCommentUseCase : IEditCommentUseCase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<EditCommentUseCase> _logger;

    public EditCommentUseCase(
        ICommentRepository commentRepository,
        ILogger<EditCommentUseCase> logger
    )
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<EditCommentResponse> ExecuteAsync(EditCommentRequest request)
    {
        var fetchResult = await _commentRepository.GetByIdAsync(request.CommentId);
        if (!fetchResult.IsSuccess)
        {
            LogCommentFetchFailure(fetchResult.Exception, request.CommentId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new EditCommentResponse(false, message, fetchResult.ErrorType);
        }
        var comment = fetchResult.Value!;

        if (comment.CommentedById != request.RequestingUserId)
        {
            LogUnauthorizedEditAttempt(request.RequestingUserId, comment.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new EditCommentResponse(false, message, ErrorType.Forbidden);
        }
        if (comment.IsDeleted)
        {
            LogEditDeletedComment(request.CommentId);
            var message = ResolveErrorMessage(ErrorType.AlreadyDeleted);
            return new EditCommentResponse(false, message, ErrorType.AlreadyDeleted);
        }

        var editResult = comment.Edit(request.NewContent);
        if (!editResult.IsSuccess)
        {
            LogValidationFailure(request.CommentId, editResult.ErrorMessage);
            var message = ResolveErrorMessage(editResult.ErrorType);
            return new EditCommentResponse(false, message, editResult.ErrorType);
        }

        var updateResult = await _commentRepository.UpdateAsync(comment);
        if (!updateResult.IsSuccess)
        {
            LogUpdateFailure(updateResult.Exception, comment.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new EditCommentResponse(false, message, updateResult.ErrorType);
        }
        
        LogEditSuccess(comment.Id);
        return new EditCommentResponse(true, "Comment edited successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.CommentNotFound => "Comment not found.",
        ErrorType.Forbidden => "You are not authorized to edit this comment.",
        ErrorType.AlreadyDeleted => "Cannot edit a deleted comment.",
        ErrorType.ContentEmpty => "Comment content cannot be empty.",
        ErrorType.ContentTooLong => $"Comment content cannot exceed {Comment.Limits.MaxContentLength} characters.",
        ErrorType.ConcurrencyFailure =>
            "Failed to update comment due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}