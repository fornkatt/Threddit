using System.Collections.Immutable;

namespace Threddit.Contracts.Responses.Reports;

public sealed record GetReportsApiResponse(IReadOnlyList<ReportApiDto> Reports);