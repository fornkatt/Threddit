using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class CreatePostUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} not found when trying to create post. Error: {ErrorMessage}")]
    private partial void LogUserNotFound(Exception? ex, Guid userId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "SubThread '{SubThreadName}' not found when trying to create post. Error: {ErrorMessage}")]
    private partial void LogSubThreadNotFound(Exception? ex, string? subThreadName, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to create post while being banned from SubThread '{SubThreadName}'")]
    private partial void LogUserSubThreadBanned(Guid userId, string subThreadName);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create post '{Title}'. Error: {ErrorMessage}")]
    private partial void LogCreationFailure(Exception? ex, string title, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Post '{Title}' created successfully with ID {PostId} by {UserName} with ID {UserId}")]
    private partial void LogPostCreated(Guid postId, string title, string username, Guid userId);
}