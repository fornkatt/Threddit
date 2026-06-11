using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Auth;

public sealed record RegistrationResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);