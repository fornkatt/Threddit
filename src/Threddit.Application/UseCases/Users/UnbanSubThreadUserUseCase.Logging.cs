using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class UnbanSubThreadUserUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread '{SubThreadName}' for SubThread unban. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message =
            "User {RequestingUserId} attempted to unban a user in SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid requestingUserId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for SubThread unban. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread ban for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogBanFetchFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Attempted to unban user {UserId} in SubThread {SubThreadId} but no active ban was found.")]
    private partial void LogNoBanFound(Guid userId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to remove SubThread ban for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogRemoveFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "SubThread ban removed for user {TargetUserId} in SubThread {SubThreadId} by {RequestingUserId}.")]
    private partial void LogUnbanSuccess(Guid targetUserId, Guid subThreadId, Guid requestingUserId);
}