using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Users;

public sealed partial class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileUseCase> _logger;

    public GetUserProfileUseCase(
        IUserRepository userRepository,
        ILogger<GetUserProfileUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUserProfileResponse> ExecuteAsync(GetUserProfileRequest request)
    {
        var userResult = await _userRepository.GetByUsernameAsync(request.Username);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.Username, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new GetUserProfileResponse(false, message, userResult.ErrorType);
        }
        var user = userResult.Value!;

        var dto = new UserProfileDto(
            user.Id,
            user.UserName!,
            user.ProfilePicture,
            user.Description,
            user.PostScore,
            user.CommentScore,
            user.TotalScore,
            user.CreationDate
        );

        return new GetUserProfileResponse(true, User: dto);
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}