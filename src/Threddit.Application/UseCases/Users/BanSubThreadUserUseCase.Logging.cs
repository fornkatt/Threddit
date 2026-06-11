using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class BanSubThreadUserUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread '{SubThreadName}' for SubThread ban. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {RequestingUserId} attempted to ban a user in SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid requestingUserId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for SubThread ban. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message =
            "Failed to fetch existing SubThread ban for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogBanFetchFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Validation failed while banning user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to save SubThread ban for user {UserId} in SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid userId, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "SubThread ban issued for user {TargetUserId} in SubThread {SubThreadId} by {BannedById}.")]
    private partial void LogBanIssued(Guid targetUserId, Guid subThreadId, Guid bannedById);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "SubThread ban updated for user {TargetUserId} in SubThread {SubThreadId} by {BannedById}.")]
    private partial void LogBanUpdated(Guid targetUserId, Guid subThreadId, Guid bannedById);
}