using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IBanSiteUserUseCase
{
    /// <summary>Issues or updates a site-wide ban for a user.</summary>
    /// <remarks>
    /// Only a site admin or the site owner may issue site-wide bans.
    /// If the user already has an active ban, it will be updated with a new reason and expiry.
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.BanReasonEmpty"/></item>
    ///     <item><see cref="ErrorType.BanReasonTooLong"/></item>
    ///     <item><see cref="ErrorType.InvalidBanDate"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<BanSiteUserResponse> ExecuteAsync(BanSiteUserRequest request);
}