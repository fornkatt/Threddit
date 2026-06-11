using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class EditSubThreadRuleUseCase
{
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Rule {RuleId} not found for editing. Error: {ErrorMessage}")]
    private partial void LogRuleFetchFailure(Exception? ex, Guid ruleId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} not found when editing rule. Error: {ErrorMessage}")]
    private partial void LogUserFetchFailure(Exception? ex, Guid userId, string? errorMessage);
    
    [LoggerMessage(Level = LogLevel.Warning,
        Message = "SubThread {SubThreadId} not found when editing rule. Error: {ErrorMessage}")]
    private partial void LogSubThreadFetchFailure(Exception? ex, Guid subThreadId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "User {UserId} attempted to edit rule {RuleId} without authorization.")]
    private partial void LogUnauthorizedAttempt(Guid userId, Guid ruleId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Validation failed while editing rule {RuleId}. Error: {ErrorMessage}")]
    private partial void LogValidationFailure(Guid ruleId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Failed to update rule {RuleId} in database. Error: {ErrorMessage}")]
    private partial void LogUpdateFailure(Exception? ex, Guid ruleId, string? errorMessage);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Rule {RuleId} updated successfully by user {Username} with ID {UserId}.")]
    private partial void LogEditSuccess(Guid ruleId, string username, Guid userId);
}