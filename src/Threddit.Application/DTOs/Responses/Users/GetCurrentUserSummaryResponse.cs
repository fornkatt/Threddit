using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Users;

public sealed record GetCurrentUserSummaryResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null,
    CurrentUserSummaryDto? Summary = null
);