namespace Threddit.Application.DTOs.Requests.Users;

public sealed record EditUserProfileRequest(
    Guid RequestingUserId,
    string? ProfilePicture = null,
    string? Description = null
);