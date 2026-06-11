using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class GetSubThreadReportsUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message =
            "Failed to fetch SubThread '{SubThreadName}' for report listing - repository returned: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to view reports for SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedAccess(Guid userId, Guid subThreadId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to fetch reports for SubThread {SubThreadId} - repository returned: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid subThreadId, string? errorMessage);
}