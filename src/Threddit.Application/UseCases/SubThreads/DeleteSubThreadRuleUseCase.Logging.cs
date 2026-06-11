using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class DeleteSubThreadRuleUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Rule {RuleId} not found for deletion. Error: {ErrorMessage}")]
    private partial void LogFetchFailure(Exception? ex, Guid ruleId, string? errorMessage);
    
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "SubThread {SubThreadId} not found for deletion. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} attempted to delete rule {RuleId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid userId, Guid ruleId);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to delete rule {RuleId}. Error: {ErrorMessage}")]
    private partial void LogDeleteFailure(Exception? ex, Guid ruleId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Rule {RuleId} deleted successfully by user with ID {UserId}.")]
    private partial void LogDeleteSuccess(Guid ruleId, Guid userId);
}