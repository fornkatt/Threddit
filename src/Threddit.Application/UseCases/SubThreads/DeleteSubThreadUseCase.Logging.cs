using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class DeleteSubThreadUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "SubThread {SubThreadId} not found for deletion. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid subThreadId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} attempted to delete SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedDeletionAttempt(Guid userId, Guid subThreadId);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to delete SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogDeletionFailure(Exception? ex, Guid subThreadId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "SubThread '{Name}' ({SubThreadId}) deleted successfully.")]
    private partial void LogDeletionSuccess(string name, Guid subThreadId);
}