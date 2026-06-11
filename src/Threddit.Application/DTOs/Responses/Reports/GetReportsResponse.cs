using System.Collections.Immutable;
using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Reports;

public sealed record GetReportsResponse(
    bool IsSuccess,
    ImmutableList<ReportDto> Reports,
    string? Message = null,
    ErrorType? ErrorType = null
);