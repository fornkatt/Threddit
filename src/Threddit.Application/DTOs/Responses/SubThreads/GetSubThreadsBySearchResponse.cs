using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.SubThreads;

public sealed record GetSubThreadsBySearchResponse(
    bool IsSuccess,
    PagedResult<SubThreadSummaryDto>? SubThreads = null,
    string? Message = null,
    ErrorType? ErrorType = null
);