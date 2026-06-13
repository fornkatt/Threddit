using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class EditSubThreadRuleUseCase : IEditSubThreadRuleUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EditSubThreadRuleUseCase> _logger;

    public EditSubThreadRuleUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<EditSubThreadRuleUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<EditSubThreadRuleResponse> ExecuteAsync(EditSubThreadRuleRequest request)
    {
        var ruleResult = await _subThreadRepository.GetRuleByIdAsync(request.RuleId);
        if (!ruleResult.IsSuccess)
        {
            LogRuleFetchFailure(ruleResult.Exception, request.RuleId, ruleResult.ErrorMessage);
            var message = ResolveErrorMessage(ruleResult.ErrorType);
            return new EditSubThreadRuleResponse(false, message, ruleResult.ErrorType);
        }

        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new EditSubThreadRuleResponse(false, message, userResult.ErrorType);
        }

        var rule = ruleResult.Value!;
        var user = userResult.Value!;
        
        var subThreadResult = await _subThreadRepository.GetByIdAsync(rule.SubThreadId);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, rule.SubThreadId, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new EditSubThreadRuleResponse(false, message, subThreadResult.ErrorType);
        }
        
        var subThread = subThreadResult.Value!;
        
        var isModerator = request.ModeratedSubThreadIds.Contains(request.SubThreadId);
        var isSubThreadOwner = request.OwnedSubThreadIds.Contains(subThread.Id);

        if (!isSubThreadOwner && !isModerator)
        {
            LogUnauthorizedAttempt(user.Id, rule.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new EditSubThreadRuleResponse(false, message, ErrorType.Forbidden);
        }
        
        var editResult = rule.Edit(request.RuleTitle, request.RuleContent, user);
        if (!editResult.IsSuccess)
        {
            LogValidationFailure(rule.Id, editResult.ErrorMessage);
            var message = ResolveErrorMessage(editResult.ErrorType);
            return new EditSubThreadRuleResponse(false, message, editResult.ErrorType);
        }

        var updateResult = await _subThreadRepository.UpdateRuleAsync(rule);
        if (!updateResult.IsSuccess)
        {
            LogUpdateFailure(updateResult.Exception, rule.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new EditSubThreadRuleResponse(false, message, updateResult.ErrorType);
        }
        
        LogEditSuccess(rule.Id, user.UserName!, user.Id);
        return new EditSubThreadRuleResponse(true, "Rule updated successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadRuleNotFound => "Rule not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.Forbidden => "You are not authorized to edit this rule.",
        ErrorType.TitleEmpty => "Rule title cannot be empty.",
        ErrorType.ContentEmpty => "Rule content cannot be empty.",
        ErrorType.TitleTooLong => $"Rule title cannot exceed {SubThreadRule.Limits.MaxTitleLength} characters.",
        ErrorType.ContentTooLong => $"Rule content cannot exceed {SubThreadRule.Limits.MaxContentLength} characters.",
        ErrorType.ConcurrencyFailure => "SubThread rule may have been edited by a different user. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}