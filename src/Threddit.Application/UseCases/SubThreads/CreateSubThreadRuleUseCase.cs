using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class CreateSubThreadRuleUseCase : ICreateSubThreadRuleUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateSubThreadRuleUseCase> _logger;

    public CreateSubThreadRuleUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<CreateSubThreadRuleUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreateSubThreadRuleResponse> ExecuteAsync(CreateSubThreadRuleRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new CreateSubThreadRuleResponse(false, Message: message, ErrorType: subThreadResult.ErrorType);
        }

        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new CreateSubThreadRuleResponse(false, Message: message, ErrorType: userResult.ErrorType);
        }

        var subThread = subThreadResult.Value!;
        var user = userResult.Value!;

        var isModerator = request.ModeratedSubThreadIds.Contains(subThread.Id);
        var isSubThreadOwner = request.OwnedSubThreadIds.Contains(subThread.Id);

        if (!isModerator && !isSubThreadOwner)
        {
            LogUnauthorizedAttempt(user.Id, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new CreateSubThreadRuleResponse(false, Message: message, ErrorType: ErrorType.Forbidden);
        }

        var createResult = SubThreadRule.Create(subThread, request.RuleTitle, request.RuleContent, user);
        if (!createResult.IsSuccess)
        {
            LogValidationFailure(subThread.Id, createResult.ErrorMessage);
            var message = ResolveErrorMessage(createResult.ErrorType);
            return new CreateSubThreadRuleResponse(false, Message: message, ErrorType: createResult.ErrorType);
        }

        var saveResult = await _subThreadRepository.AddRuleAsync(createResult.Value!);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, subThread.Id, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new CreateSubThreadRuleResponse(false, Message: message, ErrorType: saveResult.ErrorType);
        }

        LogCreateSuccess(saveResult.Value!.Id, subThread.Id, user.UserName!, user.Id);
        return new CreateSubThreadRuleResponse(true, Message: "Rule created successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.Forbidden => "You are not authorized to manage rules for this SubThread.",
        ErrorType.TitleEmpty => "Rule title cannot be empty.",
        ErrorType.ContentEmpty => "Rule content cannot be empty.",
        ErrorType.TitleTooLong => $"Rule title cannot exceed {SubThreadRule.Limits.MaxTitleLength} characters.",
        ErrorType.ContentTooLong => $"Rule content cannot exceed {SubThreadRule.Limits.MaxContentLength} characters.",
        ErrorType.ConcurrencyFailure => "SubThread rule creation failed due to internal server error. Please refresh try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}