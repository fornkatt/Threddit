using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetSubThreadModeratorsUseCase
{
    /// <summary>Gets all moderators of a SubThread with their public user details.</summary>
    /// <remarks>
    /// This is a public endpoint — no authentication required.
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetSubThreadModeratorsResponse> ExecuteAsync(GetSubThreadModeratorsRequest request);
}