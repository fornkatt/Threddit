using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class DeletePostUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch post with id {PostId}. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Guid postId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Error, 
        Message = "Database update failed for post {PostId}. Error: {ErrorMessage}")]
    private partial void LogDatabaseUpdateFailure(Exception? ex, string? errorMessage, Guid postId);

    [LoggerMessage(
        Level = LogLevel.Information, 
        Message = "Successfully deleted post {PostId}")]
    private partial void LogDeletionSuccess(Guid postId);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to delete post {PostId}. Error: {ErrorMessage}")]
    private partial void LogDeletionFailure(Guid postId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to delete post {PostId} without authorization.")]
    private partial void LogUnauthorizedDeletionAttempt(Guid userId, Guid postId);
}