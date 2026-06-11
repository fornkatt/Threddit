using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Users;

public sealed partial class AssignSiteAdminUseCase : IAssignSiteAdminUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AssignSiteAdminUseCase> _logger;

    public AssignSiteAdminUseCase(
        IUserRepository userRepository,
        ILogger<AssignSiteAdminUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<AssignSiteAdminResponse> ExecuteAsync(AssignSiteAdminRequest request)
    {
        var targetResult = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (!targetResult.IsSuccess)
        {
            LogUserFetchFailure(targetResult.Exception, request.TargetUserId, targetResult.ErrorMessage);
            var message = ResolveErrorMessage(targetResult.ErrorType);
            return new AssignSiteAdminResponse(false, message, targetResult.ErrorType);
        }

        var target = targetResult.Value!;

        var existingResult = await _userRepository.GetSiteAdminAsync(target.Id);
        if (!existingResult.IsSuccess)
        {
            LogSiteAdminFetchFailure(existingResult.Exception, target.Id, existingResult.ErrorMessage);
            var message = ResolveErrorMessage(existingResult.ErrorType);
            return new AssignSiteAdminResponse(false, message, existingResult.ErrorType);
        }

        if (existingResult.Value is not null)
        {
            LogAlreadyAdmin(target.Id);
            var message = ResolveErrorMessage(ErrorType.AlreadyAssigned);
            return new AssignSiteAdminResponse(false, message, ErrorType.AlreadyAssigned);
        }

        var siteAdmin = SiteAdmin.Assign(target);

        var saveResult = await _userRepository.AddSiteAdminAsync(siteAdmin);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, siteAdmin.Id, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new AssignSiteAdminResponse(false, message, saveResult.ErrorType);
        }

        LogAssignSuccess(target.Id);
        return new AssignSiteAdminResponse(true, "Site administrator assignment successful.");
    }


    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.AlreadyAssigned => "This user is already a site administrator.",
        ErrorType.ConcurrencyFailure =>
            "Failed to save assignment due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}