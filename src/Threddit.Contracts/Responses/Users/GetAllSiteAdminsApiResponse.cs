namespace Threddit.Contracts.Responses.Users;

public sealed record GetAllSiteAdminsApiResponse(
    IReadOnlyList<SiteAdminApiDto> SiteAdmins
);

public sealed record SiteAdminApiDto(
    Guid Id,
    string Username,
    DateTime AssignedAt,
    IReadOnlyList<IssuedSiteBanApiDto> IssuedBans
);

public sealed record IssuedSiteBanApiDto(
    Guid BanId,
    Guid BannedUserId,
    string BannedUsername,
    string Reason,
    DateTime BannedAt,
    DateTime ExpiresAt,
    bool IsExpired
);