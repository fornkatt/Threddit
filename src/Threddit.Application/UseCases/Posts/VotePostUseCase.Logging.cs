using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class VotePostUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to save vote for post {PostId} by user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid postId, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Vote successfully saved for post {PostId} by user {UserId}")]
    private partial void LogVoteSuccess(Guid postId, Guid userId);
}