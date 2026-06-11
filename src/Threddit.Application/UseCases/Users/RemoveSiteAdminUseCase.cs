using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Users;

public sealed partial class RemoveSiteAdminUseCase : IRemoveSiteAdminUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RemoveSiteAdminUseCase> _logger;

    public RemoveSiteAdminUseCase(
        IUserRepository userRepository,
        ILogger<RemoveSiteAdminUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<RemoveSiteAdminResponse> ExecuteAsync(RemoveSiteAdminRequest request)
    {
        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new RemoveSiteAdminResponse(false, message, targetResult.ErrorType);
        }
        var target = targetResult.Value!;

        var adminResult = await _userRepository.GetSiteAdminAsync(target.Id);
        if (!adminResult.IsSuccess)
        {
            LogSiteAdminFetchFailure(adminResult.Exception, target.Id, adminResult.ErrorMessage);
            var message = ResolveErrorMessage(adminResult.ErrorType);
            return new RemoveSiteAdminResponse(false, message, adminResult.ErrorType);
        }

        if (adminResult.Value is null)
        {
            LogNotAdmin(target.Id);
            var message = ResolveErrorMessage(ErrorType.NotFound);
            return new RemoveSiteAdminResponse(false, message, ErrorType.NotFound);
        }
        var admin = adminResult.Value!;

        var removeResult = await _userRepository.RemoveSiteAdminAsync(admin);
        if (!removeResult.IsSuccess)
        {
            LogRemoveFailure(removeResult.Exception, target.Id, removeResult.ErrorMessage);
            var message = ResolveErrorMessage(removeResult.ErrorType);
            return new RemoveSiteAdminResponse(false, message, removeResult.ErrorType);
        }
        
        LogRemoveSuccess(target.Id);
        return new RemoveSiteAdminResponse(true, "Site admin removed successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.SiteAdminNotFound => "This user is not a site administrator.",
        ErrorType.ConcurrencyFailure =>
            "Failed to save admin removal due to internal server error. Please refresh and try again. Please try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}