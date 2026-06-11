using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetCurrentUserSummaryUseCase
{
    /// <summary>Gets the current authenticated user's summary for the client-side store.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetCurrentUserSummaryResponse> ExecuteAsync(GetCurrentUserSummaryRequest request);
}