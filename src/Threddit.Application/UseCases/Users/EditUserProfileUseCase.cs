using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Users;

public sealed partial class EditUserProfileUseCase : IEditUserProfileUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EditUserProfileUseCase> _logger;

    public EditUserProfileUseCase(
        IUserRepository userRepository,
        ILogger<EditUserProfileUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<EditUserProfileResponse> ExecuteAsync(EditUserProfileRequest request)
    {
        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new EditUserProfileResponse(false, message, userResult.ErrorType);
        }
        var user = userResult.Value!;

        var editResult = user.Edit(request.ProfilePicture, request.Description);
        if (!editResult.IsSuccess)
        {
            LogValidationFailure(user.Id, editResult.ErrorMessage);
            var message = ResolveErrorMessage(editResult.ErrorType);
            return new EditUserProfileResponse(false, message, editResult.ErrorType);
        }

        var updateResult = await _userRepository.UpdateAsync(user);
        if (!updateResult.IsSuccess)
        {
            LogUpdateFailure(updateResult.Exception, user.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new EditUserProfileResponse(false, message, updateResult.ErrorType);
        }
        
        LogEditSuccess(user.Id);
        return new EditUserProfileResponse(true, "Profile updated successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.ImageUrlTooLong =>
            $"Profile picture URL cannot exceed {User.Limits.MaxProfilePictureUrlLength} characters.",
        ErrorType.ContentTooLong => $"Description cannot exceed {User.Limits.MaxDescriptionLength} characters.",
        ErrorType.ConcurrencyFailure =>
            "Failed to update user profile due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}