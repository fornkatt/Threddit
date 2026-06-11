using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class DeleteSubThreadRuleUseCase : IDeleteSubThreadRuleUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteSubThreadRuleUseCase> _logger;

    public DeleteSubThreadRuleUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<DeleteSubThreadRuleUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<DeleteSubThreadRuleResponse> ExecuteAsync(DeleteSubThreadRuleRequest request)
    {
        var ruleResult = await _subThreadRepository.GetRuleByIdAsync(request.RuleId);
        if (!ruleResult.IsSuccess)
        {
            LogFetchFailure(ruleResult.Exception, request.RuleId, ruleResult.ErrorMessage);
            var message = ResolveErrorMessage(ruleResult.ErrorType);
            return new DeleteSubThreadRuleResponse(false, message, ruleResult.ErrorType);
        }

        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogFetchFailure(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new DeleteSubThreadRuleResponse(false, message, userResult.ErrorType);
        }

        var rule = ruleResult.Value!;
        
        var subThreadResult = await _subThreadRepository.GetByIdAsync(rule.SubThreadId);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, rule.SubThreadId, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new DeleteSubThreadRuleResponse(false, message, subThreadResult.ErrorType);
        }
        
        var subThread = subThreadResult.Value!;
        
        var isModerator = request.ModeratedSubThreadIds.Contains(request.SubThreadId);
        var isSubThreadOwner = subThread.SubThreadOwner.UserId == request.RequestingUserId;

        if (!isSubThreadOwner && !isModerator)
        {
            LogUnauthorizedAttempt(request.RequestingUserId, rule.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new DeleteSubThreadRuleResponse(false, message, ErrorType.Forbidden);
        }

        var deleteResult = await _subThreadRepository.DeleteRuleAsync(rule);
        if (!deleteResult.IsSuccess)
        {
            LogDeleteFailure(deleteResult.Exception, rule.Id, deleteResult.ErrorMessage);
            var message = ResolveErrorMessage(deleteResult.ErrorType);
            return new DeleteSubThreadRuleResponse(false, message, deleteResult.ErrorType);
        }

        LogDeleteSuccess(rule.Id, request.RequestingUserId);
        return new DeleteSubThreadRuleResponse(true, "Rule deleted successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadRuleNotFound => "Rule not found.",
        ErrorType.Forbidden => "You are not authorized to delete this rule.",
        ErrorType.ConcurrencyFailure => "Failed to delete SubThread rule due to internal server error. Please refresh try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}