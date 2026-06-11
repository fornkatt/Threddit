using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.SubThreads;

public sealed record AssignModeratorResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);