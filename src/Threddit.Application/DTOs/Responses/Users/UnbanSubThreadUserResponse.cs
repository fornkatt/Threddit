using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Users;

public sealed record UnbanSubThreadUserResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);