using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadPostsUseCase
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "SubThread not found when trying to load posts: '{Name}'. Error: {ErrorMessage}")]
    private partial void LogNotFound(Exception? ex, string name, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to load posts for SubThread '{Name}'. Error: {ErrorMessage}")]
    private partial void LogFailedToLoadPosts(Exception? ex, string name, string? errorMessage);
}