using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class DeleteCommentUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch comment {CommentId} for deletion. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid commentId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} attempted to delete comment {CommentId} without authorization.")]
    private partial void LogUnauthorizedDeletionAttempt(Guid userId, Guid commentId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to soft-delete comment {CommentId}. Error: {ErrorMessage}")]
    private partial void LogDeletionFailure(Exception? ex, Guid commentId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Database update failed for comment {CommentId}. Error: {ErrorMessage}")]
    private partial void LogDatabaseUpdateFailure(Exception? ex, Guid commentId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Successfully deleted comment {CommentId}.")]
    private partial void LogDeletionSuccess(Guid commentId);
}