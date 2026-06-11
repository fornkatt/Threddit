using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class RemoveSiteAdminUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch target user {UserId} for site admin removal - repository returned: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch site admin record for user {UserId} - repository returned: {ErrorMessage}")]
    private partial void LogSiteAdminFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Attempted to remove site admin from user {TargetUserId} but no admin record was found.")]
    private partial void LogNotAdmin(Guid targetUserId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to remove site admin record for user {UserId} - repository returned: {ErrorMessage}")]
    private partial void LogRemoveFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Site admin status removed from user {TargetUserId}.")]
    private partial void LogRemoveSuccess(Guid targetUserId);
}