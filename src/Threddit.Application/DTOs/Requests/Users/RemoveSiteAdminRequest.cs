namespace Threddit.Application.DTOs.Requests.Users;

public sealed record RemoveSiteAdminRequest(
    Guid TargetUserId
);