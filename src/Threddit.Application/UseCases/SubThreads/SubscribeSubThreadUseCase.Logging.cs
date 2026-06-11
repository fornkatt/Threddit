using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class SubscribeSubThreadUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread {SubThreadName} for subscription. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for subscription. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch existing subscription for SubThread {SubThreadId} by user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSubscriptionFetchFailure(Exception? ex, Guid subThreadId, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} is already subscribed to SubThread {SubThreadId}.")]
    private partial void LogAlreadySubscribed(Exception? ex, Guid userId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to save subscription for SubThread {SubThreadId} by user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid subThreadId, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "User {UserId} subscribed to SubThread {SubThreadId}.")]
    private partial void LogSubscribeSuccess(Guid userId, Guid subThreadId);
}