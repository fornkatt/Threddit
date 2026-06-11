using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Users;

public sealed record GetUserProfileResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null,
    UserProfileDto? User = null
);