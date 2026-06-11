namespace Threddit.Application.DTOs.Requests.Users;

public sealed record AssignSiteAdminRequest(
    Guid TargetUserId
);