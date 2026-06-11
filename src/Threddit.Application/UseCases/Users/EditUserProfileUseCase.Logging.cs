using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class EditUserProfileUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for profile edit. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Validation failed while editing profile for user {UserId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update profile for user {UserId} in database. Error: {ErrorMessage}")]
    private partial void LogUpdateFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Profile updated successfully for user {UserId}.")]
    private partial void LogEditSuccess(Guid userId);
}