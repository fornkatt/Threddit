using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class EditCommentUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch comment {CommentId} for editing. Error: {ErrorMessage}")]
    private partial void LogCommentFetchFailure(Exception? ex, Guid commentId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to edit comment {CommentId} without authorization.")]
    private partial void LogUnauthorizedEditAttempt(Guid userId, Guid commentId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Attempted to edit deleted comment {CommentId}.")]
    private partial void LogEditDeletedComment(Guid commentId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Validation failed while editing comment {CommentId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid commentId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update comment {CommentId} in database. Error: {ErrorMessage}")]
    private partial void LogUpdateFailure(Exception? ex, Guid commentId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Comment {CommentId} updated successfully.")]
    private partial void LogEditSuccess(Guid commentId);
}