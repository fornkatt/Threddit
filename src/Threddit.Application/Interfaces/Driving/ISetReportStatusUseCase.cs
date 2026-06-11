using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface ISetReportStatusUseCase
{
    /// <summary>Sets the status of a report.</summary>
    /// <remarks>
    /// SubThread reports can be set by the SubThread owner or moderators.
    /// Site-wide reports can be set by an admin or the site owner.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ReportNotFound"/></item>
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.SameStatus"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<SetReportStatusResponse> ExecuteAsync(SetReportStatusRequest request);
}