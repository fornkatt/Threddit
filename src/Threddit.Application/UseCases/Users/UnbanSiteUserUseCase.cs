using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Users;

public sealed partial class UnbanSiteUserUseCase : IUnbanSiteUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UnbanSiteUserUseCase> _logger;

    public UnbanSiteUserUseCase(
        IUserRepository userRepository,
        ILogger<UnbanSiteUserUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UnbanSiteUserResponse> ExecuteAsync(UnbanSiteUserRequest request)
    {
        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new UnbanSiteUserResponse(false, message, targetResult.ErrorType);
        }
        
        var target = targetResult.Value!;
        
        var banResult = await _userRepository.GetSiteBanAsync(target.Id);
        if (!banResult.IsSuccess)
        {
            LogBanFetchFailure(banResult.Exception, target.Id, banResult.ErrorMessage);
            var message = ResolveErrorMessage(banResult.ErrorType);
            return new UnbanSiteUserResponse(false, message, banResult.ErrorType);
        }

        if (banResult.Value is null)
        {
            LogNoBanFound(target.Id);
            var message = ResolveErrorMessage(ErrorType.NotFound);
            return new UnbanSiteUserResponse(false, message, ErrorType.NotFound);
        }

        var removeResult = await _userRepository.RemoveSiteBanAsync(banResult.Value);
        if (!removeResult.IsSuccess)
        {
            LogRemoveFailure(removeResult.Exception, target.Id, removeResult.ErrorMessage);
            var message = ResolveErrorMessage(removeResult.ErrorType);
            return new UnbanSiteUserResponse(false, message, removeResult.ErrorType);
        }
        
        LogUnbanSuccess(target.Id, request.RequestingUserId);
        return new UnbanSiteUserResponse(true, "Site ban removed successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.NotFound => "This user does not have an active site ban.",
        ErrorType.ConcurrencyFailure => "Failed to save ban due to internal server error. Please try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}