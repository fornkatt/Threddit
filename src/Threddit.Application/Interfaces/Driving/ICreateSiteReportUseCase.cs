using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface ICreateSiteReportUseCase
{
    /// <summary>Creates a site-wide report for a User, SubThread, or Direct Message.</summary>
    /// <remarks>
    /// Any authenticated non-banned user may file a report.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.NotFound"/></item>
    ///     <item><see cref="ErrorType.InvalidReportType"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<CreateReportResponse> ExecuteAsync(CreateSiteReportRequest request);
}