namespace Threddit.Contracts.Responses.Reports;

public sealed record GetReportsApiResponse(IReadOnlyList<ReportApiDto> Reports);