using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class EditPostUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch post {PostId} for editing. Error: {ErrorMessage}")]
    private partial void LogPostFetchFailure(Exception? ex, Guid postId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to edit post {PostId} without authorization.")]
    private partial void LogUnauthorizedEditAttempt(Guid userId, Guid postId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Attempted to edit deleted post {PostId}.")]
    private partial void LogEditDeletedPost(Guid postId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Validation failed while editing post {PostId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid postId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update post {PostId} in database. Error: {ErrorMessage}")]
    private partial void LogUpdateFailure(Exception? ex, Guid postId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Post {PostId} updated successfully.")]
    private partial void LogEditSuccess(Guid postId);
}