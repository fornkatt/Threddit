using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Auth;

public sealed record LoginResponse(
    bool IsSuccess,
    string? JwtToken = null,
    DateTime? ExpiresAt = null,
    string? Username = null,
    string? Message = null,
    ErrorType? ErrorType = null
);