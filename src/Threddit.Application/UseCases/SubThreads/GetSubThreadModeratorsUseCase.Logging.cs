using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadModeratorsUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch moderators for SubThread '{SubThreadName}'. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, string subThreadName, string? errorMessage);
}