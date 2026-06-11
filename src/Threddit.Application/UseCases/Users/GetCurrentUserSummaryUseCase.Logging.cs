using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class GetCurrentUserSummaryUseCase
{
        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Failed to fetch user {UserId} for fetching user summary - repository returned: {ErrorMessage}")]
        private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);
}