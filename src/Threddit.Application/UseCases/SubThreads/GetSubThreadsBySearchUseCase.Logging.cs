using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadsBySearchUseCase
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to search for subthreads with query '{Query}'. Error: {ErrorMessage}")]
    private partial void LogSearchFailure(Exception? ex, string query, string? errorMessage);
}