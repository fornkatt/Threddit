using Threddit.Domain.Entities;

namespace Threddit.Application.DTOs.Requests.Reports;

public sealed record GetSiteReportsRequest(
    Report.ReportStatus? StatusFilter = null,
    int Page = 1,
    int PageSize = 20
);