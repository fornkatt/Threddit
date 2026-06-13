using System.Collections.Immutable;
using Threddit.Domain.Entities;

namespace Threddit.Application.DTOs.Requests.Reports;

public sealed record SetReportStatusRequest(
    Guid ReportId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreadIds,
    Report.ReportStatus NewStatus
);