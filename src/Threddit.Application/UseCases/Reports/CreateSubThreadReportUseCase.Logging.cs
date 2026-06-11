using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class CreateSubThreadReportUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message =
            "Failed to fetch SubThread '{SubThreadName}' for report creation - repository returned: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch reporter user {UserId} for report creation - repository returned: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch post {PostId} for report creation - repository returned: {ErrorMessage}")]
    private partial void LogPostFetchFailure(Exception? ex, Guid postId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch comment {CommentId} for report creation - repository returned: {ErrorMessage}")]
    private partial void LogCommentFetchFailure(Exception? ex, Guid commentId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Validation failed while creating SubThread report - repository returned: {ErrorMessage}")]
    private partial void LogValidationFailure(string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to save SubThread report - repository returned: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "SubThread report {ReportId} of type {ReportType} created successfully.")]
    private partial void LogCreateSuccess(Guid reportId, string reportType);
}