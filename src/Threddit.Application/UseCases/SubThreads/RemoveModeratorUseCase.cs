using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class RemoveModeratorUseCase : IRemoveModeratorUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RemoveModeratorUseCase> _logger;

    public RemoveModeratorUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<RemoveModeratorUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<RemoveModeratorResponse> ExecuteAsync(RemoveModeratorRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new RemoveModeratorResponse(false, message, subThreadResult.ErrorType);
        }
        var subThread = subThreadResult.Value!;

        if (subThread.SubThreadOwner.Id != request.RequestingUserId)
        {
            LogUnauthorizedAttempt(request.RequestingUserId, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new RemoveModeratorResponse(false, message, ErrorType.Forbidden);
        }

        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new RemoveModeratorResponse(false, message, targetResult.ErrorType);
        }
        var target = targetResult.Value!;
        
        var moderatorResult = await _subThreadRepository.GetModeratorAsync(subThread.Id, target.Id);
        if (!moderatorResult.IsSuccess)
        {
            LogModeratorFetchFailure(moderatorResult.Exception, target.Id, subThread.Id, moderatorResult.ErrorMessage);
            var message = ResolveErrorMessage(moderatorResult.ErrorType);
            return new RemoveModeratorResponse(false, message, moderatorResult.ErrorType);
        }
        if (moderatorResult.Value is null)
        {
            LogNotModerator(target.Id, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.NotFound);
            return new RemoveModeratorResponse(false, message, ErrorType.NotFound);
        }
        var moderator = moderatorResult.Value;

        var removeResult = await _subThreadRepository.RemoveModeratorAsync(moderator);
        if (!removeResult.IsSuccess)
        {
            LogRemoveFailure(removeResult.Exception, moderator.Id, subThread.Id, removeResult.ErrorMessage);
            var message = ResolveErrorMessage(removeResult.ErrorType);
            return new RemoveModeratorResponse(false, message, removeResult.ErrorType);
        }
        
        LogRemoveSuccess(target.Id, subThread.Id, request.RequestingUserId);
        return new RemoveModeratorResponse(true, "Moderator removed successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.Forbidden => "Only the SubThread owner may remove moderators.",
        ErrorType.NotFound => "This user is not a moderator of this SubThread.",
        ErrorType.ConcurrencyFailure => "Failed to remove moderator due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}