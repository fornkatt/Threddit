using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class GetPostByIdWithCommentsUseCase
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to fetch post with ID: '{PostId}'. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid postId, string? errorMessage);
}