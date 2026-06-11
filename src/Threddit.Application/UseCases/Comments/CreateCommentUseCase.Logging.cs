using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class CreateCommentUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} not found when trying to create comment. Error: {ErrorMessage}")]
    private partial void LogUserNotFound(Exception? ex, Guid userId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Post {PostId} not found when trying to create comment. Error: {ErrorMessage}")]
    private partial void LogPostNotFound(Exception? ex, Guid postId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to create comment while being banned from SubThread" +
                  " '{SubThreadId}' in post '{PostTitle}'")]
    private partial void LogUserSubThreadBanned(Guid userId, Guid subThreadId, string postTitle);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Parent comment {ParentCommentId} not found when trying to create comment. Error: {ErrorMessage}")]
    private partial void LogParentCommentNotFound(Exception? ex, Guid parentCommentId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create comment on post {PostId}. Error: {ErrorMessage}")]
    private partial void LogCreationFailure(Exception? ex, Guid postId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Comment {CommentId} created successfully on post {PostId}")]
    private partial void LogCreationSuccess(Guid commentId, Guid postId);
}