using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class AssignModeratorUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread '{SubThreadName}' for moderator assignment. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {RequestingUserId} attempted to assign a moderator in SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid requestingUserId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch target user {UserId} for moderator assignment. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch existing moderator record for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogModeratorFetchFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} is already a moderator of SubThread {SubThreadId}.")]
    private partial void LogAlreadyModerator(Guid userId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to save moderator assignment for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "User {TargetUserId} assigned as moderator of SubThread {SubThreadId} by {RequestingUserId}.")]
    private partial void LogAssignSuccess(Guid targetUserId, Guid subThreadId, Guid requestingUserId);
}