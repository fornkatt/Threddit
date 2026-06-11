using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class UnsubscribeSubThreadUseCase : IUnsubscribeSubThreadUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UnsubscribeSubThreadUseCase> _logger;

    public UnsubscribeSubThreadUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<UnsubscribeSubThreadUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UnsubscribeSubThreadResponse> ExecuteAsync(UnsubscribeSubThreadRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new UnsubscribeSubThreadResponse(false, message, subThreadResult.ErrorType);
        }

        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new UnsubscribeSubThreadResponse(false, message, userResult.ErrorType);
        }

        var subThread = subThreadResult.Value!;
        var user = userResult.Value!;

        var existingResult = await _subThreadRepository.GetSubscriptionAsync(subThread.Id, user.Id);
        if (!existingResult.IsSuccess)
        {
            LogSubscriptionFetchFailure(existingResult.Exception, subThread.Id, user.Id, existingResult.ErrorMessage);
            var message = ResolveErrorMessage(existingResult.ErrorType);
            return new UnsubscribeSubThreadResponse(false, message, existingResult.ErrorType);
        }

        if (existingResult.Value is null)
        {
            LogNotSubscribed(user.Id, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.NotSubscribed);
            return new UnsubscribeSubThreadResponse(false, message, ErrorType.NotSubscribed);
        }
        
        subThread.ApplySubscriberCountDelta(-1);

        var unsubscribeResult = await _subThreadRepository.UnsubscribeAsync(existingResult.Value, subThread);
        if (!unsubscribeResult.IsSuccess)
        {
            LogSaveFailure(unsubscribeResult.Exception, subThread.Id, user.Id, unsubscribeResult.ErrorMessage);
            var message = ResolveErrorMessage(unsubscribeResult.ErrorType);
            return new UnsubscribeSubThreadResponse(false, message, unsubscribeResult.ErrorType);
        }
        
        LogUnsubscribeSuccess(user.Id, subThread.Id);
        return new UnsubscribeSubThreadResponse(true, "Successfully unsubscribed from SubThread.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.NotSubscribed => "You are not subscribed to this SubThread.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        ErrorType.ConcurrencyFailure => "Failed to save subscription. Please refresh and try again.",
        _ => "An unexpected error occurred. Please try again later."
    };
}