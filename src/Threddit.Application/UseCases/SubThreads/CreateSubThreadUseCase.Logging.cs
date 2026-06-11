using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class CreateSubThreadUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {UserId} not found when trying to create SubThread. Error: {ErrorMessage}")]
    private partial void LogUserNotFound(Guid userId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message =
            "User {UserId} tried to create SubThread with name '{Name}' but it already exists.")]
    private partial void LogSubThreadAlreadyExists(Guid userId, string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to create SubThread '{Name}'. Error: {ErrorMessage}")]
    private partial void LogCreationFailure(Exception? ex, string name, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to save SubThread '{Name}' to database. Error: {ErrorMessage}")]
    private partial void LogDatabaseSaveFailure(Exception? ex, string name, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "SubThread '{Name}' created successfully with ID {SubThreadId}")]
    private partial void LogCreationSuccess(string name, Guid subThreadId);
}