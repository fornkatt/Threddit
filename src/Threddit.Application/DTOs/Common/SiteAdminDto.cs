using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Common;

public sealed record SiteAdminDto(
    Guid Id,
    string Username,
    DateTime AssignedAt,
    ImmutableList<IssuedSiteBanDto> IssuedBans
);

public sealed record IssuedSiteBanDto(
    Guid BanId,
    Guid BannedUserId,
    string BannedUsername,
    string Reason,
    DateTime BannedAt,
    DateTime ExpiresAt,
    bool IsExpired
);