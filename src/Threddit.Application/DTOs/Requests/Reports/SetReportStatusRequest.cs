using System.Collections.Immutable;
using Threddit.Domain.Entities;

namespace Threddit.Application.DTOs.Requests.Reports;

public sealed record SetReportStatusRequest(
    Guid ReportId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner,
    ImmutableArray<Guid> ModeratedSubThreadIds,
    Report.ReportStatus NewStatus
);