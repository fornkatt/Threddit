using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetUserProfileUseCase
{
    /// <summary>Gets the public profile of a user by username.</summary>
    /// <remarks>
    /// This is for publicly available information, no authentication is required.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetUserProfileResponse> ExecuteAsync(GetUserProfileRequest request);
}