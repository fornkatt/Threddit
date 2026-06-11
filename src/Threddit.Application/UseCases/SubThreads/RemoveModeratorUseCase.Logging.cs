using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class RemoveModeratorUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread '{SubThreadName}' for moderator removal. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {RequestingUserId} attempted to remove a moderator from SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid requestingUserId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch target user {UserId} for moderator removal. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch moderator record for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogModeratorFetchFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Attempted to remove user {UserId} as moderator from SubThread {SubThreadId} but no moderator record was found.")]
    private partial void LogNotModerator(Guid userId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to remove moderator record for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogRemoveFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "User {TargetUserId} removed as moderator from SubThread {SubThreadId} by {RequestingUserId}.")]
    private partial void LogRemoveSuccess(Guid targetUserId, Guid subThreadId, Guid requestingUserId);
}