using Threddit.Application.DTOs.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetAllSiteAdminsUseCase
{
    /// <summary>Gets all site admins with their issued ban data.</summary>
    /// <remarks>
    /// Only for use by the site owner.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetAllSiteAdminsResponse> ExecuteAsync();
}