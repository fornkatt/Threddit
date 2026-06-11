using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class CreateSubThreadRuleUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "SubThread '{SubThreadName}' not found for creating rule. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, string subThreadName, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} not found when creating rule. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} attempted to create a rule for SubThread {SubThreadId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid userId, Guid subThreadId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Validation failed while creating rule for SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to save rule for SubThread {SubThreadId}. Error: {ErrorMessage}")]
    private partial void LogSaveFailure(Exception? ex, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Rule {RuleId} created for SubThread {SubThreadId} by user {Username} with ID {UserId}.")]
    private partial void LogCreateSuccess(Guid ruleId, Guid subThreadId, string username, Guid userId);
}