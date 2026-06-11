using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class GetSiteReportsUseCase
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Attempted to fetch site-wide reports - repository returned: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, string? errorMessage);
}