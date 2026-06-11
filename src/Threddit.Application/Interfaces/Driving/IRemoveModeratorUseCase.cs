using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IRemoveModeratorUseCase
{
    /// <summary>Removes a moderator from a SubThread.</summary>
    /// <remarks>
    /// Only the SubThread owner may remove moderators.
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.NotFound"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<RemoveModeratorResponse> ExecuteAsync(RemoveModeratorRequest request);
}