using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class AssignSiteAdminUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message =
            "Failed to fetch target user {UserId} for site admin assignment - repository returned: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch existing site admin record for user {UserId} - repository returned: {ErrorMessage}")]
    private partial void LogSiteAdminFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User {TargetUserId} is already a site administrator.")]
    private partial void LogAlreadyAdmin(Guid targetUserId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to save site admin assignment for user {UserId} - repository returned: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {TargetUserId} assigned as site administrator.")]
    private partial void LogAssignSuccess(Guid targetUserId);
}