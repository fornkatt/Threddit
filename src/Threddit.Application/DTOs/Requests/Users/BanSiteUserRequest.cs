namespace Threddit.Application.DTOs.Requests.Users;

public sealed record BanSiteUserRequest(
    Guid TargetUserId,
    Guid RequestingUserId,
    string Reason,
    DateTime ExpiresAt
);