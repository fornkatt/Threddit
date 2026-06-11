using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetSubThreadReportsUseCase
{
    /// <summary>Gets paginated reports for a SubThread. Only the SubThread owner and moderators for that specific
    /// SubThread may access this.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetReportsResponse> ExecuteAsync(GetSubThreadReportsRequest request);
}