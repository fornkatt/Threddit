using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Reports;

public sealed record SetReportStatusResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);