using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class UnsubscribeSubThreadUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch SubThread {SubThreadName} for unsubscription. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch user {UserId} for unsubscription. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to fetch subscription for SubThread {SubThreadId} by user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSubscriptionFetchFailure(Exception? ex, Guid subThreadId, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} attempted to unsubscribe from SubThread {SubThreadId} but is not subscribed.")]
    private partial void LogNotSubscribed(Guid userId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Failed to remove subscription for SubThread {SubThreadId} by user {UserId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid subThreadId, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "User {UserId} unsubscribed from SubThread {SubThreadId}.")]
    private partial void LogUnsubscribeSuccess(Guid userId, Guid subThreadId);
}