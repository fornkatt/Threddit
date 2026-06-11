using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class VoteCommentUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to save vote for comment {CommentId} by user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid commentId, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Vote successfully saved for comment {CommentId} by user {UserId}")]
    private partial void LogVoteSuccess(Guid commentId, Guid userId);
}