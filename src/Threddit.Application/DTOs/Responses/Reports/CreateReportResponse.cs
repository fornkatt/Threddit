using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Reports;

public sealed record CreateReportResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null,
    Guid? ReportId = null
);