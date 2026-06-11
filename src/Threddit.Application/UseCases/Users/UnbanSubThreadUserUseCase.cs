using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Users;

public sealed partial class UnbanSubThreadUserUseCase : IUnbanSubThreadUserUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UnbanSubThreadUserUseCase> _logger;

    public UnbanSubThreadUserUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<UnbanSubThreadUserUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UnbanSubThreadUserResponse> ExecuteAsync(UnbanSubThreadUserRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new UnbanSubThreadUserResponse(false, message, subThreadResult.ErrorType);
        }

        var subThread = subThreadResult.Value!;
        
        var isModerator = request.ModeratedSubThreadIds.Contains(subThread.Id);
        var isSubThreadOwner = subThread.SubThreadOwner.UserId == request.RequestingUserId;

        if (!isModerator && !isSubThreadOwner)
        {
            LogUnauthorizedAttempt(request.RequestingUserId, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new UnbanSubThreadUserResponse(false, message, ErrorType.Forbidden);
        }

        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new UnbanSubThreadUserResponse(false, message, targetResult.ErrorType);
        }
        
        var target = targetResult.Value!;
        
        var banResult = await _userRepository.GetSubThreadBanAsync(target.Id, subThread.Id);
        if (!banResult.IsSuccess)
        {
            LogBanFetchFailure(banResult.Exception, target.Id, subThread.Id, banResult.ErrorMessage);
            var message = ResolveErrorMessage(banResult.ErrorType);
            return new UnbanSubThreadUserResponse(false, message, banResult.ErrorType);
        }

        if (banResult.Value is null)
        {
            LogNoBanFound(target.Id, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.NotFound);
            return new UnbanSubThreadUserResponse(false, message, ErrorType.NotFound);
        }

        var removeResult = await _userRepository.RemoveSubThreadBanAsync(banResult.Value);
        if (!removeResult.IsSuccess)
        {
            LogRemoveFailure(removeResult.Exception, target.Id, subThread.Id, removeResult.ErrorMessage);
            var message = ResolveErrorMessage(removeResult.ErrorType);
            return new UnbanSubThreadUserResponse(false, message, removeResult.ErrorType);
        }
        
        LogUnbanSuccess(target.Id, subThread.Id, request.RequestingUserId);
        return new UnbanSubThreadUserResponse(true, "SubThread ban removed successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.Forbidden => "You are not authorized to unban users in this SubThread.",
        ErrorType.NotFound => "This user does not have an active ban in this SubThread.",
        ErrorType.ConcurrencyFailure => "Failed to save ban due to internal server error. Please try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}