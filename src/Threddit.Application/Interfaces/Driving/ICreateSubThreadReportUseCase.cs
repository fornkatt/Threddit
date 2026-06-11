using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface ICreateSubThreadReportUseCase
{
    /// <summary>Creates a report for a Post or Comment within a SubThread.</summary>
    /// <remarks>
    /// Any authenticated non-banned user may file a report.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.PostNotFound"/></item>
    ///     <item><see cref="ErrorType.CommentNotFound"/></item>
    ///     <item><see cref="ErrorType.InvalidReportType"/></item>
    ///     <item><see cref="ErrorType.PostDoesNotBelongToSubThread"/></item>
    ///     <item><see cref="ErrorType.CommentDoesNotBelongToPost"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<CreateReportResponse> ExecuteAsync(CreateSubThreadReportRequest request);
}