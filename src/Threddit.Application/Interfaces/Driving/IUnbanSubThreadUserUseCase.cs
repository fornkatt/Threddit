using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IUnbanSubThreadUserUseCase
{
    /// <summary>Removes a SubThread ban from a user.</summary>
    /// <remarks>
    /// Only the SubThread owner or a moderator of the SubThread may remove SubThread bans.
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.NotFound"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<UnbanSubThreadUserResponse> ExecuteAsync(UnbanSubThreadUserRequest request);
}