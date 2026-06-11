using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IBanSubThreadUserUseCase
{
    /// <summary>Issues or updates a SubThread ban for a user.</summary>
    /// <remarks>
    /// Only the SubThread owner or a moderator of the SubThread may issue SubThread bans.
    /// If the user already has an active ban in the SubThread, it will be updated.
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.BanReasonEmpty"/></item>
    ///     <item><see cref="ErrorType.BanReasonTooLong"/></item>
    ///     <item><see cref="ErrorType.InvalidBanDate"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<BanSubThreadUserResponse> ExecuteAsync(BanSubThreadUserRequest request);
}