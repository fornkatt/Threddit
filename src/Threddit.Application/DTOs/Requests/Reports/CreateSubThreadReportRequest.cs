using Threddit.Domain.Entities;

namespace Threddit.Application.DTOs.Requests.Reports;

public sealed record CreateSubThreadReportRequest(
    string SubThreadName,
    Guid RequestingUserId,
    Report.ReportType Type,
    Guid TargetId,
    Report.ReportCategory Category,
    string? Message = null
);