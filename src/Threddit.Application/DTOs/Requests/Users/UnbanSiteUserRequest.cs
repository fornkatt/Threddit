namespace Threddit.Application.DTOs.Requests.Users;

public sealed record UnbanSiteUserRequest(
    Guid TargetUserId,
    Guid RequestingUserId
);