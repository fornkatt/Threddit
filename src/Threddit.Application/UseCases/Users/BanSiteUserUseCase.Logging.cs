using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class BanSiteUserUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for site ban. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch existing site ban for user {UserId}. Error: {ErrorMessage}")]
    private partial void LogBanFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Validation failed while banning user {UserId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to save site ban for user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Site ban issued for user {BannedUsername} with ID {TargetUserId} by {BannedByUsername} with ID {BannedById}.")]
    private partial void LogBanIssued(string bannedUsername, Guid targetUserId, string bannedByUsername, Guid bannedById);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Site ban updated for user {BannedUsername} with ID {TargetUserId} by {BannedByUsername} with ID {BannedById}.")]
    private partial void LogBanUpdated(string bannedUsername, Guid targetUserId, string bannedByUsername, Guid bannedById);
}