using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IEditUserProfileUseCase
{
    /// <summary>Edits the profile picture and/or description of the authenticated user.</summary>
    /// <remarks>
    /// Users may only edit their own profile.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<EditUserProfileResponse> ExecuteAsync(EditUserProfileRequest request);
}