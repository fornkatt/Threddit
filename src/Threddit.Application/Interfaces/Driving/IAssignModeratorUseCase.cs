using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IAssignModeratorUseCase
{
    /// <summary>Assigns a user as a moderator to a SubThread.</summary>
    /// <remarks>
    /// Only the SubThread owner may assign moderators.
    /// If the user is already a moderator of this SubThread, it returns early.
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.AlreadyAssigned"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<AssignModeratorResponse> ExecuteAsync(AssignModeratorRequest request);
}