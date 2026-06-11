using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Users;

public sealed partial class BanSubThreadUserUseCase : IBanSubThreadUserUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BanSubThreadUserUseCase> _logger;

    public BanSubThreadUserUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<BanSubThreadUserUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<BanSubThreadUserResponse> ExecuteAsync(BanSubThreadUserRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new BanSubThreadUserResponse(false, message, subThreadResult.ErrorType);
        }

        var subThread = subThreadResult.Value!;
        
        var isModerator = request.ModeratedSubThreadIds.Contains(subThread.Id);
        var isSubThreadOwner = subThread.SubThreadOwner.UserId == request.RequestingUserId;

        if (!isModerator && !isSubThreadOwner)
        {
            LogUnauthorizedAttempt(request.RequestingUserId, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new BanSubThreadUserResponse(false, message, ErrorType.Forbidden);
        }

        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new BanSubThreadUserResponse(false, message, targetResult.ErrorType);
        }

        var bannerResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!bannerResult.IsSuccess)
        {
            LogUserFetchFailure(bannerResult.Exception, request.RequestingUserId, bannerResult.ErrorMessage);
            var message = ResolveErrorMessage(bannerResult.ErrorType);
            return new BanSubThreadUserResponse(false, message, bannerResult.ErrorType);
        }
        
        var target = targetResult.Value!;
        var banner = bannerResult.Value!;
        
        var existingBanResult = await _userRepository.GetSubThreadBanAsync(target.Id, subThread.Id);
        if (!existingBanResult.IsSuccess)
        {
            LogBanFetchFailure(existingBanResult.Exception, target.Id, subThread.Id, existingBanResult.ErrorMessage);
            var message = ResolveErrorMessage(existingBanResult.ErrorType);
            return new BanSubThreadUserResponse(false, message, existingBanResult.ErrorType);
        }

        if (existingBanResult.Value is not null)
        {
            var editResult = existingBanResult.Value.Edit(request.Reason, banner, request.ExpiresAt);
            if (!editResult.IsSuccess)
            {
                LogValidationFailure(target.Id, subThread.Id, editResult.ErrorMessage);
                var message = ResolveErrorMessage(editResult.ErrorType);
                return new BanSubThreadUserResponse(false, message, editResult.ErrorType);
            }

            var updateResult = await _userRepository.UpdateSubThreadBanAsync(existingBanResult.Value);
            if (!updateResult.IsSuccess)
            {
                LogSaveFailure(updateResult.Exception, target.Id, subThread.Id, updateResult.ErrorMessage);
                var message = ResolveErrorMessage(updateResult.ErrorType);
                return new BanSubThreadUserResponse(false, message, updateResult.ErrorType);
            }
            
            LogBanUpdated(target.Id, subThread.Id, banner.Id);
            return new BanSubThreadUserResponse(true, "SubThread ban updated successfully.");
        }

        var createResult = BannedSubThreadUser.Create(target, banner, subThread, request.Reason, request.ExpiresAt);
        if (!createResult.IsSuccess)
        {
            LogValidationFailure(target.Id, subThread.Id, createResult.ErrorMessage);
            var message = ResolveErrorMessage(createResult.ErrorType);
            return new BanSubThreadUserResponse(false, message, createResult.ErrorType);
        }

        var saveResult = await _userRepository.AddSubThreadBanAsync(createResult.Value!);
        
        LogBanIssued(target.Id, subThread.Id, banner.Id);
        return new BanSubThreadUserResponse(true, "User successfully banned from SubThread.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.Forbidden => "You are not authorized to ban users in this SubThread.",
        ErrorType.BanReasonEmpty => "Ban reason cannot be empty.",
        ErrorType.BanReasonTooLong => $"Ban reason cannot exceed {BannedSubThreadUser.Limits.MaxReasonLength}.",
        ErrorType.InvalidBanDate => "Expiry date cannot be in the past.",
        ErrorType.ConcurrencyFailure => "Failed to save ban due to internal server error. Please try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}