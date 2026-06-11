using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class AssignModeratorUseCase : IAssignModeratorUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AssignModeratorUseCase> _logger;

    public AssignModeratorUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<AssignModeratorUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<AssignModeratorResponse> ExecuteAsync(AssignModeratorRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new AssignModeratorResponse(false, message, subThreadResult.ErrorType);
        }
        var subThread = subThreadResult.Value!;

        if (subThread.SubThreadOwner.UserId != request.RequestingUserId)
        {
            LogUnauthorizedAttempt(request.RequestingUserId, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new AssignModeratorResponse(false, message, ErrorType.Forbidden);
        }
        
        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new AssignModeratorResponse(false, message, targetResult.ErrorType);
        }
        var target = targetResult.Value!;

        var existingResult = await _subThreadRepository.GetModeratorAsync(subThread.Id, target.Id);
        if (!existingResult.IsSuccess)
        {
            LogModeratorFetchFailure(existingResult.Exception, target.Id, subThread.Id, existingResult.ErrorMessage);
            var message = ResolveErrorMessage(existingResult.ErrorType);
            return new AssignModeratorResponse(false, message, existingResult.ErrorType);
        }

        if (existingResult.Value is not null)
        {
            LogAlreadyModerator(target.Id, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.AlreadyAssigned);
            return new AssignModeratorResponse(false, message, ErrorType.AlreadyAssigned);
        }

        var moderator = SubThreadModerator.Assign(target, subThread);

        var saveResult = await _subThreadRepository.AddModeratorAsync(moderator);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, target.Id, subThread.Id, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new AssignModeratorResponse(false, message, saveResult.ErrorType);
        }
        
        LogAssignSuccess(target.Id, subThread.Id, request.RequestingUserId);
        return new AssignModeratorResponse(true, "Moderator assigned successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.Forbidden => "Only the SubThread owner may assign moderators.",
        ErrorType.AlreadyAssigned => "This user is already a moderator of this SubThread.",
        ErrorType.ConcurrencyFailure => "Failed to assign moderator due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}