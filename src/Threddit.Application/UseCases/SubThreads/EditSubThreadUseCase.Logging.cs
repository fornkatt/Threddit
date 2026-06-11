using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class EditSubThreadUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "SubThread '{SubThreadName}' not found for editing. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to edit SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedEditAttempt(Guid userId, Guid subThreadId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Validation failed while editing SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid subThreadId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update SubThread {SubThreadId} in database. Error: {ErrorMessage}")]
    private partial void LogUpdateFailure(Exception? ex, Guid subThreadId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "SubThread '{Name}' ({SubThreadId}) updated successfully by user {UserId}.")]
    private partial void LogEditSuccess(string name, Guid subThreadId, Guid userId);
}