using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class CreateSiteReportUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch reporter user {UserId} for site report creation - repository returned: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch report target {TargetId} for site report creation - repository returned: {ErrorMessage}")]
    private partial void LogTargetFetchFailure(Exception? ex, Guid targetId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Validation failed while creating site report - repository returned: {ErrorMessage}")]
    private partial void LogValidationFailure(string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to save site report - repository returned: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Site report {ReportId} of type {ReportType} created successfully.")]
    private partial void LogCreateSuccess(Guid reportId, string reportType);
}