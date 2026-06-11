using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetSiteReportsUseCase
{
    /// <summary>Gets paginated site-wide reports. Only an admin or the site owner may access this.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetReportsResponse> ExecuteAsync(GetSiteReportsRequest request);
}