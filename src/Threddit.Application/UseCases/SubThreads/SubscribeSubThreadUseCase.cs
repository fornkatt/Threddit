using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class SubscribeSubThreadUseCase : ISubscribeSubThreadUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SubscribeSubThreadUseCase> _logger;

    public SubscribeSubThreadUseCase(ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<SubscribeSubThreadUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<SubscribeSubThreadResponse> ExecuteAsync(SubscribeSubThreadRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new SubscribeSubThreadResponse(false, message, subThreadResult.ErrorType);
        }
        
        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new SubscribeSubThreadResponse(false, message, userResult.ErrorType);
        }
        
        var subThread = subThreadResult.Value!;
        var user = userResult.Value!;
        
        var existingResult = await  _subThreadRepository.GetSubscriptionAsync(subThread.Id, user.Id);
        if (existingResult.Value is not null)
        {
            LogAlreadySubscribed(existingResult.Exception, user.Id, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.AlreadySubscribed);
            return new SubscribeSubThreadResponse(false, message, ErrorType.AlreadySubscribed);
        }

        var subscription = SubThreadSubscription.Create(user, subThread);
        subThread.ApplySubscriberCountDelta(1);
        
        var saveResult = await _subThreadRepository.SubscribeAsync(subscription, subThread);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, subThread.Id, user.Id, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new SubscribeSubThreadResponse(false, message, saveResult.ErrorType);
        }
        
        LogSubscribeSuccess(user.Id, subThread.Id);
        return new SubscribeSubThreadResponse(true, "Successfully subscribed to SubThread.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.AlreadySubscribed => "You are already subscribed to this SubThread.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        ErrorType.ConcurrencyFailure => "Failed to save subscription. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}