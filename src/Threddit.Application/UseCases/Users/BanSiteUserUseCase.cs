using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Users;

public sealed partial class BanSiteUserUseCase : IBanSiteUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BanSiteUserUseCase> _logger;

    public BanSiteUserUseCase(
        IUserRepository userRepository,
        ILogger<BanSiteUserUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<BanSiteUserResponse> ExecuteAsync(BanSiteUserRequest request)
    {
        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new BanSiteUserResponse(false, message, targetResult.ErrorType);
        }

        var bannerResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!bannerResult.IsSuccess)
        {
            LogUserFetchFailure(bannerResult.Exception, request.RequestingUserId, bannerResult.ErrorMessage);
            var message = ResolveErrorMessage(bannerResult.ErrorType);
            return new BanSiteUserResponse(false, message, bannerResult.ErrorType);
        }

        var target = targetResult.Value!;
        var banner = bannerResult.Value!;

        var existingBanResult = await _userRepository.GetSiteBanAsync(target.Id);
        if (!existingBanResult.IsSuccess)
        {
            LogBanFetchFailure(existingBanResult.Exception, target.Id, existingBanResult.ErrorMessage);
            var message = ResolveErrorMessage(existingBanResult.ErrorType);
            return new BanSiteUserResponse(false, message, existingBanResult.ErrorType);
        }

        if (existingBanResult.Value is not null)
        {
            var editResult = existingBanResult.Value.Edit(request.Reason, banner, request.ExpiresAt);
            if (!editResult.IsSuccess)
            {
                LogValidationFailure(target.Id, editResult.ErrorMessage);
                var message = ResolveErrorMessage(editResult.ErrorType);
                return new BanSiteUserResponse(false, message, editResult.ErrorType);
            }

            var updateResult = await _userRepository.UpdateSiteBanAsync(existingBanResult.Value);
            if (!updateResult.IsSuccess)
            {
                LogSaveFailure(updateResult.Exception, target.Id, updateResult.ErrorMessage);
                var message = ResolveErrorMessage(updateResult.ErrorType);
                return new BanSiteUserResponse(false, message, updateResult.ErrorType);
            }
            
            LogBanUpdated(target.UserName!, target.Id, banner.UserName!, banner.Id);
            return new BanSiteUserResponse(true, "Site ban updated successfully.");
        }

        var createResult = BannedSiteUser.Create(target, banner, request.Reason, request.ExpiresAt);
        if (!createResult.IsSuccess)
        {
            LogValidationFailure(target.Id, createResult.ErrorMessage);
            var message = ResolveErrorMessage(createResult.ErrorType);
            return new BanSiteUserResponse(false, message, createResult.ErrorType);
        }

        var saveResult = await _userRepository.AddSiteBanAsync(createResult.Value!);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, target.Id, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new BanSiteUserResponse(false, message, saveResult.ErrorType);
        }
        
        LogBanIssued(target.UserName!, target.Id, banner.UserName!, banner.Id);
        return new BanSiteUserResponse(true, "User has been successfully banned from the site.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.BanReasonEmpty => "Ban reason cannot be empty.",
        ErrorType.BanReasonTooLong => $"Ban reason cannot exceed {BannedSiteUser.Limits.MaxReasonLength} characters.",
        ErrorType.InvalidBanDate => "Expiry date cannot be in the past.",
        ErrorType.ConcurrencyFailure => "Failed to save ban due to internal server error. Please try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}