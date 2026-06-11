using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class GetCommentRepliesUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch comment with ID: {CommentId}. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid? commentId, string? errorMessage);
}