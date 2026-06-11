using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadByNameUseCase
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "SubThread not found with name '{Name}'. Error: {ErrorMessage}")]
    private partial void LogNotFound(Exception? ex, string name, string? errorMessage);
}