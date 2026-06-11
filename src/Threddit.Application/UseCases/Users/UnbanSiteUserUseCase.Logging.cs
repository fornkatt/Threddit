using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class UnbanSiteUserUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for site unban. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch site ban for user {UserId}. Error: {ErrorMessage}")]
    private partial void LogBanFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Attempted to unban user {UserId} but no active site ban was found.")]
    private partial void LogNoBanFound(Guid userId);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to remove site ban for user {UserId}. Error: {ErrorMessage}")]
    private partial void LogRemoveFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Site ban removed for user {TargetUserId} by {RequestingUserId}.")]
    private partial void LogUnbanSuccess(Guid targetUserId, Guid requestingUserId);
}