using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IAssignSiteAdminUseCase
{
    /// <summary>Assigns a user as a site administrator.</summary>
    /// <remarks>
    /// Only the site owner may assign site administrators.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.AlreadyAssigned"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<AssignSiteAdminResponse> ExecuteAsync(AssignSiteAdminRequest request);
}