using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class GetUserProfileUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to fetch user profile for '{Username}'. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, string username, string? errorMessage);
}