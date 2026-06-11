using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Users;

public sealed partial class GetAllSiteAdminsUseCase
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to fetch site administrators - repository returned: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, string? errorMessage);
}