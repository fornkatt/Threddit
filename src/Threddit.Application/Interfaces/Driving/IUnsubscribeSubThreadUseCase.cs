using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IUnsubscribeSubThreadUseCase
{
    /// <summary>Unsubscribes a user from a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.NotSubscribed"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<UnsubscribeSubThreadResponse> ExecuteAsync(UnsubscribeSubThreadRequest request);
}