using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class SetReportStatusUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch report {ReportId} for status update - repository returned: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid reportId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread {SubThreadId} for owner comparison - repository returned: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, Guid subThreadId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to set status on report {ReportId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid userId, Guid reportId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Report {ReportId} already has status {Status} - no change made.")]
    private partial void LogSameStatus(Guid reportId, string status);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update status for report {ReportId} - repository returned: {ErrorMessage}")]
    private partial void LogUpdateFailure(Exception? ex, Guid reportId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Report {ReportId} status updated to {NewStatus}.")]
    private partial void LogStatusUpdated(Guid reportId, string newStatus);
}